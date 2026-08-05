Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class ClasseAttivitaAdapter
    Public Shared Function ClasseAttivitaDaLavCod(Lav_Cod As Integer) As enum_Classi_Attivita

        Dim rval As enum_Classi_Attivita

        Select Case Lav_Cod

            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO,
                 LAVCOD_RILIEVO_INDICI_MATURITA,
                 LAVCOD_FASI_FENOLOGICHE

                rval = enum_Classi_Attivita.Rilievo

            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_DISERBO,
                 LAVCOD_DISSECCAMENTO, LAVCOD_GEODISINFESTAZIONE,
                 LAVCOD_CONCIA_SEME, LAVCOD_TRATTAMENTO_FITOREGOLATORE

                rval = enum_Classi_Attivita.Trattamento

            Case LAVCOD_SEMINA,
                    LAVCOD_TRAPIANTO,
                    LAVCOD_SOD_SEDDING,
                    LAVCOD_SOVESCIO

                rval = enum_Classi_Attivita.SeminaTrapianto

            Case LAVCOD_RACCOLTA

                rval = enum_Classi_Attivita.Raccolta

            Case LAVCOD_FERTIRRIGAZIONE, LAVCOD_CONCIMAZIONE_FOGLIARE,
                 LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                 LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                rval = enum_Classi_Attivita.Fertilizzazione

            Case Else

                rval = enum_Classi_Attivita.LavorazioneBase

        End Select

        Return rval

    End Function
End Class
