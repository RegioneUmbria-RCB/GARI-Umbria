Imports Newtonsoft.Json.Linq

Public Class Periodo
    Public DataInizio As DateTime
    Public DataFine As DateTime
    Public Output As IOT_Output
End Class

Public Class IOT_Output
    Public Dati As DataTable
    Public Sensori As DataTable
    Public Charts As JArray
    Public Riepilogo As DataTable
    Public RiepilogoSensori As DataTable
End Class


