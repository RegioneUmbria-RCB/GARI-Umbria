
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi



Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreEntityFramework_POCO.MeteoSuite

Public Class CompletezzaDati

    ''' <summary>
    ''' Su ciascun elemento temporale passato nel parametro DatoMeteoDaRicostruire vengono applicati gli algoritmi di ricostruzione come da parametro ConfigRicostruzione
    ''' </summary>
    ''' <param name="DatoMeteoDaRicostruire"></param>
    ''' <param name="DonfigRicostruzione"></param>
    ''' <param name="DatoMeteoRicostruto">Oggetto restituito con i dati ricostruiti</param>
    ''' <returns></returns>
    Public Function RicostruisciDatiMancanti(
            ByVal Stazione_Cod As Integer,
            ByVal DatoMeteoDaRicostruire As List(Of DateTime),
            ByVal ConfigRicostruzione As List(Of Agronica_Stazioni_Configurazioni),
            ByRef DatoMeteoRicostruto As List(Of DatoSingolo),
            objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As RispostaStandard


        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        Dim rv1 As New RispostaStandard

        Dim NomeColonnaPrecedente As String = ""


        Try

            For Each iteraDatoMeteoDaRicostruire In DatoMeteoDaRicostruire

                NomeColonnaPrecedente = ""
                rv1.RispostaConferma = False

                For Each iteraConfigRicostruzione In ConfigRicostruzione

                    'se la ricostruzione non va a buon fine si procede con algoritmo successivo..                    

                    If NomeColonnaPrecedente <> iteraConfigRicostruzione.NomeColonna OrElse Not rv1.RispostaConferma Then


                        Dim ParametriAggiutiviRicostruzione As RicostruzioneDatoParametriAggiuntivi =
                        JsonConvert.DeserializeObject(Of RicostruzioneDatoParametriAggiuntivi)(iteraConfigRicostruzione.ParametriAggiuntivi)

                        Select Case iteraConfigRicostruzione.AlgoritmoRicostruizioneDatiMancanti
                            Case enum_MeteoAlgoritmiNormalizzazione.LeggiDatiPrecedenti
                                rv1 = RicostruisciDatiMancanti_LeggiDatiPrecedenti(
                                    Stazione_Cod,
                                    iteraDatoMeteoDaRicostruire,
                                    iteraConfigRicostruzione,
                                    ParametriAggiutiviRicostruzione,
                                    DatoMeteoRicostruto,
                                    objParametri
                                )
                            Case enum_MeteoAlgoritmiNormalizzazione.LeggiDatoSuSerieStoriche
                                rv1 = RicostruisciDatiMancanti_LeggiDatiSerieStoriche(
                                    Stazione_Cod,
                                    iteraDatoMeteoDaRicostruire,
                                    iteraConfigRicostruzione,
                                    ParametriAggiutiviRicostruzione,
                                    DatoMeteoRicostruto,
                                    objParametri
                                )
                            Case enum_MeteoAlgoritmiNormalizzazione.LeggiDatoPiuVicinoSpazialeInCascata
                                rv1 = RicostruisciDatiMancanti_LeggiDatoPiuVicinoSpazialeInCascata(
                                    iteraDatoMeteoDaRicostruire,
                                    iteraConfigRicostruzione,
                                    ParametriAggiutiviRicostruzione,
                                    DatoMeteoRicostruto,
                                    objParametri
                                )

                        End Select
                        'algoritmi ricostruzione

                    End If
                    'fine verifica ricostruzione a buon fine...

                    NomeColonnaPrecedente =
                        iteraConfigRicostruzione.NomeColonna
                Next
                'prossima configurazione

            Next
            'dato meteo da ricostruire

        Catch ex As Exception


            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try


        Return rval
    End Function

    Private Function RicostruisciDatiMancanti_LeggiDatoPiuVicinoSpazialeInCascata(
        ByVal singoloDatoMeteoDaRicostruire As DateTime,
        ByVal configRicostruzione As Agronica_Stazioni_Configurazioni,
        ByVal configRicostruzioneParametri As RicostruzioneDatoParametriAggiuntivi,
        ByRef datoMeteoRicostruto As List(Of DatoSingolo),
        objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    )

        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        Try

            '1. lettura del dato per il quadrante più vicino 

            '2. lettura dato della stazione più vicina

        Catch ex As Exception


            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try


        Return rval
    End Function

    Private Function RicostruisciDatiMancanti_LeggiDatiPrecedenti(
        ByVal Stazione_cod As Integer,
        ByVal singoloDatoMeteoDaRicostruire As DateTime,
        ByVal configRicostruzione As Agronica_Stazioni_Configurazioni,
        ByVal configRicostruzioneParametri As RicostruzioneDatoParametriAggiuntivi,
        ByRef datoMeteoRicostruto As List(Of DatoSingolo),
        objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    )

        Dim rval As New RispostaStandard
        rval.RispostaOK = True
        rval.RispostaConferma = True

        Try

            Dim DataInizioRicerca As Date
            Dim DataFinericerca As Date

            DataInizioRicerca = DateAdd(
                DateInterval.Hour,
                -configRicostruzioneParametri.PeriodoValiditaDatoDaRiportareInOre,
                singoloDatoMeteoDaRicostruire
            )

            DataFinericerca = DateAdd(
                DateInterval.Hour,
                configRicostruzioneParametri.PeriodoValiditaDatoDaRiportareInOre,
                singoloDatoMeteoDaRicostruire
            )

            '1. lettura del dato non null nella stessa stazione temporalmente più vicino
            Dim leggiDati As New AgronicaCoreMeteoDAL.Agronica_Dati_Rilevati_R
            Dim dtDatiLetti As DataTable =
                leggiDati.LeggiEsistenzaDato(
                    Stazione_cod,
                    AGRODATAINIZIO,
                    configRicostruzione.NomeTabellaDati,
                    xFiltroAggiuntivo:=configRicostruzione.NomeColonna & " is not null and DataOraRilievo >= " & Agro_SQL_SaveDateTime(DataInizioRicerca) & " AND  DataOraRilievo <= " & Agro_SQL_SaveDateTime(DataFinericerca),
                    xOrderBy:=" DataOraRilievo DESC ",
                    objParametri:=objParametri
                )

            'se esite, imposto il dato ricostruito
            If dtDatiLetti.Rows.Count > 0 Then
                Dim datoRicostruito As New DatoSingolo
                datoRicostruito.valore = dtDatiLetti.Rows(0)(configRicostruzione.NomeColonna)
                datoRicostruito.DataOraRilievo = singoloDatoMeteoDaRicostruire
                datoRicostruito.DescrizioneDato = configRicostruzione

                Dim v1 As List(Of DatoSingolo) = (
                    From v In datoMeteoRicostruto
                    Where v.DataOraRilievo = singoloDatoMeteoDaRicostruire And
                        v.DescrizioneDato.NomeColonna = configRicostruzione.NomeColonna And
                        v.DescrizioneDato.Stazione_Cod = Stazione_cod
                   ).ToList

                If v1.Count = 0 Then
                    datoMeteoRicostruto.Add(datoRicostruito)
                End If

            Else
                rval.RispostaConferma = False
            End If

        Catch ex As Exception


            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try


        Return rval
    End Function

    Private Function RicostruisciDatiMancanti_LeggiDatiSerieStoriche(
        ByVal Stazione_cod As Integer,
        ByVal singoloDatoMeteoDaRicostruire As DateTime,
        ByVal configRicostruzione As Agronica_Stazioni_Configurazioni,
        ByVal configRicostruzioneParametri As RicostruzioneDatoParametriAggiuntivi,
        ByRef datoMeteoRicostruto As List(Of DatoSingolo),
        objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    )

        Dim rval As New RispostaStandard
        rval.RispostaOK = True
        rval.RispostaConferma = True

        Try


            Dim giorno As Integer = singoloDatoMeteoDaRicostruire.Day
            Dim mese As Integer = singoloDatoMeteoDaRicostruire.Month
            Dim ora As Integer = singoloDatoMeteoDaRicostruire.Hour


            Dim dataSerieStoriche As Date = #01/01/1904#.AddMonths(mese - 1).AddDays(giorno - 1).AddHours(ora)

            '1. lettura del dato non null nella stessa stazione temporalmente più vicino
            Dim leggiDati As New AgronicaCoreMeteoDAL.Agronica_Dati_Rilevati_R
            Dim dtDatiLetti As DataTable =
                leggiDati.LeggiEsistenzaDato(
                    Stazione_cod,
                    AGRODATAINIZIO,
                    configRicostruzione.NomeTabellaDati,
                    xFiltroAggiuntivo:=configRicostruzione.NomeColonna & " is not null and DataOraRilievo = " & Agro_SQL_SaveDateTime(dataSerieStoriche),
                    xOrderBy:=" DataOraRilievo DESC ",
                    objParametri:=objParametri
                )

            'se esite, imposto il dato ricostruito
            If dtDatiLetti.Rows.Count > 0 Then
                Dim datoRicostruito As New DatoSingolo
                datoRicostruito.valore = dtDatiLetti.Rows(0)(configRicostruzione.NomeColonna)
                datoRicostruito.DataOraRilievo = singoloDatoMeteoDaRicostruire
                datoRicostruito.DescrizioneDato = configRicostruzione

                Dim v1 As List(Of DatoSingolo) = (
                    From v In datoMeteoRicostruto
                    Where v.DataOraRilievo = singoloDatoMeteoDaRicostruire And
                        v.DescrizioneDato.NomeColonna = configRicostruzione.NomeColonna And
                        v.DescrizioneDato.Stazione_Cod = Stazione_cod
                   ).ToList

                If v1.Count = 0 Then
                    datoMeteoRicostruto.Add(datoRicostruito)
                End If

            Else
                rval.RispostaConferma = False
            End If

        Catch ex As Exception


            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try


        Return rval
    End Function

End Class
