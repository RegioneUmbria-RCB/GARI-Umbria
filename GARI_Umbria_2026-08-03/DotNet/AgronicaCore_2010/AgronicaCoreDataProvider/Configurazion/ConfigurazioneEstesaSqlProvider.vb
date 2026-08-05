Public Class ConfigurazioneEstesaSqlProvider

    Public UtilizzaCache As Boolean = True
    Public CreaParametri As Boolean = True
    Public SlidingExpirationInMinuti As Integer = 60
    Public LimiteElementiClausoleIn As Integer = 50
    Public TempoWarningParser_Millisecondi As Integer?
    Public SQL_EseguiQueryOriginale As Boolean = False
    Public LanciaEccezioneSuInjection As Boolean = True

End Class
