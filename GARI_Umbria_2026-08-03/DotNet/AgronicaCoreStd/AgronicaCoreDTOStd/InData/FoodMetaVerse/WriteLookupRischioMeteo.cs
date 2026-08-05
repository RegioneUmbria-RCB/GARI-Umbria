using System;

namespace InData.FoodMetaVerse
{
    public class WriteLookupRischioMeteo
    {
        /// <summary>Identificativo univoco record</summary>
        public int Id { get; set; }

        /// <summary>CUAA Filiera</summary>
        public string Cuaa_Filiera { get; set; }

        /// <summary>CUAA Azienda</summary>
        public string Cuaa_Azienda { get; set; }

        /// <summary>Identificativo appezzamento</summary>
        public int Id_Appezzamento { get; set; }

        /// <summary>Identificativo esercizio</summary>
        public int Id_Esercizio { get; set; }

        /// <summary>Anno della campagna agraria</summary>
        public int Anno_Esercizio { get; set; }

        /// <summary>Codice specie vegetale</summary>
        public int Cod_Specie { get; set; }

        /// <summary>Codice varieta colturale</summary>
        public int? Cod_Varieta { get; set; }

        /// <summary>Codice ISO 3166-1 alpha-3</summary>
        public string Nazione { get; set; }

        /// <summary>Codice ISTAT regione geografica (max 5 caratteri), es. 01. Opzionale.</summary>
        public string Regione { get; set; }

        /// <summary>Centroide in WKT POINT</summary>
        public string Centroide_Wkt { get; set; }

        /// <summary>Poligono in WKT POLYGON</summary>
        public string Poligono_Wkt { get; set; }

        /// <summary>Codice EPSG coordinate</summary>
        public string Epsg { get; set; }

        /// <summary>Superficie in ettari</summary>
        public decimal Superficie_Ha { get; set; }

        /// <summary>Payload JSON richiesta M2</summary>
        public string Json_Richiesta { get; set; }

        /// <summary>Payload JSON risposta M2</summary>
        public string Json_Risposta { get; set; }

        /// <summary>Indicatore sintetico rischio gelo (0.0-100.0)</summary>
        public double? Rischio_Gelo { get; set; }

        /// <summary>Indicatore sintetico rischio siccita (0.0-100.0)</summary>
        public double? Rischio_Siccita { get; set; }

        /// <summary>Indicatore sintetico rischio allagamento (0.0-100.0)</summary>
        public double? Rischio_Allagamento { get; set; }

        /// <summary>Flag invio token: 0 = non inviato, 1 = inviato</summary>
        public short? Inviato { get; set; }

        /// <summary>Data/ora di invio del token</summary>
        public DateTime? DataInvio { get; set; }

        /// <summary>Data/ora di creazione del record, UTC</summary>
        public DateTime Data_Creazione { get; set; }

        /// <summary>Data/ora di ultima modifica del record, UTC</summary>
        public DateTime Data_Modifica { get; set; }

        /// <summary>Username che ha creato il record</summary>
        public string Username_Creazione { get; set; }

        /// <summary>Username che ha effettuato l'ultima modifica</summary>
        public string Username_Modifica { get; set; }

        /// <summary>Data inizio validita del record</summary>
        public DateTime Validita_Inizio { get; set; }

        /// <summary>Data fine validita del record</summary>
        public DateTime Validita_Fine { get; set; }

        /// <summary>Data/ora invocazione motore M2, UTC</summary>
        public DateTime Data_Invocazione { get; set; }

        public WriteLookupRischioMeteo() { }
    }
}