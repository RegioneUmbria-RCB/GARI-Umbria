Imports AgronicaCoreContabStdBIZ
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate
Public Class Ricette_Dettagli_Rilievi


    Public Shared Sub LeggiDettagli(dbContext As GiasDbContext, DettaglioDaLeggere As AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi, app_Dettagli As APP_Ricette_Dettagli, app_Tecnico As APP_Ricette_Dettaglio_Tecnico, app_Destinazione As APP_Ricette_Destinazioni)

        'parte comune:
        Ricette_Dettagli.LeggiDettagli(DettaglioDaLeggere, app_Dettagli)



        'parte di rilievi (comune)
        Dim xImpiantoLettura As New AgronicaCoreAnagrafeStdDAL.Reg_Impianti_R
        Dim impiantoDes As APP_Reg_Impianti =
                xImpiantoLettura.LeggiImpianto(dbContext, app_Destinazione.Piva, app_Destinazione.Sa_Cod, app_Destinazione.Appezza, app_Destinazione.Id_Reg)

        Dim xSaLettura As New AgronicaCoreAnagrafeStdDAL.CentriAziendali_R
        Dim sa As AgronicaCoreModelloSTD.Centri_Aziendali =
                xSaLettura.EstraiListaCentriAziendali(dbContext, app_Destinazione.Piva).Where(Function(g) g.Sa_Cod = app_Destinazione.Sa_Cod).FirstOrDefault

        DettaglioDaLeggere.Impianto = SalvaRilievoComponiDescrizione(sa, impiantoDes)

        'Avversità
        If app_Tecnico.Av_Cod <> 0 OrElse app_Tecnico.Av_Gru <> 0 Then

            Dim xLetturaAV As New AgronicaCoreAnagrafeStdDAL.MisuraXAvversita_R
            Dim ffDes As AgronicaCoreModelloSTD.MisuraXAvversita =
                xLetturaAV.Leggi(dbContext, 0, app_Tecnico.Av_Cod, app_Tecnico.Av_Gru, app_Tecnico.Dett_Cod).FirstOrDefault

            If ffDes IsNot Nothing Then
                DettaglioDaLeggere.Descrizione = ffDes.MisuraAvversitaDescrizione
            End If
            DettaglioDaLeggere.Avversita = app_Tecnico.Av_Cod
            DettaglioDaLeggere.GruppoAvversita = app_Tecnico.Av_Gru
            DettaglioDaLeggere.UnitaDiMisuraCod = app_Tecnico.Dett_Cod
            DettaglioDaLeggere.QtaRilevata = app_Destinazione.Qta
            DettaglioDaLeggere.QtaRilevataString = app_Destinazione.Qta.ToString()
        End If

        'Fase Fenologica (la data è stata specificata)
        If app_Tecnico.FF_Classe <> 0 AndAlso app_Destinazione.Validita_Inizio <> AGRODATAINIZIO Then

            Dim xLetturaFF As New AgronicaCoreAnagrafeStdDAL.SpecieVegetaliXStadiCrescita_R
            Dim ffDes As AgronicaCoreModelloSTD.SpecieVegetaliXStadiCrescita =
                xLetturaFF.EstraiListaSpecieVegetaliXStadiCrescitaFiltrataPerSpecieBBCH(dbContext, 0, app_Tecnico.FF_Classe).FirstOrDefault


            DettaglioDaLeggere.FaseFenologica = app_Tecnico.FF_Classe
            DettaglioDaLeggere.DataOraRilievo = app_Destinazione.Validita_Inizio
            DettaglioDaLeggere.Descrizione = ffDes.Descrizione
            DettaglioDaLeggere.QtaRilevataString = app_Destinazione.Validita_Inizio.ToShortDateString()
        End If

        'Indici Maturità
        If app_Tecnico.FF_Classe <> 0 AndAlso app_Destinazione.Validita_Inizio = AGRODATAINIZIO Then


            Dim xLetturaAV As New AgronicaCoreAnagrafeStdDAL.IndiciMaturita_R
            Dim ffDes As AgronicaCoreModelloSTD.MisuraXindiciMaturita =
                xLetturaAV.EstraiListaIndiciMaturitaFiltrataPerSpecie(dbContext, 0, app_Tecnico.FF_Classe, app_Tecnico.Dett_Cod).FirstOrDefault

            If ffDes IsNot Nothing Then
                DettaglioDaLeggere.Descrizione = ffDes.MisuraIndiciMaturitaDescrizione
            End If

            DettaglioDaLeggere.IndiceMaturita = app_Tecnico.FF_Classe
            DettaglioDaLeggere.UnitaDiMisuraCod = app_Tecnico.Dett_Cod
            DettaglioDaLeggere.QtaRilevata = app_Destinazione.Qta
            DettaglioDaLeggere.QtaRilevataString = app_Destinazione.Qta.ToString()

        End If

    End Sub

    Private Shared Function SalvaRilievoComponiDescrizione(ByVal sa As AgronicaCoreModelloSTD.Centri_Aziendali, ByVal impianto As APP_Reg_Impianti) As String
        Dim rval As String
        rval = String.Format("{0} {1}", sa.Sa_Nome, impianto.app_nome)
        Return rval
    End Function

    Public Shared Sub ScriviDettagli(Ricetta_Cod As Integer, Ricetta_Operazione_Cod As Integer, DettagliDaScrivere As AgronicaCoreModelloSTD.Ricette_Dettagli_Rilievi, App_Dettagli As APP_Ricette_Dettagli, app_Tecnico As APP_Ricette_Dettaglio_Tecnico)


        Ricette_Dettagli.ScriviDettagli(Ricetta_Cod, Ricetta_Operazione_Cod, DettagliDaScrivere, App_Dettagli)

        App_Dettagli.Mezzo_Det = -1

        If app_Tecnico IsNot Nothing Then

            'chiavi
            app_Tecnico.Ricetta_Cod = Ricetta_Cod
            app_Tecnico.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod
            app_Tecnico.Ricetta_Dettaglio_Cod = DettagliDaScrivere.Ricetta_Dettaglio_Cod

            'fase fenologica
            If DettagliDaScrivere.FaseFenologica <> 0 Then
                app_Tecnico.FF_Classe = DettagliDaScrivere.FaseFenologica
            End If

            'indice maturità
            If DettagliDaScrivere.IndiceMaturita <> 0 Then
                app_Tecnico.FF_Classe = DettagliDaScrivere.IndiceMaturita
                app_Tecnico.Dett_Cod = DettagliDaScrivere.UnitaDiMisuraCod
            End If

            'rilievo avversità
            If DettagliDaScrivere.IndiceMaturita = 0 AndAlso DettagliDaScrivere.FaseFenologica = 0 Then
                app_Tecnico.Dett_Cod = DettagliDaScrivere.UnitaDiMisuraCod
                app_Tecnico.Av_Cod = DettagliDaScrivere.Avversita
                app_Tecnico.Av_Gru = DettagliDaScrivere.GruppoAvversita
            End If

            If app_Tecnico.Inn1_data < AGRODATAINIZIO Then
                    app_Tecnico.Inn1_data = AGRODATAINIZIO
                End If

                If app_Tecnico.Inn2_data < AGRODATAINIZIO Then
                    app_Tecnico.Inn2_data = AGRODATAFINE
                End If
            End If

    End Sub

End Class
