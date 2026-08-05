Imports System.Net.Mime.MediaTypeNames
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Gis

Public Class Algoritmo_Intersezione
    Implements IAlgoritmoProiezione

    Private ReadOnly LayerAnalysisConfig_Algorithm_Cod As Int32
    Private ReadOnly LayerAnalysisConfig_AlgorithmType_Cod As Int32

    Private Layer_1 As ProiezioneLayer
    Private Layer_2 As ProiezioneLayer
    Private Layer_Risultato As ProiezioneLayer

    Public Sub New(ByVal Algoritmo_Cod As Int32, ByVal TipoAlgoritmo_Cod As Int32)
        Me.LayerAnalysisConfig_Algorithm_Cod = Algoritmo_Cod
        Me.LayerAnalysisConfig_AlgorithmType_Cod = TipoAlgoritmo_Cod
    End Sub

    Public Sub LeggiLayerDaConfigurazioneAlgoritmo(ByVal LayerAnalysisConfig_Cod As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri) Implements IAlgoritmoProiezione.LeggiLayerDaConfigurazioneAlgoritmo

        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R

        Dim configurazione = xRead.LeggiLayerDaConfigurazione(LayerAnalysisConfig_Cod, objParametri)

        Me.Layer_1 = configurazione.Layer1
        Me.Layer_2 = configurazione.Layer2
        Me.Layer_Risultato = configurazione.LayerRisultato

    End Sub

    Public Sub LeggiLayerDaParametriEsecuzione(ByVal ParametriEsecuzione As String) Implements IAlgoritmoProiezione.LeggiLayerDaParametriEsecuzione

    End Sub

    Public Function Esegui(ByVal LayerAnalysisConfig_Cod As Int32,
                           ByVal Entita_cod_1 As Integer,
                           ByVal Entita_cod_2 As Integer,
                           ByVal Entita_cod_Risultato As Integer,
                           ByVal Esecuzione_cod As Int32,
                           ByVal Esecuzione_GUID As String,
                           ByVal ParametriEsecuzione As String,
                           ByRef objParametri_Server As AgronicaCoreParametri,
                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                           ByRef objParametri_Super_Server As AgronicaCoreParametri,
                           ByVal Optional override_transazione As Boolean = False) As Boolean Implements IAlgoritmoProiezione.Esegui

        Dim descrizioneElementoGrafico As String
        Dim geoData As String
        Dim xRead As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R
        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

        Dim resp As Boolean
        Dim messaggio As String = ""

        Dim DT As DataTable

        Dim alg_ext As New Algorithm_Extension

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try


            LeggiLayerDaConfigurazioneAlgoritmo(LayerAnalysisConfig_Cod, objParametri_Server)

            If Layer_1.LayerElementiGrafici_Cod = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI Then
                DT = xRead.LeggiDatiMinimiDaCodiceEntita(Entita_cod_1, objParametri_Server)
            Else
                DT = xRead.LeggiDescrizioneElementoDaCodiceEntita(Entita_cod_1, objParametri_Server)
            End If

            If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
                Throw New Exception("Errore nel reperimento dell'elemento grafico.")
            End If

            descrizioneElementoGrafico = leggiDescrizioneEntita1(DT.Rows(0)("ElementoGrafico_Des").ToString,
                                                                 Layer_1.LayerElementiGrafici_Cod = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI)
            geoData = DT.Rows(0)("GeoData").ToString

            DT = xRead.calcolaIntersezioniTraLayer(Layer_2.LayerElementiGrafici_Cod,
                                                   Layer_2.Params.FirstOrDefault.TipologiaLayer_struct_cod,
                                                   geoData,
                                                   objParametri_Server)

            Dim ParametriAlgoritmoFactory As New ParametriAlgoritmo_Factory

            Dim newIDEntita As Int32

            If DT Is Nothing Then
                Throw New Exception("Si è verificato un errore nel calcolo dell'intersezione del poligono")
            End If

            'lavez - 25/07/2024 - sposto qui l'apertura della transazione per sfruttare la possibilità di attivare il "NOLOCK" nelle query
            If Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)
            End If

            If DT.Rows.Count = 0 Then
                resp = xWrite.InserisciLogEsecuzioneAlgoritmo(LayerAnalysisConfig_Cod,
                                                              Esecuzione_cod,
                                                              Entita_cod_1,
                                                              Entita_cod_2,
                                                              0,
                                                              1,
                                                              "Il poligono non interseca l'elemento.",
                                                              objParametri_Server)

                'Lavez - 05/08/2024 - gestione estensioni algoritmi di proiezione
                alg_ext.AlgorithmVector_Extension(LayerAnalysisConfig_Cod,
                                                  Esecuzione_cod,
                                                  Esecuzione_GUID,
                                                  ParametriEsecuzione,
                                                  "",
                                                  messaggio,
                                                  objParametri_Server)

            End If

            For Each row In DT.Rows
                If Not row("Intersezione").ToString.Equals("") Then

                    Dim sequenza As New Agro_Sequenze

                    newIDEntita = sequenza.NuovoId_Tabella("GIS_Entita", 0, Int32.MaxValue, objParametri_Server, True)

                    resp = inserisciEntita(newIDEntita, objParametri_Server, objParametri_Utenti)

                    If Not resp Then
                        Throw New Exception("Errore nell'inserimento dell'entità.")
                    End If

                    Dim parametriAddizionali As New Dictionary(Of String, Object)
                    parametriAddizionali.Add("Descrizione", descrizioneElementoGrafico)

                    Dim outputFactory As New ParametriAlgoritmo_Factory
                    Dim generatore_output = outputFactory.CreaParametri(Of Input_Intersezione_GEE)(Me.LayerAnalysisConfig_AlgorithmType_Cod)

                    generatore_output.SetupParametriOpzionali(row, parametriAddizionali)

                    Dim parametriRiga = generatore_output.CreaParametriElementoGrafico

                    Dim elemento_Des As String = preparaDescrizioneElementoGrafico(parametriRiga)

                    resp = inserisciElementoGrafico(row("Intersezione").ToString, newIDEntita, elemento_Des, objParametri_Server)

                    If Not resp Then
                        Throw New Exception("Errore nell'inserimento dell'elemento grafico.")
                    End If

                    messaggio = String.Format("Il poligono interseca l'elemento per il {0}%.", row("PercentualeIntersezione").ToString)
                Else
                    newIDEntita = 0
                    messaggio = "Il poligono non interseca l'elemento."
                End If

                resp = xWrite.InserisciLogEsecuzioneAlgoritmo(LayerAnalysisConfig_Cod,
                                                              Esecuzione_cod,
                                                              Entita_cod_1,
                                                              Entita_cod_2,
                                                              newIDEntita,
                                                              1,
                                                              messaggio,
                                                              objParametri_Server)

                If Not ParametriEsecuzione.Equals("") Then
                    'Lavez - 05/08/2024 - gestione estensioni algoritmi di proiezione
                    alg_ext.AlgorithmVector_Extension(LayerAnalysisConfig_Cod,
                                                  Esecuzione_cod,
                                                  Esecuzione_GUID,
                                                  ParametriEsecuzione,
                                                  If(DT.Rows.Count <= 0, "", row("Intersezione").ToString()),
                                                  messaggio,
                                                  objParametri_Server)

                End If
            Next

            If Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If

        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing And Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex
        Finally
            If Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
            End If
        End Try



        Return True
    End Function

    Private Function leggiDescrizioneEntita1(ByVal descrizioneDaDB As String, ByVal impianto As Boolean) As String
        If impianto Then Return descrizioneDaDB

        Dim partiDescrizione = descrizioneDaDB.ToString.Split("|")

        Dim elementoDescrizione = partiDescrizione.Where(Function(s) s.StartsWith(Layer_1.Params.FirstOrDefault.LayerElementiGrafici_Etichetta))

        If elementoDescrizione Is Nothing OrElse elementoDescrizione.Count <> 1 Then
            Throw New Exception("Impossibile determinare la descrizione dell'elemento grafico iniziale.")
        End If

        Return elementoDescrizione.FirstOrDefault.Replace(String.Format("{0}§ ", Layer_1.Params.FirstOrDefault.LayerElementiGrafici_Etichetta), "").Trim
    End Function

    Private Function preparaDescrizioneElementoGrafico(ByVal parametriRiga As Dictionary(Of Int32, String)) As String

        Dim partiDescrizione As New List(Of String)

        For Each tupla In parametriRiga
            Dim etichetta = Layer_Risultato.Params.Where(Function(s) s.GIS_LayerAnalysisConfig_AlgorithmType_Param_Cod = tupla.Key).FirstOrDefault.LayerElementiGrafici_Etichetta
            partiDescrizione.Add(String.Format("{0}§ {1}", etichetta, tupla.Value.ToString))
        Next

        Return String.Join("|", partiDescrizione)
    End Function

    Private Function inserisciElementoGrafico(ByVal geoDataIntersezione As String,
                                              ByVal newIDEntita As Int32,
                                              ByVal ElementoGrafico_Des As String,
                                              ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_W

        Dim newIDElementoGrafico As Int32

        Dim sequenza As New Agro_Sequenze

        newIDElementoGrafico = sequenza.NuovoId_Tabella("GIS_ElementiGrafici", 0, Int32.MaxValue, objParametri, True)

        resp = xWrite.scriviElementoGraficoBaseDaWKT(newIDElementoGrafico,
                                                     ElementoGrafico_Des,
                                                     newIDEntita,
                                                     Layer_Risultato.LayerElementiGrafici_Cod,
                                                     geoDataIntersezione,
                                                     objParametri)

        Return resp

    End Function

    Private Function inserisciEntita(ByVal newIDEntita As Int32,
                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean
        Dim xWrite As New AgronicaCoreGisBIZ.GIS_Entita_W

        resp = xWrite.scriviEntitaBase(newIDEntita,
                                       CInt(TipiEnumerativi.enum_GIS2012_TipoEntita.DATI_IMPORTATI),
                                       objParametri_Server, objParametri_Utenti)

        Return resp

    End Function
End Class
