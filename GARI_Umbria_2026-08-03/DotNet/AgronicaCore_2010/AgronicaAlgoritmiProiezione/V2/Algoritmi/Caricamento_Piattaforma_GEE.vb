Imports System.Reflection
Imports AgronicaAlgoritmiProiezione.V2.Model
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq

Namespace V2.Algoritmi
    Public Class Caricamento_Piattaforma_GEE
        Inherits AlgoritmoBaseGEE
        Implements IAlgoritmoProiezione

        Public Sub New(ByVal Algoritmo_Cod As Int32, ByVal TipoAlgoritmo_Cod As Int32, cfg_siti As DTOs.CF_Config)
            Me.LayerAnalysisConfig_Algorithm_Cod = Algoritmo_Cod
            Me.LayerAnalysisConfig_AlgorithmType_Cod = TipoAlgoritmo_Cod

            Me.cf_config = cfg_siti

        End Sub

        Public Sub LeggiLayerDaConfigurazioneAlgoritmo(LayerAnalysisConfig_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) Implements IAlgoritmoProiezione.LeggiLayerDaConfigurazioneAlgoritmo
            Throw New NotImplementedException()
        End Sub

        Public Sub LeggiLayerDaParametriEsecuzione(ParametriEsecuzione As String) Implements IAlgoritmoProiezione.LeggiLayerDaParametriEsecuzione
            Throw New NotImplementedException()
        End Sub

        Public Function Esegui(LayerAnalysisConfig_Cod As Integer,
                               Entita_cod_1 As Integer,
                               Entita_cod_2 As Integer,
                               Entita_cod_Risultato As Integer,
                               Esecuzione_cod As Integer,
                               Esecuzione_GUID As String,
                               ParametriEsecuzione As String,
                               ByRef objParametri_Server As AgronicaCoreParametri,
                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                               ByRef objParametri_Super_Server As AgronicaCoreParametri,
                               Optional override_transazione As Boolean = False) As JArray Implements IAlgoritmoProiezione.Esegui

            Dim ret As New JArray

            Dim xRead As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R
            Dim cfgRead As New AgronicaCoreVarieDAL.Configurazione_Siti_R


            Try
                SetupDatiGEE(LayerAnalysisConfig_Cod, Entita_cod_1, Entita_cod_2, Entita_cod_Risultato, objParametri_Server)
                Dim GUID_entita_1 = LeggiGUIDEntita(Entita_cod_1, objParametri_Server)
                Dim DT = xRead.LeggiDescrizioneElementoDaCodiceEntita(Entita_cod_1, objParametri_Server)

                If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
                    Throw New Exception("Impossibile leggere i dati geografici del poligono.")
                End If

                Dim geoData = DT.Rows(0)("GeoData").ToString

                Dim input_factory = New ParametriAlgoritmo_Factory
                Dim generatore_input = CType(input_factory.CreaParametri(LayerAnalysisConfig_AlgorithmType_Cod), Richiesta_Accodamento_GEE)
                generatore_input.SetupParametriOpzionali(LayerAnalysisConfig_GUID, Layer_1, Layer_2, Layer_Risultato)

                Dim usaCollectionNames As Boolean = False
                Dim collectionNames As JArray
                Dim responseBody As String

                If LayerAnalysisConfig_AlgorithmType_Cod = TipiEnumerativi.enum_TipoAlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE Then
                    Dim dtcfg = cfgRead.Leggi(0, CostantiPersonalizzate.Google_Earth_Engine_ConfKey, "", "", objParametri_Server)
                    If dtcfg.Rows.Count > 0 Then
                        Dim DT_Conf = JObject.Parse(dtcfg.Rows(0)("Valore").ToString)

                        collectionNames = CType(DT_Conf("CollectionNames"), JArray)

                        If (collectionNames IsNot Nothing) Then
                            For Each index In collectionNames
                                Dim newIndexRequest As New JObject
                                newIndexRequest = generatore_input.CostruisciRigaDaAccodare(Esecuzione_cod,
                                                                               geoData,
                                                                               GUID_entita_1, "", "",
                                                                               LayerAnalysisConfig_Algorithm_Cod,
                                                                               LayerAnalysisConfig_AlgorithmType_Cod,
                                                                               AttivoTuttoLayer,
                                                                               Esecuzione_GUID,
                                                                               objParametri_Server,
                                                                               objParametri_Utenti,
                                                                               objParametri_Super_Server,
                                                                               collectionName:=index.ToString)
                                'lavez - 21/05/2024 - il guid viene assegnato in fase di generazione della request, per cui deve essere propagato anche a quelle successive a parita di Esecuzione_cod
                                Dim newEsecuzioneGUID = newIndexRequest.SelectToken("geoJsonPolygon.features[0].properties.GIS_LayerAnalysisConfig_Exec_Log_GUID")

                                If Not IsNothing(newEsecuzioneGUID) Then
                                    Esecuzione_GUID = newEsecuzioneGUID.ToString
                                End If

                                ret.Add(newIndexRequest)
                            Next
                        Else
                            Throw New Exception("Elenco indici non specificato in parametrizzazione gsb. Impossibile proseguire")
                        End If


                    Else
                        Throw New Exception("Google_Earth_Engine_ConfKey non torvata. Impossibile proseguire")
                    End If
                Else
                    Dim newIndexRequest As New JObject
                    newIndexRequest = generatore_input.CostruisciRigaDaAccodare(Esecuzione_cod,
                                                                   geoData,
                                                                   GUID_entita_1, "", "",
                                                                   LayerAnalysisConfig_Algorithm_Cod,
                                                                   LayerAnalysisConfig_AlgorithmType_Cod,
                                                                   AttivoTuttoLayer,
                                                                   Esecuzione_GUID,
                                                                   objParametri_Server,
                                                                   objParametri_Utenti,
                                                                   objParametri_Super_Server)

                    ret.Add(newIndexRequest)
                End If

                'If LayerAnalysisConfig_AlgorithmType_Cod = TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync Or
                '    LayerAnalysisConfig_AlgorithmType_Cod = TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync_Yearly Then

                '    Dim responseObj = JObject.Parse(responseBody)

                '    EseguiOperazioniSincrone_Intersezione(responseObj,
                '                                          Esecuzione_cod,
                '                                          LayerAnalysisConfig_Cod,
                '                                          Entita_cod_1,
                '                                          Entita_cod_2,
                '                                          GUID_entita_1,
                '                                          objParametri_Server,
                '                                          objParametri_Utenti)
                'End If

            Catch ex As Exception
                Scrivi_LOG(objParametri_Server, "Caricamento_Piattaforma_GEE.Esegui", ex.Message)
                ret = Nothing
            End Try

            Return ret

        End Function

        Private Sub EseguiOperazioniSincrone_Intersezione(ByVal responseObj As JObject,
                                                      ByVal esecuzione_cod As Integer,
                                                      ByVal layerAnalysisConfig_Cod As Integer,
                                                      ByVal entita_cod_1 As Integer,
                                                      ByVal entita_cod_2 As Integer,
                                                      ByVal gUID_entita_1 As String,
                                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti As AgronicaCoreParametri)

            Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

            Dim resp As Boolean

            Dim messaggio As String
            Dim newIDEntita As Int32 = 0

            If responseObj("1.0") IsNot Nothing AndAlso CDbl(responseObj("1.0")) > 0 Then

                resp = xWrite.SalvaElementoGraficoIntersezioneDaRisultatoGEE(esecuzione_cod,
                                                                             layerAnalysisConfig_Cod,
                                                                             responseObj,
                                                                             newIDEntita,
                                                                             objParametri_Server,
                                                                             objParametri_Utenti)

                If Not resp Then
                    Throw New Exception("Errore nella scrittura del elemento grafico per la intersezione.")
                End If

                messaggio = String.Format("Il poligono interseca l'elemento per il {0}%.", responseObj("1.0").ToString)
            Else
                messaggio = "Il poligono non interseca l'elemento."
            End If

            If responseObj("RisultatoOperazione") IsNot Nothing Then

                Dim risultati = CType(responseObj("RisultatoOperazione"), JArray)
                Dim risultatoString As String = ""

                For Each annoObj As JObject In risultati
                    Dim annoUltimeCifre = annoObj.Properties.First.Name.ToString

                    Dim anno = ""
                    If (annoUltimeCifre.Length = 4) Then
                        anno = annoUltimeCifre
                    Else
                        anno = String.Format("{0}{1}", Date.Now.Year.ToString.Substring(0, 2), annoUltimeCifre)
                    End If

                    risultatoString = String.Format("{0}{1}: {2}, ", risultatoString, anno, annoObj(annoUltimeCifre).ToString)
                Next

                If Not risultatoString.Equals("") Then
                    'Rimuovo ultima virgola
                    messaggio = risultatoString.Substring(0, risultatoString.Length - 2)

                    resp = xWrite.SalvaElementoGraficoIntersezioneDaRisultatoGEE(esecuzione_cod,
                                                                                 layerAnalysisConfig_Cod,
                                                                                 responseObj,
                                                                                 newIDEntita,
                                                                                 objParametri_Server,
                                                                                 objParametri_Utenti,
                                                                                 messaggio)

                    If Not resp Then
                        Throw New Exception("Errore nella scrittura del elemento grafico per il calcolo della deforestazione.")
                    End If
                End If
            End If

            resp = xWrite.InserisciLogEsecuzioneAlgoritmo(layerAnalysisConfig_Cod,
                                                          esecuzione_cod,
                                                          entita_cod_1,
                                                          entita_cod_2,
                                                          newIDEntita,
                                                          1,
                                                          messaggio,
                                                          objParametri_Server,
                                                          gUID_entita_1)

            If Not resp Then
                Throw New Exception("Errore nella scrittura del log dell'operazione.")
            End If

        End Sub

    End Class
End Namespace

