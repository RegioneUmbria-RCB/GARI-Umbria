using System;
using System.Collections.Generic;
using System.Linq;

namespace AgronicaCoreDTOStd.InData.Demetra
{
    public class Contatto
    {

        public string Id_CF { get; set; }
        public string codice_contatto { get; set; }
        public string nome { get; set; }

        public string cognome { get; set; }

        public string Cod_Fisc { get; set; }
        public string partita_iva { get; set; }
        public string ragione_Sociale { get; set; }

        public List<RapportoContabile> ruolo { get; set; }

        public List<Documento> documenti { get; set; }

        public Contatto()
        {
            this.codice_contatto = String.Empty;
            this.Cod_Fisc = String.Empty;
            this.cognome = String.Empty;
            this.Id_CF = String.Empty;
            this.nome = String.Empty;
            this.partita_iva = String.Empty;
            this.ragione_Sociale = String.Empty;

            this.ruolo = new List<RapportoContabile>();
            this.documenti = new List<Documento>();

        }

        public void Normalizza()
        {
            if (this.Cod_Fisc == null || String.IsNullOrEmpty(this.Cod_Fisc))
                this.Cod_Fisc = String.Empty;
            if (this.cognome == null || String.IsNullOrEmpty(this.cognome))
                this.cognome = String.Empty;
            if (this.nome== null || String.IsNullOrEmpty(this.nome))
                this.nome = String.Empty;
            if (this.ragione_Sociale == null || String.IsNullOrEmpty(this.ragione_Sociale))
                this.ragione_Sociale = String.Empty;
            if (this.partita_iva == null || String.IsNullOrEmpty(this.partita_iva))
                this.partita_iva = String.Empty;

            if (this.ruolo == null)
                this.ruolo = new List<RapportoContabile>();

            if (this.documenti == null)
                this.documenti = new List<Documento>();

            if (this.documenti.Count() > 0)
                documenti.ForEach((d) => d.Normalizza());

        }

    }

    public class RapportoContabile
    {
        public string codice { get; set; }
        public string codice_ruolo_esterno { get; set; }

        public string cod_rapporto { get; set; }
        public string rapporto_des { get; set; }
        public bool flag_cancellazione { get; set; }
        public string utente_ultima_modifica { get; set; }

        [Demetra_Contatto_ActualData()]
        public DateTime data_ultima_modifica { get; set; }
        public Validita validita { get; set; }

        public RapportoContabile()
        {
            this.codice = String.Empty;
            this.codice_ruolo_esterno = String.Empty;

            this.cod_rapporto = String.Empty;
            this.rapporto_des = String.Empty;
            this.flag_cancellazione = false;
            this.utente_ultima_modifica = String.Empty;
            this.validita = new Validita();
        }

    }

    public class Validita
    {
        [Demetra_Contatto_MinData()]
        public DateTime? inizio { get; set; }
        [Demetra_Contatto_MaxData()]
        public DateTime? fine { get; set; }

        public bool IntersectsWith(Validita altraValidita)
        {
            //return !(inizio > altraValidita.fine | fine < altraValidita.inizio);
            if (this.fine < altraValidita.inizio || this.inizio > altraValidita.fine)
            {
                return false;
            }
            return true;

        }

        public override string ToString()
        {
            if (this.inizio == null || this.fine == null)
                return string.Empty;

            return String.Format("{0} - {1}", inizio.Value.ToString("dd/MM/yyyy"), fine.Value.ToString("dd/MM/yyyy"));
        }

    }

    public class Documento
    {
        public string codice { get; set; }

        public string codice_patentino_esterno { get; set; }

        [Demetra_Contatto_MaxData(erroreSeVuoto: true)]
        public DateTime? Data_Scadenza { get; set; }
        public string Descrizione { get; set; }

        [Demetra_Contatto_ActualData(erroreSeVuoto: true)]
        public DateTime? Data_Rilascio { get; set; }
        public string Numero { get; set; }

        public Documento()
        {
            this.Descrizione = String.Empty;
            this.Numero = String.Empty;
            this.codice = String.Empty;
            this.codice_patentino_esterno = String.Empty;
        }

        public void Normalizza()
        {
            if (this.Numero == null || String.IsNullOrEmpty(this.Numero))
                this.Numero = String.Empty;

            if (this.Data_Rilascio == null)
                this.Data_Rilascio = DateTime.MinValue;

            if (this.Data_Scadenza == null)
                this.Data_Scadenza = DateTime.MinValue;

        }
    }

    public class Demetra_Contatto_MinDataAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Demetra_Contatto_MaxDataAttribute : Attribute
    {
        public bool ErroreSeVuoto
        {
            get { return _erroreSeVuoto; }
        }

        bool _erroreSeVuoto = false;

        public Demetra_Contatto_MaxDataAttribute() { }

        public Demetra_Contatto_MaxDataAttribute(bool erroreSeVuoto)
        {
            _erroreSeVuoto = erroreSeVuoto;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Demetra_Contatto_ActualDataAttribute : Attribute
    {

        public bool ErroreSeVuoto
        {
            get { return _erroreSeVuoto; }
        }

        bool _erroreSeVuoto = false;

        public Demetra_Contatto_ActualDataAttribute() { }

        public Demetra_Contatto_ActualDataAttribute(bool erroreSeVuoto)
        {
            _erroreSeVuoto = erroreSeVuoto;
        }
    }

    public class ParametriInterscambioContatti { 
        public string FiltroAggRapportiContabili { get; set; }
        
        public ParametriInterscambioContatti() {
            FiltroAggRapportiContabili = "";
        }
    }

}
