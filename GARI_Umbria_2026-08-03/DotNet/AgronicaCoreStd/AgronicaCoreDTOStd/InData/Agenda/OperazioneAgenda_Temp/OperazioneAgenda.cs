
using AgronicaCoreDTOStd.InData.Budget;
using InData.Agenda;
using InData.Anagrafica;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Agenda.OperazioneAgenda_Temp
{
    public class OperazioneAgenda
    {
        public List<Movimento> Movimenti = new List<Movimento>();
        public List<Nota> Note = new List<Nota>();
        public List<Movimento_Dettaglio_Riferimento> Agenda_Riferimenti = new List<Movimento_Dettaglio_Riferimento>();
        //protected IEnumerable<GHG_Registrazione> GHG_Registrazioni;

        /// <see cref="enum_TipoOperazioneDB"/>
        public enum_TipoOperazioneDB Tipo_Operazione = enum_TipoOperazioneDB.Scrittura; // scrittura
        public int Id_Agenda = 0;
        public DateTime Data = new DateTime(1900, 1, 1);
        public string Piva = "";
        public int Sa_Cod = 0;
        public int Lav_Cod = 0;
        public string Des_Lib = "";
        public int Id_Attivita = 0;
        public int Audit_Cod = 0;
        public int Raccoglitore_Cod = 0;
        public int Pratica_Cod = 0;

        public int Blocco_Flag = 0;
        public string Blocco_Username = "";
        public DateTime Blocco_Data = new DateTime(1900, 1, 1);

        public int Linea_Cod = 0;
        public int Preparazione_Cod = 0;
        public int Id_Trasformazione = 0;
        public int Tipo_Accettazione = 0;
        public int Stato_Export = 0;
        public int Stato_Export_2 = 0;
        public int Tipo_Visibilita = 0;
        public int ChkCoge_Manuale = 0;
        public int Modulo = 0;
        public int Split = 0;

        public string Origine = "";
        public int Stato_Cod = 0;
        public int DaRemoto = 0;

        public string GisWkt = "";
        public string GisWktSistemaRiferimento = "";
        public string GisWktGps = "";
        public int GisTipoEntita_cod = 0;
        public int GisLayerCod = 0;

        // valori che poi il DAL in fase di scrittura sostituirà con quelli reali
        public DateTime Data_Creazione = new DateTime(1900, 1, 2);
        public DateTime Data_Modifica = new DateTime(1900, 1, 2);
        public string Username_Creazione = "";
        public string Username_Modifica = "";

        public int BaseCode = 0;
        public int TopCode = 200000000;

        /// <remarks>
        /// Non esiste in tabella Agenda, serve per il ribaltamento su Ricette_Operazioni
        /// </remarks>
        public int Invia_App;

        public OperazioneAgenda() { }

        public OperazioneAgenda(DateTime data, string piva, int lav_Cod)
        {
            Data = data;
            Piva = piva;
            Lav_Cod = lav_Cod;
        }

        public WriteAgenda ToWriteAgenda()
        {
            WriteAgenda wObj = new WriteAgenda(this.Piva, this.Sa_Cod, this.Id_Agenda);
            wObj.Lav_Cod = this.Lav_Cod;
            wObj.Linea_Cod = this.Linea_Cod;
            wObj.Preparazione_Cod = this.Preparazione_Cod;
            wObj.Des_Lib = this.Des_Lib;
            wObj.Tipo_Accettazione = this.Tipo_Accettazione;
            wObj.Blocco_Flag = this.Blocco_Flag;
            wObj.Blocco_Username = this.Blocco_Username;
            wObj.Blocco_Data = this.Blocco_Data;
            wObj.Validita_Inizio = this.Data;
            wObj.Audit_Cod = this.Audit_Cod;
            wObj.Stato_Export = this.Stato_Export;
            wObj.Stato_Export2 = this.Stato_Export_2;
            wObj.Tipo_Visibilita = this.Tipo_Visibilita;
            wObj.ChkCoge_Manuale = this.ChkCoge_Manuale;
            wObj.Id_Attivita = this.Id_Attivita;
            wObj.Modulo = this.Modulo;
            wObj.Raccoglitore_Cod = this.Raccoglitore_Cod;
            wObj.Split = this.Split;
            wObj.Pratica_Cod = this.Pratica_Cod;
            wObj.Origine = this.Origine;
            wObj.Stato_Cod = this.Stato_Cod;
            wObj.DaRemoto = this.DaRemoto;
            return wObj;
        }
    }
}
