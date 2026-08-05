using System.Data;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema;

namespace AgronicaNetCore.Anagrafe.DAL.HelpersSTD
{
    public static class IndirizzoMapper
    {
        private static readonly string[] _codiciItalia = { "", "IT", "ITA", "ITALIA", "italia", "Italia" };

        public static IndirizzoAssociato MapSTDFromRow(DataRow row, bool indirizzoCompleto)
        {
            var ind = new Indirizzo(Convert.ToInt32(row["cod_indirizzo"]));

            if (indirizzoCompleto)
            {
                ind.cap = Convert.ToString(row["CAP"]);
                ind.via = row["ind_des"] != DBNull.Value ? Convert.ToString(row["ind_des"]) : "";
                ind.frazione = row["frz_des"] != DBNull.Value ? Convert.ToString(row["frz_des"]) : "";
                ind.note = row["note"] != DBNull.Value ? Convert.ToString(row["note"]) : "";

                string statoCod = Convert.ToString(row["stato"]) ?? string.Empty;
                var statoNorm = _codiciItalia.Contains(statoCod)
                    ? new CodiciNazioniISO3166("IT") { descrizione = "Italia" }
                    : new CodiciNazioniISO3166(statoCod) { descrizione = Convert.ToString(row["Stato"]) };
                statoNorm.gestioneGerarchia = Convert.ToInt32(row["Gestione_Gerarchia_Geografica"]);
                ind.stato = statoNorm;

                ind.istatComune = new Istat
                {
                    cap = Convert.ToString(row["CAP"]),
                    codiceBelfiore = "",
                    prov = row["pro_cod_istat"] != DBNull.Value ? Convert.ToString(row["pro_cod_istat"]) : "",
                    com = row["com_cod_istat"] != DBNull.Value ? Convert.ToString(row["com_cod_istat"]) : "",
                    comuni_prov = row["pro_des"] != DBNull.Value ? Convert.ToString(row["pro_des"]) : "",
                    localita = row["com_des"] != DBNull.Value ? Convert.ToString(row["com_des"]) : "",
                    reg = row["reg"] != DBNull.Value ? Convert.ToString(row["reg"]) : "",
                };
            }

            return new IndirizzoAssociato
            {
                tipo_Indirizzo = Convert.ToInt32(row["Tipo_Indirizzo"]),
                indirizzo = ind,
            };
        }

        public static List<IndirizzoAssociato> MapSTDFromDataTable(DataTable dt, bool indirizzoCompleto)
        {
            var indirizzi = new List<IndirizzoAssociato>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
                indirizzi.Add(MapSTDFromRow(row, indirizzoCompleto));
            return indirizzi;
        }
    }
}
