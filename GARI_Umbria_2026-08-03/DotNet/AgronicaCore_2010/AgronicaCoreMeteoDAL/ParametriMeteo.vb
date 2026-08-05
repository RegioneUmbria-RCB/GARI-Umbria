
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Enum Enum_TimeGranularity
    Source = 0
    Hourly = 1
    Daily = 2
End Enum


Public Class ParametriMeteo

    Public TipoSorgente As enum_Meteo_Tiposorgente
    Public Id_Stazione As Integer
    Public StartPeriod As DateTime
    Public EndPeriod As DateTime
    Public GranularitaDati As Enum_TimeGranularity
    Public SogliaTermica As Decimal
    Public SogliaFabbisognoFreddo As Decimal
    Public WantPublic As Boolean
    Public WantForecast As Boolean

End Class
