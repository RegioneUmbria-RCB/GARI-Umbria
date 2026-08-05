using System.Text;

namespace AgronicaNetCore.Base.Utility
{
    public static class LogRicetteHelper
    {
        public static void AggiungiCreazioneLogRicetteUltimaOperazione(StringBuilder sb)
        {
            sb.AppendLine(" SELECT ");
            sb.AppendLine("     Tipo, Chiave, Param1, Tipo_Operazione, ");
            sb.AppendLine("     ROW_NUMBER() OVER (PARTITION BY Tipo, Chiave ORDER BY Data_Ora_RegistrazioneLog DESC, ID DESC) AS NumeroRiga ");
            sb.AppendLine(" INTO ");
            sb.AppendLine("     #cteLogRicette ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     Agronica_Log_Ricette ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     Param3 = @piva ");
            sb.AppendLine("     AND Tipo = @tipo ");
            sb.AppendLine("     AND ISDATE(Param6) = 1 ");
            sb.AppendLine("     AND CONVERT(datetime, Param6, 103) >= @dataLavorazioneMin ");
            sb.AppendLine("     AND Data_Ora_RegistrazioneLog >= @dataUltimaSincro ");
            sb.AppendLine("     AND Param5 IN (@lavCods) ");

            sb.AppendLine(" ; ");

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     ISNULL(u.Chiave,'') AS Chiave, ");
            sb.AppendLine("     u.Param1, ");
            sb.AppendLine("     (CASE ");
            sb.AppendLine("         WHEN u.Tipo_Operazione = 1 AND p.Tipo_operazione IS NULL THEN 1 --'INSERT'");
            sb.AppendLine("         WHEN u.Tipo_Operazione = 1 AND p.Tipo_operazione = 3 THEN 2 --'MODIFICA'");
            sb.AppendLine("         WHEN u.Tipo_Operazione = 2 THEN 2 --'MODIFICA DIRETTA'");
            sb.AppendLine("         WHEN u.Tipo_Operazione = 3 THEN 3 --'DELETE'");
            sb.AppendLine("         ELSE u.Tipo_Operazione");
            sb.AppendLine("     END");
            sb.AppendLine("     ) As UltimaOperazione");
            sb.AppendLine(" INTO ");
            sb.AppendLine("     #ultimoLogRicette ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     #cteLogRicette u");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     #cteLogRicette p ");
            sb.AppendLine("     ON p.Tipo = u.Tipo  ");
            sb.AppendLine("     AND p.Chiave = u.Chiave ");
            sb.AppendLine("     AND p.NumeroRiga = 2 ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     u.NumeroRiga = 1");

            sb.AppendLine(" ; ");
        }
    }
}
