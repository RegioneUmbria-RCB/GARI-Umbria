Imports AgronicaCoreContabStdBIZ
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate

Public Class Ricette_Dettagli_Trattamenti

    Public Shared Sub LeggiDettagli(dbContext As GiasDbContext, pivaPerLetturaProdotti As String, DettaglioDaLeggere As AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento, App_Dettagli As APP_Ricette_Dettagli, app_Tecnico As APP_Ricette_Dettaglio_Tecnico, APP_Dettaglio_Magazzino As APP_Ricette_Dettagli, APP_Destinazioni As List(Of APP_Ricette_Destinazioni))


        'parte comune:
        Ricette_Dettagli.LeggiDettagli(DettaglioDaLeggere, App_Dettagli)

        'parte prodotti:
        Ricette_Dettagli_Prodotti.LeggiDettagli(dbContext, pivaPerLetturaProdotti, DettaglioDaLeggere, App_Dettagli, APP_Dettaglio_Magazzino, APP_Destinazioni)


        'parte trattamenti:        
        Dim letturaAvversita As New AgronicaCoreAnagrafeStdDAL.Avversita_R
        Dim letturaGruppi As New AgronicaCoreAnagrafeStdDAL.GruppoAvversita_R

        If app_Tecnico IsNot Nothing Then
            Dim appAvv As APP_Avversita = letturaAvversita.Leggi(dbContext, app_Tecnico.Av_Cod)
            Dim appGru As APP_GruppoAvversita = letturaGruppi.Leggi(dbContext, app_Tecnico.Av_Gru)

            If appAvv IsNot Nothing AndAlso app_Tecnico.Av_Cod <> 0 Then 
                DettaglioDaLeggere.AvversitaGruppo =
                    New AgronicaCoreModelloSTD.AvversitaConGruppi With {.Cod = "0|" & appAvv.Av_Cod, .DescrizioneAvversitaGruppo = appAvv.Av_Des_Vol}
            ElseIf appGru IsNot Nothing Then
                DettaglioDaLeggere.AvversitaGruppo =
                    New AgronicaCoreModelloSTD.AvversitaConGruppi With {.Cod = appGru.Av_Gru & "|0", .DescrizioneAvversitaGruppo = appGru.Av_Gru_Des}
            End If
        End If

    End Sub

    Public Shared Sub ScriviDettagli(Ricetta_Cod As Integer, Ricetta_Operazione_Cod As Integer, DettagliDaScrivere As AgronicaCoreModelloSTD.Ricette_Dettagli_Trattamento, App_Dettagli As APP_Ricette_Dettagli, app_Tecnico As APP_Ricette_Dettaglio_Tecnico)


        Ricette_Dettagli_Prodotti.ScriviDettagli(Ricetta_Cod, Ricetta_Operazione_Cod, DettagliDaScrivere, App_Dettagli)

        If app_Tecnico IsNot Nothing Then

            'chiavi
            app_Tecnico.Ricetta_Cod = Ricetta_Cod
            app_Tecnico.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod
            app_Tecnico.Ricetta_Dettaglio_Cod = DettagliDaScrivere.Ricetta_Dettaglio_Cod

            'avversità o gruppo.
            Dim avOppureG As String() = DettagliDaScrivere.AvversitaGruppo.Cod.Split(CChar("|"))
            app_Tecnico.Av_Cod = CInt(avOppureG(1))
            app_Tecnico.Av_Gru = CInt(avOppureG(0))

            'valori predefiniti..:
            app_Tecnico.Inn1_data = AGRODATAINIZIO
            app_Tecnico.Inn2_data = AGRODATAINIZIO

        End If

    End Sub
End Class
