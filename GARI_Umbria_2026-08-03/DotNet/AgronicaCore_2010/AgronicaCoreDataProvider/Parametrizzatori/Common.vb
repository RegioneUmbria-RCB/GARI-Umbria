Public Enum ScalarFunctionsNoCheck
    ISNULL
    DATEDIFF
    DATEADD
    YEAR
    AVG
    COUNT
    MAX
    MIN
    SUM
    MONTH
    DAY
    IIF
    ISNUMERIC
    NULLIF
    CAST
    LTRIM
    RTRIM
End Enum

Public Enum ParametrizzatoreType
    PARAMETRIZZATORE = 1
    PARAMETRIZZATORE_NEW = 2
End Enum

Public Class ParametroSql
    Public Valore As Object
    Public StringaDaSostituire As String
    Public Riga As Integer
    Public PosizioneInizio As Integer
    Public PosizioneFine As Integer
    Public Tipo As String
End Class