Imports AgronicaCoreVarieDAL
Imports AgronicaCoreDataProvider

Public Class RipartoCatasto

    Public Function GetRipartoCatastoElencoEntita(
        ByVal elencoEntitaCod As String(),
        ByRef objParametri_server As AgronicaCoreParametri
        ) As RipartoCatastoEsito

        Dim ripartoCatastoEsito = New RipartoCatastoEsito()
        ripartoCatastoEsito.TipoErrore = enumTipoErroreRipartoCatasto.Nessuno
        ripartoCatastoEsito.MessaggioErrore = ""
        ripartoCatastoEsito.TabellaRipartoCatasto = Nothing

        Dim leggiCfgDisabilitaIntersezione As New Configurazione_Siti_R

        Dim dtCfgDisabilitaIntersezione As DataTable = leggiCfgDisabilitaIntersezione.Leggi(
            0,
            "RipartoCatasto_Disabilita_VerificaIntersezione",
            "",
            "",
            objParametri_server)

        Dim abilitaControlloIntersezione As Boolean = True

        If dtCfgDisabilitaIntersezione.Rows.Count > 0 AndAlso dtCfgDisabilitaIntersezione.Rows(0)("valore") = "true" Then
            abilitaControlloIntersezione = False
        End If

        ' VAnni: 24/3/2017: ho garanzia di omogeneità da selezione in precedente

        ripartoCatastoEsito.TipoEntitaCod = Nothing

        LeggiTipoEntita(elencoEntitaCod(0), objParametri_server, ripartoCatastoEsito.TipoEntitaCod)

        If abilitaControlloIntersezione Then

            For Each xVerifica_Entita_Cod In elencoEntitaCod

                Dim leggiEntitaIntersezione As New AgronicaCoreGisDAL.GIS_Entita_R

                Dim dtLeggiEntitaIntersezione As DataTable

                dtLeggiEntitaIntersezione = leggiEntitaIntersezione.LeggiIntersezioni(
                    xVerifica_Entita_Cod,
                    True,
                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "",
                    "",
                    objParametri_server)

                If dtLeggiEntitaIntersezione.Rows.Count > 0 Then

                    Return ErroreEsistonoAltriPoligoniCheIntersecano()

                End If

            Next

        End If

        Dim LeggiRiparto As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

        Dim dtRiparto As DataTable

        For Each xVerifica_Entita_Cod In elencoEntitaCod

            dtRiparto = LeggiRiparto.LeggiConfrontoGIS(
                xVerifica_Entita_Cod,
                ripartoCatastoEsito.TipoEntitaCod,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_server)

            If IsNothing(ripartoCatastoEsito.TabellaRipartoCatasto) Then

                DtRipartoFinaleGenera(dtRiparto, ripartoCatastoEsito.TabellaRipartoCatasto)

            End If

            DtRipartoFinaleAppendiDati(dtRiparto, ripartoCatastoEsito.TabellaRipartoCatasto)

        Next

        Return ripartoCatastoEsito

    End Function

    Private Sub LeggiTipoEntita(
        entita_cod As String,
        ByRef objParametri_server As AgronicaCoreParametri,
        ByRef TipoEntita_cod As TipiEnumerativi.enum_GIS2012_TipoEntita)

        Dim LeggiTipoEntita As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim dtTipoEntita As DataTable

        dtTipoEntita = LeggiTipoEntita.LeggiCompleto_Entita_cod(
            entita_cod,
            "",
            "",
            objParametri_server)

        TipoEntita_cod = TipiEnumerativi.enum_GIS2012_TipoEntita.APPEZZAMENTI

        If dtTipoEntita.Rows.Count > 0 Then
            TipoEntita_cod = dtTipoEntita.Rows(0)("TipoEntita_Cod")
        End If

    End Sub

    Private Sub DtRipartoFinaleGenera(ByVal dtRiparto As DataTable, ByRef dtRipartoFinale As DataTable)

        dtRipartoFinale = dtRiparto.Clone()

    End Sub

    Private Sub DtRipartoFinaleAppendiDati(ByVal dtRiparto As DataTable, ByRef dtRipartoFinale As DataTable)

        dtRipartoFinale.Merge(dtRiparto)

    End Sub

#Region "Gestione Errori"

    Private Function ErroreEsistonoAltriPoligoniCheIntersecano() As RipartoCatastoEsito

        Dim ripartoCatastoEsito = New RipartoCatastoEsito() With {
            .TipoErrore = enumTipoErroreRipartoCatasto.EsistonoAltriPoligoniCheIntersecano,
            .MessaggioErrore = "Ci sono altri poligoni che intersecano questo. Pertanto non è possibile procedere con il calcolo delle intersezioni. Verificare anche se è stata selezionata una particella catastale al posto di un impianto.",
            .TabellaRipartoCatasto = Nothing
            }

        Return ripartoCatastoEsito

    End Function

#End Region

End Class

Public Class RipartoCatastoEsito

    Public TipoErrore As enumTipoErroreRipartoCatasto

    Public MessaggioErrore As String

    Public TipoEntitaCod As TipiEnumerativi.enum_GIS2012_TipoEntita

    Public TabellaRipartoCatasto As DataTable

End Class

Public Enum enumTipoErroreRipartoCatasto

    Nessuno = 0

    EsistonoAltriPoligoniCheIntersecano = 1

End Enum
