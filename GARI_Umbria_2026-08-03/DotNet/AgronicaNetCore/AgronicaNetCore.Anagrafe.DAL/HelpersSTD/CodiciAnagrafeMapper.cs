using System.Data;
using AgronicaCoreModelsSTD.anagrafiche;

namespace AgronicaNetCore.Anagrafe.DAL.HelpersSTD
{
    public static class CodiciAnagrafeMapper
    {
        public static List<CodiciAnagrafeValori> MapSTDFromDataTable(DataTable dt)
        {
            var codici = new List<CodiciAnagrafeValori>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
                codici.Add(MapSTDFromRow(row));
            return codici;
        }

        public static CodiciAnagrafeValori MapSTDFromRow(DataRow row)
        {
            return new CodiciAnagrafeValori
            {
                validita = new IntervalloTemporale(
                    (DateTime)row["Validita_Inizio"],
                    (DateTime)row["Validita_Fine"]
                ),
                valore = Convert.ToString(row["val_cod"]) ?? string.Empty,
                codiceAnagrafe = new CodiceAnagrafe((int)row["id_cod"], (string)row["descrizione"]),
            };
        }
    }
}
