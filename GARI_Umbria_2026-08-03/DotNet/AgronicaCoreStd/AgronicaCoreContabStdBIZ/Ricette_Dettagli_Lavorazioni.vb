Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Ricette_Dettagli_Lavorazioni

    Public Shared Sub LeggiDettagli(DettaglioDaLeggere As AgronicaCoreModelloSTD.Ricette_Dettagli_Lavorazioni, App_Dettagli As APP_Ricette_Dettagli)


        'parte comune:
        Ricette_Dettagli.LeggiDettagli(DettaglioDaLeggere, App_Dettagli)

    End Sub

    Public Shared Sub ScriviDettagli(Ricetta_Cod As Integer, Ricetta_Operazione_Cod As Integer, DettagliDaScrivere As AgronicaCoreModelloSTD.Ricette_Dettagli_Lavorazioni, App_Dettagli As APP_Ricette_Dettagli)

        Ricette_Dettagli.ScriviDettagli(Ricetta_Cod, Ricetta_Operazione_Cod, DettagliDaScrivere, App_Dettagli)


    End Sub


End Class
