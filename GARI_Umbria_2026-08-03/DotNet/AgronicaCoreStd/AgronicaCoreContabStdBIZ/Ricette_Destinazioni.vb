Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Ricette_Destinazioni

    Public Shared Sub LeggiDestinazioni(DistribuzioneDaPopolare As AgronicaCoreModelloSTD.Ricette_DistribuzioniSuImpianti, APP_Destinazioni As APP_Ricette_Destinazioni)

        DistribuzioneDaPopolare.Ricetta_Destinazione_Cod = APP_Destinazioni.Ricetta_Destinazione_Cod

        If DistribuzioneDaPopolare.Impianto Is Nothing Then
            DistribuzioneDaPopolare.Impianto = New AgronicaCoreModelloSTD.Reg_Impianti
        End If

        DistribuzioneDaPopolare.Impianto.piva = APP_Destinazioni.Piva
        DistribuzioneDaPopolare.Impianto.sa_cod = APP_Destinazioni.Sa_Cod
        DistribuzioneDaPopolare.Impianto.appezza = APP_Destinazioni.Appezza
        DistribuzioneDaPopolare.Impianto.id_reg = APP_Destinazioni.Id_Reg
        DistribuzioneDaPopolare.QuantitaDistribuita = APP_Destinazioni.Qta
        DistribuzioneDaPopolare.SuperficieTrattata = APP_Destinazioni.Qta2
        DistribuzioneDaPopolare.Data_Riferimento = APP_Destinazioni.Validita_Inizio

    End Sub



    Public Shared Sub ScriviDestinazioni(Ricetta_Cod As Integer, Ricetta_Operazione_Cod As Integer, Ricetta_Dettaglio_Cod As Integer, DistribuzioneDaScrivere As AgronicaCoreModelloSTD.Ricette_DistribuzioniSuImpianti, APP_Destinazioni As APP_Ricette_Destinazioni, SuperficeTotale As Decimal)

        APP_Destinazioni.Ricetta_Cod = Ricetta_Cod
        APP_Destinazioni.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod
        APP_Destinazioni.Ricetta_Dettaglio_Cod = Ricetta_Dettaglio_Cod
        APP_Destinazioni.Ricetta_Destinazione_Cod = DistribuzioneDaScrivere.Ricetta_Destinazione_Cod

        APP_Destinazioni.Piva = DistribuzioneDaScrivere.Impianto.piva
        APP_Destinazioni.Sa_Cod = DistribuzioneDaScrivere.Impianto.sa_cod
        APP_Destinazioni.Appezza = DistribuzioneDaScrivere.Impianto.appezza
        APP_Destinazioni.Id_Reg = DistribuzioneDaScrivere.Impianto.id_reg
        APP_Destinazioni.Qta = DistribuzioneDaScrivere.QuantitaDistribuita
        APP_Destinazioni.Qta2 = DistribuzioneDaScrivere.SuperficieTrattata
        APP_Destinazioni.Validita_Inizio = DistribuzioneDaScrivere.Data_Riferimento

        If SuperficeTotale > 0 AndAlso DistribuzioneDaScrivere.SuperficieTrattata > 0 Then
            APP_Destinazioni.QuotaDistribuzione = DistribuzioneDaScrivere.SuperficieTrattata / SuperficeTotale
        End If

    End Sub



End Class
