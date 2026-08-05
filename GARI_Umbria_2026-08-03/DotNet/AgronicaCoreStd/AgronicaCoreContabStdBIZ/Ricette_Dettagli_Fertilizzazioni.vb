Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate
Imports AgronicaCoreEntityFrameworkSTD

Public Class Ricette_Dettagli_Fertilizzazioni

    Public Shared Sub LeggiDettagli(dbcontext As GiasDbContext, pivaPerLetturaProdotti As String, DettaglioDaLeggere As AgronicaCoreModelloSTD.Ricette_Dettagli_Fertilizzazione, App_Dettagli As APP_Ricette_Dettagli, app_Tecnico As APP_Ricette_Dettaglio_Tecnico, APP_DettaglioMagazzino As APP_Ricette_Dettagli, App_Destinazioni As List(Of APP_Ricette_Destinazioni))

        'parte comune:
        Ricette_Dettagli.LeggiDettagli(DettaglioDaLeggere, App_Dettagli)

        'parte prodotti
        Ricette_Dettagli_Prodotti.LeggiDettagli(dbcontext, pivaPerLetturaProdotti, DettaglioDaLeggere, App_Dettagli, APP_DettaglioMagazzino, App_Destinazioni)

        'parte fertilizzazioni
        DettaglioDaLeggere.N = app_Tecnico.N
        DettaglioDaLeggere.P = app_Tecnico.P
        DettaglioDaLeggere.K = app_Tecnico.K
        DettaglioDaLeggere.CU = app_Tecnico.CU

    End Sub


    Public Shared Sub ScriviDettagli(Ricetta_Cod As Integer, Ricetta_Operazione_Cod As Integer, DettagliDaScrivere As AgronicaCoreModelloSTD.Ricette_Dettagli_Fertilizzazione, App_Dettagli As APP_Ricette_Dettagli, app_Tecnico As APP_Ricette_Dettaglio_Tecnico)


        Ricette_Dettagli_Prodotti.ScriviDettagli(Ricetta_Cod, Ricetta_Operazione_Cod, DettagliDaScrivere, App_Dettagli)

        'chiavi, scrivi         
        app_Tecnico.Ricetta_Cod = Ricetta_Cod
        app_Tecnico.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod
        app_Tecnico.Ricetta_Dettaglio_Cod = DettagliDaScrivere.Ricetta_Dettaglio_Cod

        'avversità
        app_Tecnico.N = DettagliDaScrivere.N
        app_Tecnico.P = DettagliDaScrivere.P
        app_Tecnico.K = DettagliDaScrivere.K
        app_Tecnico.CU = DettagliDaScrivere.CU


        If app_Tecnico.Inn1_data < AGRODATAINIZIO Then
            app_Tecnico.Inn1_data = AGRODATAINIZIO
        End If

        If app_Tecnico.Inn2_data < AGRODATAINIZIO Then
            app_Tecnico.Inn2_data = AGRODATAFINE
        End If


    End Sub


End Class
