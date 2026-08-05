Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq

Public Class MessaggiEsecuzioneGEE

    Public Function EstraiMessaggio(ByVal risultato As JObject,
                                    ByVal messaggiErrore As String,
                                    ByVal algoritmo_Cod As Int32) As String

        Dim messaggio As String

        If Not messaggiErrore.Equals("") Then
            Return messaggiErrore
        End If

        Select Case algoritmo_Cod
            Case TipiEnumerativi.enum_AlgoritmoProiezione.Potential_Deforestation_Yearly,
                 TipiEnumerativi.enum_AlgoritmoProiezione.Intact_Forest_Landscape,
                 TipiEnumerativi.enum_AlgoritmoProiezione.Soil_Erosion_by_Water

                Dim percentualeIntersezione = CDbl(risultato("RisultatoElaborazione")("CoveragePercentage"))

                If percentualeIntersezione = 0 Then
                    messaggio = "Il poligono non interseca l'elemento."
                Else
                    messaggio = String.Format("Il poligono interseca l'elemento per il {0}%.", percentualeIntersezione.ToString)
                End If
            Case TipiEnumerativi.enum_AlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE
                Dim numeroElementi = CType(risultato("RisultatoElaborazione"), JArray).Count

                messaggio = String.Format("N. elementi elaborati: {0}.", numeroElementi.ToString)
            Case Else
                messaggio = ""
        End Select

        Return messaggio

    End Function

End Class
