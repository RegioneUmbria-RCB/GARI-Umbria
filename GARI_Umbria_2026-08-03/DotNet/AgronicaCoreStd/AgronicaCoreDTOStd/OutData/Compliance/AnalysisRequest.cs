using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;

namespace AgronicaCoreDTOStd.Compliance
{
    // TODO fix campi + add doc
    public class AnalysisRequest
    {
        public enum Status
        {
            Error = -1,
            ToProcess = 1,
            Processing = 2,
            Completed = 3
        }

        [JsonPropertyName("data_richiesta")]
        public DateTime DataRichiesta { get; set; }

        [JsonPropertyName("username_creazione")]
        public string UsernameCreazione { get; set; }

        [JsonPropertyName("data_da")]
        public DateTime IntervalloInizio { get; set;  }
        
        [JsonPropertyName("data_a")]
        public DateTime IntervalloFine { get; set; }
        
        [JsonPropertyName("idTestata")]
        public int IdTestata { get; set; }
        
        [JsonPropertyName("piva")]
        public string Piva { get; set; }
        
        [JsonPropertyName("rag_soc")]
        public string RagSoc { get; set; }
        
        [JsonPropertyName("sa_cod")]
        public int SaCod { get; set; }
        
        [JsonPropertyName("sa_nome")]
        public string SaNome { get; set; }
        
        [JsonPropertyName("veg_cod")]
        public int VegCod { get; set; }
        
        [JsonPropertyName("veg_des")]
        public string VegDes { get; set; }
        
        [JsonPropertyName("impianti")]
        public string Impianti { get; set; }
        
        [JsonPropertyName("impianti_des")]
        public string ImpiantiDes { get; set; }
        
        [JsonPropertyName("operazioni")]
        public string Operazioni { get; set; }
        
        [JsonPropertyName("operazioni_des")]
        public string OperazioniDes { get; set; }
        
        [JsonPropertyName("dpi")]
        public string Dpi { get; set; }
        
        [JsonPropertyName("dpi_des")]
        public string DpiDes { get; set; }
        
        [JsonPropertyName("flagIaf")]
        public bool IAF { get; set; }
        
        [JsonPropertyName("flagImpostazioni")]
        public bool UserSettings { get; set; }
        
        [JsonPropertyName("flagMagazzino")]
        public bool Storage { get; set; }

        [JsonPropertyName("flagNormative")]
        public bool Regulations { get; set; }

        [JsonPropertyName("status")]
        public Status RequestStatus { get; set; }

        [JsonPropertyName("controlloRiduzioneDiserbo")]
        public Boolean ControlloRiduzioneDiserbo { get; set; }

        [JsonPropertyName("origin")]
        public int Origin { get; set; }

        [JsonPropertyName("error")]
        public string Errore { get; set; }


        /// <summary>
        /// Indicates if the request is compliant to the regulations.
        /// </summary>
        /// <remarks>1 = compliant; -1 = not compliant; 0 = undefined</remarks>
        public Nullable<short> RegulationCompliance { get; set; }
        /// <summary>
        /// Indicates if the request is compliant to storage regulations.
        /// </summary>
        /// <remarks>1 = compliant; -1 = not compliant; 0 = undefined</remarks>
        public Nullable<short> StorageCompliance { get; set; }

        public static AnalysisRequest FromDataRow(DataRow row)
        {
            Func<DataRow, String, String> stringOrDefault = (DataRow r, string field) => r[field] != System.DBNull.Value ? (string)row[field] ?? "" : "";

            AnalysisRequest x = new AnalysisRequest();
            x.IdTestata = int.Parse(row["Id_Testata"].ToString());
            x.IntervalloInizio = (DateTime)row["Intervallo_Inizio"];
            x.IntervalloFine = (DateTime)row["Intervallo_Fine"];
            x.DataRichiesta = (DateTime)row["Data_Creazione"];
            x.Piva = stringOrDefault(row, "Piva");
            x.RagSoc = stringOrDefault(row, "Rag_Soc");
            x.SaCod = int.Parse(row["Sa_Cod"].ToString());
            x.SaNome = stringOrDefault(row, "Sa_Nome");
            x.VegCod = int.Parse(row["Veg_Cod"].ToString());
            x.VegDes = stringOrDefault(row, "Veg_Des");
            x.Dpi = stringOrDefault(row, "Dpi_Cod");
            x.DpiDes = stringOrDefault(row, "dpi_des");
            x.IAF = (bool)row["Flag_Verifica_IAF"];
            x.UserSettings = (bool)row["Flag_Solo_Controlli_Utente"];
            x.Storage = (bool)row["Flag_Verifica_Magazzino"];
            x.Regulations = (bool)row["Flag_Verifica_Normative"];
            x.Operazioni = stringOrDefault(row, "Tipo_Operazioni");
            x.RequestStatus  = (Status)Enum.Parse(typeof(Status), row["Status"].ToString());
            x.Origin = int.Parse(row["Origine"].ToString());
            x.UsernameCreazione = stringOrDefault(row, "Username_Creazione");
            x.Errore = stringOrDefault(row, "Errore");
            x.StorageCompliance = row["Conforme_Magazzino"] == DBNull.Value ? new Nullable<short>() : (short)row["Conforme_Magazzino"];
            x.RegulationCompliance = row["Conforme_Normative"] == DBNull.Value ? new Nullable<short>() : (short)row["Conforme_Normative"];
            return x;
        }
    }
}
