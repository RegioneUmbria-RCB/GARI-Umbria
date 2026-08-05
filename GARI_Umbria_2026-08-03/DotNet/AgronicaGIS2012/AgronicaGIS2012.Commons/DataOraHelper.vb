Public Class DataOraHelper
    Public Shared Function CreateISO8601DateTimeFromSystemDateTime(ByVal dataora As DateTime)
        Return dataora.Year.ToString & "-" & dataora.Month.ToString.PadLeft(2, "0") & "-" & dataora.Day.ToString.PadLeft(2, "0") & "T00:00:00" '& dataora.Hour.ToString & ":" & dataora.Minute.ToString & ":" & dataora.Second.ToString
    End Function
End Class
