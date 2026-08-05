Imports AgronicaCoreModelloSTD

Public MustInherit Class Ricette_Dettagli
    Implements iRicette_Dettagli

    Public Property Ricetta_Dettaglio_Cod As Integer Implements iRicette_Dettagli.Ricetta_Dettaglio_Cod

    Public Property ImpiantiInteressati As List(Of Ricette_DistribuzioniSuImpianti) Implements iRicette_Dettagli.ImpiantiInteressati

    Public Property Operazione As Ricette_Operazioni


End Class
