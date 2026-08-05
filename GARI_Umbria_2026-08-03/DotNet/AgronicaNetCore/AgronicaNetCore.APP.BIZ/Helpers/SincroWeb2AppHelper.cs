using System;

namespace AgronicaNetCore.APP.BIZ.Helpers
{
    public static class SincroWeb2AppHelper
    {
        public static DateTime? ParseTimestamp(string? timestamp)
        {
            return DateTime.TryParse(
                timestamp,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out var parsedTs) ? parsedTs : null;
        }
    }
}