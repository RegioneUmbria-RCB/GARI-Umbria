Public Class GruppiUtenteCfgAggiuntive

    Public impostaSelezionabileSeNonDestinazione As Boolean
    Public FiltrinoListaServiziStati As List(Of GruppiUtenteCfgAggiuntive_ServizioStato)

End Class
Public Class GruppiUtenteCfgAggiuntive_ServizioStato

    Public WWorkFlow_Cod As Integer

    Public Servizio_cod As Integer

    Public Stato_cod As Integer

    Public destinazione As String

End Class
