using System.Text;

namespace AgronicaNetCore.Base.Utility
{
    public static class LogAgendaHelper
    {
        public static void AggiungiCreazioneLogAgendaUltimaOperazione(StringBuilder sb)
        {
            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Id_Agenda, Piva, Lav_Cod, Data_Ora_Lavorazione, ");
            sb.AppendLine("     Origine, Tipo_Operazione, ");
            sb.AppendLine("     ROW_NUMBER() OVER (PARTITION BY Piva, Id_Agenda ORDER BY Data_Ora_RegistrazioneLog DESC, ID DESC) AS NumeroRiga");
            sb.AppendLine(" INTO ");
            sb.AppendLine("     #cteLogAgenda ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     Agronica_Log_Agenda ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Piva = @piva");
            sb.AppendLine("     AND Data_Ora_Lavorazione >= @dataLavorazioneMin");
            sb.AppendLine("     AND Data_Ora_RegistrazioneLog >= @dataUltimaSincro");
            sb.AppendLine($"     AND Lav_Cod IN (@lavCods)");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     ISNULL(u.Id_Agenda, 0) AS Id_Agenda, u.Data_Ora_Lavorazione, ");
            sb.AppendLine("     u.Lav_Cod, ISNULL(u.Piva, '') AS Piva, ");
            sb.AppendLine("     u.Origine, ");
            sb.AppendLine("     CASE ");
            sb.AppendLine("         WHEN u.Tipo_Operazione = 1 AND p.Tipo_Operazione IS NULL THEN 1 "); //INSERT
            sb.AppendLine("         WHEN u.Tipo_Operazione = 1 AND p.Tipo_Operazione = 3    THEN 2 "); //re-INSERT dopo DELETE
            sb.AppendLine("         WHEN u.Tipo_Operazione = 2                              THEN 2 "); //MODIFICA DIRETTA
            sb.AppendLine("         WHEN u.Tipo_Operazione = 3                              THEN 3 "); //DELETE
            sb.AppendLine("         ELSE u.Tipo_Operazione ");
            sb.AppendLine("     END AS UltimaOperazione ");
            sb.AppendLine(" INTO ");
            sb.AppendLine("     #ultimoLogAgenda ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     #cteLogAgenda u ");
            sb.AppendLine(" LEFT JOIN  ");
            sb.AppendLine("     #cteLogAgenda p");
            sb.AppendLine("     ON p.Id_Agenda = u.Id_Agenda ");
            sb.AppendLine("     AND p.NumeroRiga = 2 ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     u.NumeroRiga = 1 ");
            sb.AppendLine("; ");
        }
    }
}
