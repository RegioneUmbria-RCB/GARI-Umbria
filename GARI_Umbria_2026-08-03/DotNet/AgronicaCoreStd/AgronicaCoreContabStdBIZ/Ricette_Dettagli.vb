Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Ricette_Dettagli

    Public Shared Sub LeggiDettagli(DettaglioDaLeggere As AgronicaCoreModelloSTD.Ricette_Dettagli, App_Dettagli As APP_Ricette_Dettagli)

        DettaglioDaLeggere.Ricetta_Dettaglio_Cod = App_Dettagli.Ricetta_Dettaglio_Cod

    End Sub

    Public Shared Sub ScriviDettagli(Ricetta_Cod As Integer, Ricetta_Operazione_Cod As Integer, DettagliDaScrivere As AgronicaCoreModelloSTD.Ricette_Dettagli, App_Dettagli As APP_Ricette_Dettagli)

        App_Dettagli.Ricetta_Cod = Ricetta_Cod
        App_Dettagli.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod
        App_Dettagli.Ricetta_Dettaglio_Cod = DettagliDaScrivere.Ricetta_Dettaglio_Cod

    End Sub


End Class
