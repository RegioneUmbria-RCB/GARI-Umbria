Imports System.Globalization

Public Class TimeZoneUtility

    Public Shared Function Get_TZ_Short_Id() As String

        Dim tzInfo = TimeZoneInfo.Local
        Dim regInfo = RegionInfo.CurrentRegion.TwoLetterISORegionName

        Return TimeZoneConverter.TZConvert.WindowsToIana(tzInfo.Id, regInfo)

    End Function

End Class
