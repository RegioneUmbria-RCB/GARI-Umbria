Imports Agronica.Helpers.AlgoritmiProiezione
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreModelsSTD.Gis
Imports Newtonsoft.Json.Linq

Public Class ProiezioniLayer_R
    Public Function LeggiElencoConfigurazioni(ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Utente As AgronicaCoreParametri,
                                              ByVal Optional isAttivo As Boolean = False,
                                              ByVal Optional cfgFilter As String = "") As ElencoConfigurazioniProiezione

        Dim elencoConfigurazioni As New ElencoConfigurazioniProiezione With {
            .elencoConfigurazioniProiezione = New List(Of ConfigurazioneProiezione)
        }

        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R
        Dim xReadGruppi As New AgronicaCoreUtentiDAL.Gruppi_Utente_R

        Dim gruppi_appartenenza As New List(Of Int32)
        Dim DT As DataTable

        DT = xReadGruppi.LeggiGruppiDaUtente(objParametri_Utente)

        For Each row In DT.Rows
            gruppi_appartenenza.Add(CInt(row("Gruppi_Utente_cod")))
        Next

        DT = xRead.LeggiElencoConfigurazioni(cfgFilter, isAttivo, gruppi_appartenenza, objParametri_Server)

        If DT Is Nothing Then
            Throw New Exception("Errore nella lettura delle configurazioni di proiezione.")
        End If

        Dim ConfigurazioneCorrente As Int32 = 0
        Dim configurazione As ConfigurazioneProiezione = Nothing

        For Each row In DT.Rows
            If CInt(row("LayerAnalysisConfig_Cod")) <> ConfigurazioneCorrente Then

                If ConfigurazioneCorrente <> 0 Then
                    elencoConfigurazioni.elencoConfigurazioniProiezione.Add(configurazione)
                End If

                ConfigurazioneCorrente = CInt(row("LayerAnalysisConfig_Cod"))

                configurazione = New ConfigurazioneProiezione With {
                    .ConfigurazioneProiezione_Cod = ConfigurazioneCorrente,
                    .ConfigurazioneProiezione_Des = row("LayerAnalysisConfig_Des").ToString,
                    .AlgoritmoProiezione_Cod = CInt(row("LayerAnalysisConfig_Algorithm_Cod")),
                    .AttivoTuttiLayer = CBool(row("AttivaSuTuttiLayer")),
                    .canActivate = CBool(row("CanActivate")),
                    .canEditCfg = CBool(row("CanEditCfg")),
                    .cfg = row("LayerAnalysisConfig_Cfg"),
                    .ConfigurazioneProiezione_GUID = row("LayerAnalysisConfig_GUID").ToString,
                    .Layer1 = New ProiezioneLayer With {.Params = New List(Of ParametriProiezioneLayer)},
                    .Layer2 = New ProiezioneLayer With {.Params = New List(Of ParametriProiezioneLayer)},
                    .LayerRisultato = New ProiezioneLayer With {.Params = New List(Of ParametriProiezioneLayer)}
                }

            End If

            Dim layerParam As New ParametriProiezioneLayer With {
                            .Parametro_Cod = CInt(row("LayerAnalysisConfig_Algorithm_Param_Cod")),
                            .TipologiaLayer_struct_cod = CInt(row("TipologiaLayer_Struct_Cod")),
                            .TipologiaLayer_struct_GUID = row("TipologiaLayer_struct_GUID").ToString
                        }

            Select Case CType(CInt(row("LayerXConfig_Cod")), enum_TipoRigaLayer)
                Case enum_TipoRigaLayer.LAYER1
                    configurazione.Layer1.LayerElementiGrafici_Cod = CInt(row("LayerElementiGrafici_Cod"))
                    configurazione.Layer1.TipologiaLayer_cod = CInt(row("TipologiaLayer_Cod"))
                    configurazione.Layer1.LayerElementiGrafici_GUID = row("LayerElementiGrafici_GUID").ToString

                    configurazione.Layer1.Params.Add(layerParam)
                Case enum_TipoRigaLayer.LAYER2
                    configurazione.Layer2.LayerElementiGrafici_Cod = CInt(row("LayerElementiGrafici_Cod"))
                    configurazione.Layer2.TipologiaLayer_cod = CInt(row("TipologiaLayer_Cod"))
                    configurazione.Layer2.LayerElementiGrafici_GUID = row("LayerElementiGrafici_GUID").ToString

                    configurazione.Layer2.Params.Add(layerParam)
                Case enum_TipoRigaLayer.LAYERRISULTATO
                    configurazione.LayerRisultato.LayerElementiGrafici_Cod = CInt(row("LayerElementiGrafici_Cod"))
                    configurazione.LayerRisultato.TipologiaLayer_cod = CInt(row("TipologiaLayer_Cod"))
                    configurazione.LayerRisultato.LayerElementiGrafici_GUID = row("LayerElementiGrafici_GUID").ToString

                    configurazione.LayerRisultato.Params.Add(layerParam)
            End Select
        Next

        If configurazione IsNot Nothing Then
            elencoConfigurazioni.elencoConfigurazioniProiezione.Add(configurazione)
        End If

        Return elencoConfigurazioni
    End Function

    Public Function LeggiListaEsecuzioniAlgoritmi(ByRef objParametri_Server As AgronicaCoreParametri,
                                                  Optional ByVal noConf As Boolean = False,
                                                  Optional ByVal ordinaPerAlgoritmo As Boolean = False,
                                                  Optional ByVal Batchsize As Integer = 0,
                                                  Optional ByVal TagName As String = "") As DataTable
        Dim DT As DataTable

        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R

        If Batchsize > 0 AndAlso TagName <> "" Then
            Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W

            xWrite.AssociaRichiesteEsecuzioneATagname(Batchsize, TagName, noConf, objParametri_Server)
        End If

        DT = xRead.LeggiListaEsecuzioniAlgoritmi(objParametri_Server, noConf, ordinaPerAlgoritmo, TagName)

        Return DT
    End Function

    Public Function LeggiLayerDaConfigurazione(ByVal LayerAnalysisConfig_Cod As Int32, ByRef objParametri As AgronicaCoreParametri) As ConfigurazioneProiezione

        Dim DT As DataTable

        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R

        DT = xRead.LeggiLayerDaConfigurazione(LayerAnalysisConfig_Cod, objParametri)

        If DT Is Nothing Then
            Throw New Exception("Errore nella lettura delle configurazioni di proiezione.")
        End If

        Dim ConfigurazioneCorrente As Int32 = 0
        Dim configurazione As ConfigurazioneProiezione = Nothing

        For Each row In DT.Rows
            If CInt(row("LayerAnalysisConfig_Cod")) <> ConfigurazioneCorrente Then

                ConfigurazioneCorrente = CInt(row("LayerAnalysisConfig_Cod"))

                configurazione = New ConfigurazioneProiezione With {
                    .ConfigurazioneProiezione_Cod = ConfigurazioneCorrente,
                    .ConfigurazioneProiezione_GUID = row("LayerAnalysisConfig_GUID").ToString,
                    .ConfigurazioneProiezione_Des = row("LayerAnalysisConfig_Des").ToString,
                    .AlgoritmoProiezione_Cod = CInt(row("LayerAnalysisConfig_Algorithm_Cod")),
                    .AttivoTuttiLayer = CBool(row("AttivaSuTuttiLayer")),
                    .canActivate = CBool(row("CanActivate")),
                    .canEditCfg = CBool(row("CanEditCfg")),
                    .cfg = row("LayerAnalysisConfig_Cfg"),
                    .Layer1 = New ProiezioneLayer With {.Params = New List(Of ParametriProiezioneLayer)},
                    .Layer2 = New ProiezioneLayer With {.Params = New List(Of ParametriProiezioneLayer)},
                    .LayerRisultato = New ProiezioneLayer With {.Params = New List(Of ParametriProiezioneLayer)}
                }

            End If

            Dim layerParam As New ParametriProiezioneLayer With {
                            .Parametro_Cod = CInt(row("LayerAnalysisConfig_Algorithm_Param_Cod")),
                            .TipologiaLayer_struct_cod = CInt(row("TipologiaLayer_Struct_Cod")),
                            .TipologiaLayer_struct_GUID = row("TipologiaLayer_Struct_GUID").ToString,
                            .LayerElementiGrafici_Etichetta = row("LayerElementiGrafici_Etichetta").ToString,
                            .GIS_LayerAnalysisConfig_AlgorithmType_Param_Cod = CInt(row("GIS_LayerAnalysisConfig_AlgorithmType_Param_Cod"))
                        }

            Select Case CType(CInt(row("LayerXConfig_Cod")), enum_TipoRigaLayer)
                Case enum_TipoRigaLayer.LAYER1
                    configurazione.Layer1.LayerElementiGrafici_Cod = CInt(row("LayerElementiGrafici_Cod"))
                    configurazione.Layer1.LayerElementiGrafici_GUID = row("LayerElementiGrafici_GUID").ToString
                    configurazione.Layer1.TipologiaLayer_cod = CInt(row("TipologiaLayer_Cod"))

                    configurazione.Layer1.Params.Add(layerParam)
                Case enum_TipoRigaLayer.LAYER2
                    configurazione.Layer2.LayerElementiGrafici_Cod = CInt(row("LayerElementiGrafici_Cod"))
                    configurazione.Layer2.LayerElementiGrafici_GUID = row("LayerElementiGrafici_GUID").ToString
                    configurazione.Layer2.TipologiaLayer_cod = CInt(row("TipologiaLayer_Cod"))

                    configurazione.Layer2.Params.Add(layerParam)
                Case enum_TipoRigaLayer.LAYERRISULTATO
                    configurazione.LayerRisultato.LayerElementiGrafici_Cod = CInt(row("LayerElementiGrafici_Cod"))
                    configurazione.LayerRisultato.LayerElementiGrafici_GUID = row("LayerElementiGrafici_GUID").ToString
                    configurazione.LayerRisultato.TipologiaLayer_cod = CInt(row("TipologiaLayer_Cod"))

                    configurazione.LayerRisultato.Params.Add(layerParam)
            End Select
        Next

        Return configurazione

    End Function

    Public Function LeggiElencoConfigurazioniSincro(ByRef objParametri_Server As AgronicaCoreParametri) As ElencoConfigurazioniProiezione

        Dim elencoConfigurazioni As New ElencoConfigurazioniProiezione With {
            .elencoConfigurazioniProiezione = New List(Of ConfigurazioneProiezione)
        }

        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R
        Dim DT As DataTable

        DT = xRead.LeggiElencoConfigurazioniSincro(objParametri_Server)

        If DT Is Nothing Then
            Throw New Exception("Errore nella lettura delle configurazioni di proiezione.")
        End If

        Dim ConfigurazioneCorrente As Int32 = 0
        Dim configurazione As ConfigurazioneProiezione = Nothing

        For Each row In DT.Rows
            If CInt(row("LayerAnalysisConfig_Cod")) <> ConfigurazioneCorrente Then

                If ConfigurazioneCorrente <> 0 Then
                    elencoConfigurazioni.elencoConfigurazioniProiezione.Add(configurazione)
                End If

                ConfigurazioneCorrente = CInt(row("LayerAnalysisConfig_Cod"))

                configurazione = New ConfigurazioneProiezione With {
                    .ConfigurazioneProiezione_Cod = ConfigurazioneCorrente,
                    .ConfigurazioneProiezione_Des = row("LayerAnalysisConfig_Des").ToString,
                    .AlgoritmoProiezione_Cod = CInt(row("LayerAnalysisConfig_Algorithm_Cod")),
                    .AttivoTuttiLayer = CBool(row("AttivaSuTuttiLayer")),
                    .Layer1 = New ProiezioneLayer With {.Params = New List(Of ParametriProiezioneLayer)},
                    .Layer2 = New ProiezioneLayer With {.Params = New List(Of ParametriProiezioneLayer)},
                    .LayerRisultato = New ProiezioneLayer With {.Params = New List(Of ParametriProiezioneLayer)}
                }

            End If

            Dim layerParam As New ParametriProiezioneLayer With {
                            .Parametro_Cod = CInt(row("LayerAnalysisConfig_Algorithm_Param_Cod")),
                            .TipologiaLayer_struct_cod = CInt(row("TipologiaLayer_Struct_Cod"))
                        }

            Select Case CType(CInt(row("LayerXConfig_Cod")), enum_TipoRigaLayer)
                Case enum_TipoRigaLayer.LAYER1
                    configurazione.Layer1.LayerElementiGrafici_Cod = CInt(row("LayerElementiGrafici_Cod"))
                    configurazione.Layer1.TipologiaLayer_cod = CInt(row("TipologiaLayer_Cod"))

                    configurazione.Layer1.Params.Add(layerParam)
                Case enum_TipoRigaLayer.LAYER2
                    configurazione.Layer2.LayerElementiGrafici_Cod = CInt(row("LayerElementiGrafici_Cod"))
                    configurazione.Layer2.TipologiaLayer_cod = CInt(row("TipologiaLayer_Cod"))

                    configurazione.Layer2.Params.Add(layerParam)
                Case enum_TipoRigaLayer.LAYERRISULTATO
                    configurazione.LayerRisultato.LayerElementiGrafici_Cod = CInt(row("LayerElementiGrafici_Cod"))
                    configurazione.LayerRisultato.TipologiaLayer_cod = CInt(row("TipologiaLayer_Cod"))

                    configurazione.LayerRisultato.Params.Add(layerParam)
            End Select
        Next

        If configurazione IsNot Nothing Then
            elencoConfigurazioni.elencoConfigurazioniProiezione.Add(configurazione)
        End If

        Return elencoConfigurazioni
    End Function


    Public Function LeggiElencoConfigurazioniSuLayer(ByVal layer_cod As Int32,
                                                     ByVal tipologia_layer_cod As Int32,
                                                     ByVal Entita_Cod As Int32,
                                                     ByRef objParametri_Server As AgronicaCoreParametri
                                                     ) As ElencoConfigurazioniSuLayer

        Dim elencoConfigurazioni As New ElencoConfigurazioniSuLayer With {
            .elencoConfigurazioni = New List(Of ConfigurazioneSuLayer)
        }

        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R
        Dim DT As DataTable

        DT = xRead.LeggiElencoConfigurazioniSuLayer(layer_cod, tipologia_layer_cod, Entita_Cod, objParametri_Server)

        If DT Is Nothing Then
            Throw New Exception("Errore nella lettura delle configurazioni di proiezione.")
        End If

        Dim ConfigurazioneCorrente As Int32 = 0
        Dim configurazione As ConfigurazioneSuLayer = Nothing

        For Each row In DT.Rows
            If CInt(row("LayerAnalysisConfig_Cod")) <> ConfigurazioneCorrente Then

                If ConfigurazioneCorrente <> 0 Then
                    configurazione.NumEntitaAttive = configurazione.EntitaAttive.Count
                    elencoConfigurazioni.elencoConfigurazioni.Add(configurazione)
                End If

                ConfigurazioneCorrente = CInt(row("LayerAnalysisConfig_Cod"))

                configurazione = New ConfigurazioneSuLayer With {
                    .LayerAnalysisConfig_Cod = CInt(row("LayerAnalysisConfig_Cod")),
                    .LayerAnalysisConfig_Des = row("LayerAnalysisConfig_Des").ToString,
                    .LayerAnalysisConfig_Algorithm_Cod = CInt(row("LayerAnalysisConfig_Algorithm_Cod")),
                    .LayerAnalysisConfig_Algorithm_Des = row("LayerAnalysisConfig_Algorithm_Des").ToString,
                    .AttivaSuTuttiLayer = CBool(row("AttivaSuTuttiLayer")),
                    .NumEntitaMax = CInt(row("NumEntitaMax")),
                    .EntitaAttive = New List(Of ConfigurazioneSuEntitaAttiva)
                }

            End If

            If (CInt(row("Entita_Cod")) <> 0) Then
                Dim configurazioneAttiva As New ConfigurazioneSuEntitaAttiva With {
                    .Entita_Cod = CInt(row("Entita_Cod")),
                    .AttivaDa = CDate(row("Validita_Inizio")),
                    .AttivaA = CDate(row("Validita_Fine")),
                    .Sospesa = False 'TODO: leggere da tabella
                }

                configurazione.EntitaAttive.Add(configurazioneAttiva)
            End If
        Next

        If configurazione IsNot Nothing Then
            configurazione.NumEntitaAttive = configurazione.EntitaAttive.Count
            elencoConfigurazioni.elencoConfigurazioni.Add(configurazione)
        End If

        Return elencoConfigurazioni
    End Function

    Public Function LeggiLogEsecuzioni(ByVal Entita_Cod As Integer,
                                       ByRef objParametri_Server As AgronicaCoreParametri) As ElencoLogEsecuzioniConfigurazioniProiezione

        Dim resp As New ElencoLogEsecuzioniConfigurazioniProiezione With
            {.elencoLogEsecuzioniConfigurazione = New List(Of LogEsecuzioniConfigurazioniProiezione)}

        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R
        Dim logDT As New DataTable

        logDT = xRead.LeggiLogEsecuzioni(Entita_Cod, objParametri_Server)

        For Each row In logDT.Rows
            'Scarto le righe in cui il LOG non ha errori e non c'è ancora nemmeno un DETAILS
            If IsDBNull(row("GIS_LayerAnalysisConfig_Exec_Log_Details_Cod")) AndAlso row("Messaggi").ToString.Equals("") Then
                Continue For
            End If

            Dim esecuzione As New LogEsecuzioniConfigurazioniProiezione With {
                .Entita_cod_1 = CInt(row("Entita_cod_1")),
                .Entita_cod_2 = CInt(row("Entita_cod_2")),
                .Entita_cod_Risultato = CInt(row("Entita_cod_Risultato")),
                .GIS_LayerAnalysisConfig_Exec_Log_Details_Cod = CInt(row("GIS_LayerAnalysisConfig_Exec_Log_Details_Cod")),
                .LayerAnalysisConfig_Cod = CInt(row("LayerAnalysisConfig_Cod")),
                .LayerAnalysisConfig_Des = row("LayerAnalysisConfig_Des").ToString,
                .StatoElaborazione = CInt(row("StatoElaborazione")),
                .Data_Creazione = CDate(row("Data_Creazione")),
                .Messaggi = New List(Of String)
            }

            esecuzione.Messaggi.AddRange(row("Messaggi").ToString.Split(","))

            resp.elencoLogEsecuzioniConfigurazione.Add(esecuzione)
        Next

        If logDT IsNot Nothing And logDT.Rows.Count > 0 Then
            resp.elencoLogEsecuzioniConfigurazione = resp.elencoLogEsecuzioniConfigurazione.OrderByDescending(Of DateTime)(Function(log) log.Data_Creazione).ToList
        End If

        Return resp
    End Function

    Public Function VerificaEsistenzaEsecuzione(ByVal esecuzione_guid As String,
                                                ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R

        Dim DT = xRead.LeggiEsecuzioneDaGUID(esecuzione_guid, objParametri)

        Return (DT IsNot Nothing AndAlso DT.Rows.Count = 1)
    End Function

    Public Function LeggiEsecuzioniAnalisiMappeSatellitari(ByVal poligonoWKT As String,
                                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                                           Optional ByVal Entita_Cod As Integer = 0
                                                           ) As Lista_GisSat_SentinelOverlay_Out

        Dim resp As New Lista_GisSat_SentinelOverlay_Out With {
            .ListaOverlayer = New List(Of Gis_Sat_Sentinel_Overlay)
        }

        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R

        Dim DTEsecuzioni As DataTable = xRead.LeggiEsecuzioniAnalisiMappeSatellitari(poligonoWKT, objParametri_Server, Entita_Cod)

        For Each row In DTEsecuzioni.Rows
            Dim objRisultato = JObject.Parse(row("Risultato_Json").ToString)

            Dim listaEntita = CType(objRisultato.SelectToken("geoJsonPolygon.features[0].properties.LayerAnalysisConfig_Response[0].RisultatoElaborazione"), JArray)

            For Each entita In listaEntita
                Dim overlay = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Gis_Sat_Sentinel_Overlay)(entita.ToString)
                resp.ListaOverlayer.Add(overlay)
            Next
        Next

        Return resp
    End Function

    Public Function LeggiEsecuzioneDaGUID(ByVal GIS_LayerAnalysisConfig_Exec_Log_Cod_GUID As String,
                                          ByRef objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim DT As DataTable
        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R

        DT = xRead.LeggiEsecuzioneDaGUID(GIS_LayerAnalysisConfig_Exec_Log_Cod_GUID, objParametri_Server)

        If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
            Throw New Exception("Errore nella lettura dell'esecuzione algoritmo.")
        End If

        Return DT
    End Function

    Public Function LeggiConfigurazioneDaCodice(ByVal configurazione_cod As Integer,
                                                ByRef objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim DT As DataTable
        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R

        DT = xRead.LeggiConfigurazioneDaCodice(configurazione_cod, objParametri_Server)

        If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
            Throw New Exception("Errore nella lettura dell'esecuzione algoritmo.")
        End If

        Return DT
    End Function

    Public Function VerificaEntitaRisultatoGEE(ByVal risultato As JArray, ByRef objParametri_Server As AgronicaCoreParametri)
        Dim xReadEnt As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim Entita As DataTable = Nothing

        For Each rec In risultato
            If rec("Entita_GUID_1") IsNot Nothing AndAlso Not rec("Entita_GUID_1").ToString.Equals("") Then
                Entita = Nothing
                Entita = xReadEnt.LeggiEntitaDaGUID(rec("Entita_GUID_1").ToString, objParametri_Server)
                If Entita Is Nothing OrElse Entita.Rows.Count = 0 Then
                    Return False
                End If
            End If
            If rec("Entita_GUID_2") IsNot Nothing AndAlso Not rec("Entita_GUID_2").ToString.Equals("") Then
                Entita = Nothing
                Entita = xReadEnt.LeggiEntitaDaGUID(rec("Entita_GUID_2").ToString, objParametri_Server)
                If Entita Is Nothing OrElse Entita.Rows.Count = 0 Then
                    Return False
                End If
            End If
            If rec("Entita_GUID_Risultato") IsNot Nothing AndAlso Not rec("Entita_GUID_Risultato").ToString.Equals("") Then
                Entita = Nothing
                Entita = xReadEnt.LeggiEntitaDaGUID(rec("Entita_GUID_Risultato").ToString, objParametri_Server)
                If Entita Is Nothing OrElse Entita.Rows.Count = 0 Then
                    Return False
                End If
            End If
        Next

        Return True
    End Function
End Class
Public Class ProiezioniLayer_W
    Public Function SalvaConfigurazioneProiezione(ByVal algoritmoProiezione_Cod As Int32,
                                                  ByVal configurazioneProiezione_Des As String,
                                                  ByVal configurazioneProiezione_Guid As String,
                                                  ByVal layer1 As ProiezioneLayer,
                                                  ByVal layer2 As ProiezioneLayer,
                                                  ByVal layerRisultato As ProiezioneLayer,
                                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                  ByVal Optional IdPerModifica As Int32 = 0
                                                  ) As String

        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W
        Dim sequenze As New Agro_Sequenze

        Dim resp As Boolean

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            If IdPerModifica = 0 Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri_Server)
            End If

            Dim newConfId As Int32

            If IdPerModifica <> 0 Then
                newConfId = IdPerModifica
            Else
                resp = VerificaEsistenzaConfigurazione(algoritmoProiezione_Cod, layer1, layer2, layerRisultato, objParametri_Server)

                If resp Then
                    If Not objParametri_Server.objTransazione Is Nothing And IdPerModifica = 0 Then
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                    End If

                    Return "CONF_ESISTENTE"
                End If

                newConfId = sequenze.NuovoId_Tabella("GIS_LayerAnalysisConfig", 0, Int32.MaxValue, objParametri_Server, True)
            End If


            resp = xWrite.SalvaConfigurazioneProiezione(algoritmoProiezione_Cod,
                                                        configurazioneProiezione_Des,
                                                        configurazioneProiezione_Guid,
                                                        newConfId,
                                                        objParametri_Server)

            If Not resp Then
                Throw New Exception("Errore nel salvataggio della configurazione di proiezione.")
            End If

            If layer1 IsNot Nothing AndAlso layer1.LayerElementiGrafici_Cod <> 0 Then

                VerificaParametri(layer1)

                resp = SalvaParametriConfigurazione(xWrite, layer1, CInt(TipiEnumerativi.enum_TipoRigaLayer.LAYER1), newConfId, objParametri_Server)

                If Not resp Then
                    Throw New Exception("Errore nel salvataggio dei parametri del layer 1 della configurazione di proiezione.")
                End If

            End If

            If layer2 IsNot Nothing AndAlso layer2.LayerElementiGrafici_Cod <> 0 Then

                VerificaParametri(layer2)

                resp = SalvaParametriConfigurazione(xWrite, layer2, CInt(TipiEnumerativi.enum_TipoRigaLayer.LAYER2), newConfId, objParametri_Server)

                If Not resp Then
                    Throw New Exception("Errore nel salvataggio dei parametri del layer 2 della configurazione di proiezione.")
                End If

            End If

            If layerRisultato IsNot Nothing AndAlso layerRisultato.LayerElementiGrafici_Cod <> 0 Then

                VerificaParametri(layerRisultato)

                resp = SalvaParametriConfigurazione(xWrite, layerRisultato, CInt(TipiEnumerativi.enum_TipoRigaLayer.LAYERRISULTATO), newConfId, objParametri_Server)

                If Not resp Then
                    Throw New Exception("Errore nel salvataggio dei parametri del layer risultato della configurazione di proiezione.")
                End If

            End If
            Dim xWritePermessi As New AgronicaCoreGisDAL.GIS_Permessi_Configurazione_W

            resp = xWritePermessi.UpdatePermesso(1, 1, 1, 1, 1, 1, newConfId,
                                                 objParametri_Utenti.UtenteUsername,
                                                 0, objParametri_Server)

            If Not resp Then
                Throw New Exception("Errore nel salvataggio dei permessi sulla configurazione di proiezione.")
            End If

            If IdPerModifica = 0 Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If

        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing And IdPerModifica = 0 Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If
            Throw ex
        Finally
            If IdPerModifica = 0 Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
            End If
        End Try

        Return "OK"
    End Function

    Private Sub VerificaParametri(ByRef layer As ProiezioneLayer)
        If layer.Params Is Nothing OrElse layer.Params.Count = 0 Then

            layer.Params = New List(Of ParametriProiezioneLayer)

            Dim emptyParam As New ParametriProiezioneLayer With {
                .Parametro_Cod = 0,
                .TipologiaLayer_struct_cod = 0
            }

            layer.Params.Add(emptyParam)

        End If
    End Sub

    Public Function ModificaConfigurazioneProiezione(ByVal configurazioneProiezione_Cod As Int32,
                                                     ByVal algoritmoProiezione_Cod As Int32,
                                                     ByVal configurazioneProiezione_Des As String,
                                                     ByVal configurazioneProiezione_Guid As String,
                                                     ByVal layer1 As ProiezioneLayer,
                                                     ByVal layer2 As ProiezioneLayer,
                                                     ByVal layerRisultato As ProiezioneLayer,
                                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                                     ByRef objParametri_Utenti As AgronicaCoreParametri
                                                     ) As String

        Dim resp As Boolean
        Dim respString As String

        resp = VerificaEsistenzaConfigurazione(algoritmoProiezione_Cod, Nothing, Nothing, Nothing, objParametri_Server)

        If Not resp Then
            Return "CONF_INESISTENTE"
        End If

        resp = VerificaAttivazioneConfigurazione(configurazioneProiezione_Cod, objParametri_Server)

        If Not resp Then
            Return "CONF_INUSO"
        End If

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            resp = EliminaConfigurazione(configurazioneProiezione_Cod, objParametri_Server)

            If Not resp Then
                Throw New Exception("Errore nell'aggiornamento della configurazione di proiezione.")
            End If

            respString = SalvaConfigurazioneProiezione(
                algoritmoProiezione_Cod,
                configurazioneProiezione_Des,
                configurazioneProiezione_Guid,
                layer1,
                layer2,
                layerRisultato,
                objParametri_Server,
                objParametri_Utenti,
                configurazioneProiezione_Cod
                )

            If Not respString.Equals("OK") Then
                Throw New Exception("Errore nell'aggiornamento della configurazione di proiezione.")
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If
            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return "OK"
    End Function

    Public Function ActivateAlgoritm(ByVal configurazioneProiezione_Cod As Int32,
                                     ByVal algoritmoProiezione_Cod As Int32,
                                     ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim resp As Boolean

        resp = VerificaEsistenzaConfigurazione(algoritmoProiezione_Cod, Nothing, Nothing, Nothing, objParametri_Server)

        If Not resp Then
            Return "CONF_INESISTENTE"
        End If

        resp = IsConfigurationAlreadyActive(configurazioneProiezione_Cod, objParametri_Server)

        If resp Then
            Return "CONF_INUSO"
        End If

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            resp = Me.InserisciAttivazioneEsecuzione(
                configurazioneProiezione_Cod,
                0,
                0,
                0,
                objParametri_Server
                )

            If Not resp Then
                Throw New Exception("Error during the attivation of algorithm.")
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If
            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return "OK"
    End Function

    Public Function SaveAlgorithmConfigurationCfg(ByVal configurazioneProiezione_Cod As Int32,
                                                  ByVal algoritmoProiezione_Cod As Int32,
                                                  ByVal cfg As String,
                                                  ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim resp As Boolean

        resp = VerificaEsistenzaConfigurazione(algoritmoProiezione_Cod, Nothing, Nothing, Nothing, objParametri_Server)

        If Not resp Then
            Return "CONF_INESISTENTE"
        End If

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W
            resp = xWrite.SaveAlgorithmConfigurationCfg(configurazioneProiezione_Cod, cfg, objParametri_Server)

            If Not resp Then
                Throw New Exception("Error")
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If
            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return "OK"
    End Function

    Public Function AttivaDisattivaConfigurazioneSuLayer(ByVal configurazioneProiezione_Cod As Integer,
                                                         ByVal layer_cod As Int32,
                                                         ByVal tipologia_layer_cod As Int32,
                                                         ByVal isAttivo As Boolean,
                                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                                         Optional ByVal groupId As Integer = 0) As Boolean

        Dim xRead As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W

        Dim DT As DataTable

        Dim Risp As Boolean

        Dim idRequest As Integer = -1 'Lavez - 07/11/2024 - garbage per riunificazione metodo AttivaDisattivaConfigurazioneSuEntita

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Risp = xWrite.SetFlagAttivazioneInteroLayer(configurazioneProiezione_Cod, isAttivo, objParametri_Server)

            If Not Risp Then
                Throw New Exception("Errore nell'attivazione della configurazione sull'intero layer.")
            End If

            DT = xRead.LeggiElencoEntitaDaLayer(layer_cod, objParametri_Server)

            For Each row In DT.Rows
                Risp = AttivaDisattivaConfigurazioneSuEntita(configurazioneProiezione_Cod,
                                                             CInt(row("Entita_Cod")),
                                                             0,
                                                             0,
                                                             isAttivo,
                                                             idRequest,
                                                             objParametri_Server,
                                                             False,
                                                             groupId:=groupId)

                If Not Risp Then
                    Throw New Exception("Errore nell'attivazione della configurazione sull'intero layer.")
                End If
            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If
            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return Risp

    End Function

    Public Function AttivaDisattivaConfigurazioneSuEntita(ByVal configurazioneProiezione_Cod As Int32,
                                                          ByVal entita_cod_1 As Int32,
                                                          ByVal entita_cod_2 As Int32,
                                                          ByVal entita_cod_risultato As Int32,
                                                          ByVal isAttivo As Boolean,
                                                          ByRef idRequest As Integer,
                                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                                          ByVal Optional apriTransazione As Boolean = True,
                                                          ByVal Optional riattivaEsecuzione As Boolean = True,
                                                          ByVal Optional parametriElaborazione As String = "",
                                                          ByVal Optional groupId As Integer = 0) As Boolean



        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W
        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R
        Dim sequenze As New Agro_Sequenze

        Dim resp As Boolean
        Dim codiceEsecuzione As Int32 = 0
        idRequest = -1

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            If apriTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri_Server)
            End If

            codiceEsecuzione = xRead.VerificaEsistenzaEsecuzione(configurazioneProiezione_Cod,
                                                                 entita_cod_1,
                                                                 entita_cod_2,
                                                                 entita_cod_risultato,
                                                                 objParametri_Server)

            If codiceEsecuzione <> 0 And riattivaEsecuzione Then
                resp = xWrite.SetFlagAttivazioneEsecuzione(codiceEsecuzione, isAttivo, objParametri_Server)
            Else
                idRequest = sequenze.NuovoId_Tabella("GIS_LayerAnalysisConfig_Exec_Log",
                                                               0, Int32.MaxValue, objParametri_Server, True)

                If idRequest < 0 Then
                    Throw New Exception("Errore in recupero contatore su GIS_LayerAnalysisConfig_Exec_Log")
                End If

                resp = xWrite.InserisciAttivazioneEsecuzione(idRequest,
                                                             configurazioneProiezione_Cod,
                                                             entita_cod_1,
                                                             entita_cod_2,
                                                             entita_cod_risultato,
                                                             objParametri_Server,
                                                             parametriElaborazione,
                                                             groupId)

                If Not resp Then
                    Throw New Exception("Errore nell'attivazione dell'esecuzione..")
                End If
            End If

            If apriTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If

        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing And apriTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If
            Throw ex
        Finally
            If apriTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
            End If
        End Try

        Return resp
    End Function

    Private Function VerificaAttivazioneConfigurazione(ByVal configurazioneProiezione_Cod As Integer,
                                                       ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R

        Return xRead.VerificaAttivazioneEsecuzione(configurazioneProiezione_Cod, objParametri_Server)

    End Function

    Private Function IsConfigurationAlreadyActive(ByVal configurazioneProiezione_Cod As Integer,
                                                  ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R

        Return xRead.IsConfigurationAlreadyActive(configurazioneProiezione_Cod, objParametri_Server)

    End Function

    Private Function VerificaEsistenzaConfigurazione(ByVal algoritmoProiezione_Cod As Int32,
                                                     ByVal layer1 As ProiezioneLayer,
                                                     ByVal layer2 As ProiezioneLayer,
                                                     ByVal layerRisultato As ProiezioneLayer,
                                                     ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R
        Dim layerParams As New List(Of String)

        If layer1 IsNot Nothing Then
            If layer1.Params IsNot Nothing Then
                For Each param In layer1.Params
                    layerParams.Add(AggiungiCondizioneExists(layer1, param))
                Next
            End If
        End If

        If layer2 IsNot Nothing Then
            If layer2.Params IsNot Nothing Then
                For Each param In layer2.Params
                    layerParams.Add(AggiungiCondizioneExists(layer2, param))
                Next
            End If
        End If

        If layerRisultato IsNot Nothing Then
            If layerRisultato.Params IsNot Nothing Then
                For Each param In layerRisultato.Params
                    layerParams.Add(AggiungiCondizioneExists(layerRisultato, param))
                Next
            End If
        End If

        resp = xRead.VerificaEsistenzaConfigurazione(algoritmoProiezione_Cod,
                                                     layerParams,
                                                     objParametri_Server)

        Return resp

    End Function

    Private Function AggiungiCondizioneExists(ByVal layer As ProiezioneLayer, ByVal param As ParametriProiezioneLayer) As String
        Return String.Format(" AND EXISTS (SELECT 1 FROM dbo.GIS_LayerAnalysisConfig_DataStruct
					                                WHERE LayerAnalysisConfig_Cod = LAC.LayerAnalysisConfig_Cod
					                                AND LayerXConfig_Cod = {0}
                                                    AND LayerElementiGrafici_Cod = {1}
                                                    AND TipologiaLayer_Cod = {2}
                                                    AND LayerAnalysisConfig_Algorithm_Param_Cod = {3} 
                                                    AND TipologiaLayer_Struct_Cod = {4}) ",
                            CInt(TipiEnumerativi.enum_TipoRigaLayer.LAYER1).ToString,
                            layer.LayerElementiGrafici_Cod.ToString,
                            layer.TipologiaLayer_cod.ToString,
                            param.Parametro_Cod.ToString,
                            param.TipologiaLayer_struct_cod.ToString)

    End Function

    Private Function SalvaParametriConfigurazione(ByRef xWrite As AgronicaCoreGisDAL.ProiezioniLayer_W,
                                                  ByVal layer As ProiezioneLayer,
                                                  ByVal tipoRiga As Int32,
                                                  ByVal newConfId As Integer,
                                                  ByRef objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim resp As Boolean

        For Each param In layer.Params
            resp = xWrite.SalvaConfigurazioneProiezioneLayer(newConfId,
                                                             tipoRiga,
                                                             layer.LayerElementiGrafici_Cod,
                                                             layer.TipologiaLayer_cod,
                                                             param.Parametro_Cod,
                                                             param.TipologiaLayer_struct_cod,
                                                             objParametri_Server)
        Next

        Return resp
    End Function
    Private Function EliminaConfigurazione(ByVal configurazioneProiezione_Cod As Int32,
                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                           ByVal Optional bypassEliminazionePermessi As Boolean = True) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W

        resp = xWrite.EliminaConfigurazione(configurazioneProiezione_Cod, objParametri_Server)

        If Not resp Then
            Throw New Exception("Errore nell'eliminazione della configurazione.")
        End If

        If Not bypassEliminazionePermessi Then
            Dim xWritePermessi As New AgronicaCoreGisDAL.GIS_Permessi_Configurazione_W
        End If

        Return resp
    End Function

    Public Function SincronizzaConfigurazione(ByVal configurazioni As ElencoConfigurazioniProiezione,
                                              ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim idRequest As Integer = -1 'Lavez - 07/11/2024 - garbage per riunificazione metodo AttivaDisattivaConfigurazioneSuEntita

        Dim xRead As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            For Each configurazione In configurazioni.elencoConfigurazioniProiezione

                Dim listaEntita = xRead.LeggiElencoEntitaDaLayer(configurazione.Layer1.LayerElementiGrafici_Cod,
                                                                 objParametri_Server)
                For Each row In listaEntita.Rows
                    resp = AttivaDisattivaConfigurazioneSuEntita(configurazione.ConfigurazioneProiezione_Cod,
                                                                 CInt(row("Entita_Cod")),
                                                                 0,
                                                                 0,
                                                                 True,
                                                                 idRequest,
                                                                 objParametri_Server,
                                                                 False,
                                                                 False)

                    If Not resp Then
                        Throw New Exception("Errore nell'attivazione della configurazione sull'intero layer.")
                    End If

                Next
            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If
            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return resp

    End Function

    Public Function EliminaAttivazioneEntitaScadute(ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean
        Dim entitaScaduteDT As DataTable

        Dim xRead As New AgronicaCoreGisDAL.ProiezioniLayer_R
        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W

        entitaScaduteDT = xRead.LeggiAttivazioneEntitaScadute(objParametri_Server)

        If entitaScaduteDT Is Nothing Then
            Throw New Exception("Errore nella lettura delle entità scadute.")
        End If

        For Each row In entitaScaduteDT.Rows
            resp = xWrite.SetFlagAttivazioneEsecuzione(CInt(row("GIS_LayerAnalysisConfig_Exec_Log_Cod")), False, objParametri_Server)
        Next

        Return True
    End Function

    Public Function AggiornaStatusEsecuzioneAlgoritmo(ByVal codice_esecuzione As Int32,
                                                      ByVal status_esecuzione As Int32,
                                                      ByVal disattiva As Boolean,
                                                      ByVal messaggio As String,
                                                      ByRef objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim resp As Boolean
        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W

        resp = xWrite.AggiornaStatusEsecuzioneAlgoritmo(codice_esecuzione,
                                                        status_esecuzione,
                                                        disattiva,
                                                        messaggio,
                                                        objParametri_Server)

        Return resp
    End Function

    Public Function InserisciLogEsecuzioneAlgoritmo(ByVal configurazione_cod As Int32,
                                                    ByVal esecuzione_cod As Int32,
                                                    ByVal entita_cod_1 As Int32,
                                                    ByVal entita_cod_2 As Int32,
                                                    ByVal entita_cod_risultato As Int32,
                                                    ByVal statoElaborazione As Int32,
                                                    ByVal messaggi As String,
                                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                                    ByVal Optional entita_GUID_1 As String = "",
                                                    ByVal Optional entita_GUID_2 As String = "",
                                                    ByVal Optional entita_GUID_risultato As String = "",
                                                    ByVal Optional Risultato_Json As String = ""
                                                    ) As Boolean

        Dim resp As Boolean
        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W
        Dim sequenza As New Agro_Sequenze

        Dim newDetailLogID = sequenza.NuovoId_Tabella("GIS_LayerAnalysisConfig_Exec_Log_Details", 0, Int32.MaxValue, objParametri_Server)

        resp = xWrite.InserisciLogEsecuzioneAlgoritmo(newDetailLogID,
                                                      esecuzione_cod,
                                                      configurazione_cod,
                                                      entita_cod_1,
                                                      entita_cod_2,
                                                      entita_cod_risultato,
                                                      statoElaborazione,
                                                      messaggi,
                                                      objParametri_Server,
                                                      entita_GUID_1,
                                                      entita_GUID_2,
                                                      entita_GUID_risultato,
                                                      Risultato_Json)

        Return resp
    End Function

    Public Function SalvaGUIDEsecuzione(ByVal esecuzione_cod As Integer,
                                        ByVal newGuidEsecuzione As String,
                                        ByRef objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W

        resp = xWrite.SalvaGUIDEsecuzione(esecuzione_cod, newGuidEsecuzione, objParametri_Server)

        Return resp
    End Function

    Public Function SalvaGUIDConfigurazione(ByVal layerAnalysisConfig_Cod As Integer,
                                            ByVal layerAnalysisConfig_GUID As String,
                                            ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W

        resp = xWrite.SalvaGUIDConfigurazione(layerAnalysisConfig_Cod, layerAnalysisConfig_GUID, objParametri_Server)

        Return resp
    End Function

    Public Function InserisciLogEsecuzioneAlgoritmoGUID(ByVal featureProperties As JToken,
                                                        ByVal risultatoStringa As String,
                                                        ByVal messaggi As String,
                                                        ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean


        Dim xReadProiezione As New AgronicaCoreGisDAL.ProiezioniLayer_R
        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W

        Dim guidEsecuzione = featureProperties("GIS_LayerAnalysisConfig_Exec_Log_GUID").ToString
        Dim guidConfigurazione = featureProperties("LayerAnalysisConfig_GUID").ToString

        Dim configurazione As DataTable = xReadProiezione.LeggiConfigurazioneDaGUID(guidConfigurazione, objParametri_Server)
        Dim esecuzione As DataTable = xReadProiezione.LeggiEsecuzioneDaGUID(guidEsecuzione, objParametri_Server)

        Dim risultatiEsecuzione = CType(featureProperties("LayerAnalysisConfig_Response"), JArray)

        Dim sequenza As New Agro_Sequenze
        Dim estrattoreMessaggio As New MessaggiEsecuzioneGEE

        For Each risultato In risultatiEsecuzione

            Dim datiEntita As JObject = leggiDatiEntita(risultato, objParametri_Server)

            Dim messaggio As String = estrattoreMessaggio.EstraiMessaggio(risultato,
                                                                          messaggi,
                                                                          CInt(configurazione.Rows(0)("LayerAnalysisConfig_Algorithm_Cod")))

            Dim newLogID = sequenza.NuovoId_Tabella("GIS_LayerAnalysisConfig_Exec_Log_Details", 0, Int32.MaxValue, objParametri_Server)

            resp = xWrite.InserisciLogEsecuzioneAlgoritmo(newLogID,
                                                          CInt(esecuzione.Rows(0)("GIS_LayerAnalysisConfig_Exec_Log_Cod")),
                                                          CInt(configurazione.Rows(0)("LayerAnalysisConfig_Cod")),
                                                          CInt(datiEntita("entita1COD")),
                                                          CInt(datiEntita("entita2COD")),
                                                          CInt(datiEntita("entitaRisultatoCOD")),
                                                          1,
                                                          messaggio,
                                                          objParametri_Server,
                                                          datiEntita("entita1GUID").ToString,
                                                          datiEntita("entita2GUID").ToString,
                                                          datiEntita("entitaRisultatoGUID").ToString,
                                                          risultatoStringa)

            If Not resp Then
                Throw New Exception("Errore nell'inserimento del log dell'esecuzione.")
            End If
        Next

        Return resp
    End Function

    Public Function InserisciLogEsecuzioneAlgoritmoGuidSat(ByVal featureProperties As JToken,
                                                           ByVal risultatoStringa As String,
                                                           ByVal messaggi As String,
                                                           ByRef objParametri_Server As AgronicaCoreParametri) _
        As Boolean

        Dim resp As Boolean


        Dim xReadProiezione As New AgronicaCoreGisDAL.ProiezioniLayer_R
        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W

        Dim guidEsecuzione = featureProperties("GIS_LayerAnalysisConfig_Exec_Log_GUID").ToString
        Dim guidConfigurazione = featureProperties("LayerAnalysisConfig_GUID").ToString

        Dim configurazione As DataTable = xReadProiezione.LeggiConfigurazioneDaGUID(guidConfigurazione,
                                                                                    objParametri_Server)
        If (configurazione.Rows.Count = 0) Then
            Return True ' Old execution that probably have been disposed
        End If

        Dim esecuzione As DataTable = xReadProiezione.LeggiEsecuzioneDaGUID(guidEsecuzione, objParametri_Server)

        Dim risultatiEsecuzione = CType(featureProperties("LayerAnalysisConfig_Response"), JArray)

        Dim sequenza As New Agro_Sequenze
        Dim estrattoreMessaggio As New MessaggiEsecuzioneGEE

        For Each risultato In risultatiEsecuzione
            Dim datiEntita As JObject = leggiDatiEntita(risultato, objParametri_Server)
            Dim codiceSensore As String = LeggiCodiceSensoreDaRisultatoSat(risultato)
            Dim messaggio As String = estrattoreMessaggio.EstraiMessaggio(risultato,
                                                                          messaggi,
                                                                          CInt(
                                                                              configurazione.Rows(0)(
                                                                                  "LayerAnalysisConfig_Algorithm_Cod")))

            Dim newLogID = sequenza.NuovoId_Tabella("GIS_LayerAnalysisConfig_Exec_Log_Details", 0, Int32.MaxValue,
                                                    objParametri_Server)

            resp = xWrite.UpsertLogEsecuzioneAlgoritmoSat(
                newLogID,
                CInt(esecuzione.Rows(0)("GIS_LayerAnalysisConfig_Exec_Log_Cod")),
                CInt(configurazione.Rows(0)("LayerAnalysisConfig_Cod")),
                CInt(datiEntita("entita1COD")),
                CInt(datiEntita("entita2COD")),
                CInt(datiEntita("entitaRisultatoCOD")),
                1,
                messaggio,
                codiceSensore,
                objParametri_Server,
                datiEntita("entita1GUID").ToString,
                datiEntita("entita2GUID").ToString,
                datiEntita("entitaRisultatoGUID").ToString,
                risultatoStringa)

            If Not resp Then
                Throw New Exception("Errore nell'inserimento del log dell'esecuzione.")
            End If
        Next
        Return resp
    End Function

    Private Function LeggiCodiceSensoreDaRisultatoSat(ByVal risultato As JToken) As String
        Dim token As JToken = risultato.SelectToken("RisultatoElaborazione[0].Passaggi[0].Sensore[0].CodiceSensore")

        If token Is Nothing OrElse token.Type = JTokenType.Null Then
            Return ""
        End If

        Return token.ToString
    End Function

    Private Function leggiDatiEntita(ByVal risultato As JToken, ByRef objParametri_Server As AgronicaCoreParametri) As JObject
        Dim output As New JObject

        Dim xRead As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim Entita As DataTable

        Dim entita1GUID As String = ""
        Dim entita1COD As String = "0"
        Dim entita2GUID As String = ""
        Dim entita2COD As String = "0"
        Dim entitaRisultatoGUID As String = ""
        Dim entitaRisultatoCOD As String = "0"

        If risultato("Entita_GUID_1") IsNot Nothing AndAlso Not risultato("Entita_GUID_1").ToString.Equals("") Then
            entita1GUID = risultato("Entita_GUID_1").ToString

            Entita = xRead.LeggiEntitaDaGUID(entita1GUID, objParametri_Server)

            entita1COD = CInt(Entita.Rows(0)("Entita_Cod"))
        End If

        If risultato("Entita_GUID_2") IsNot Nothing AndAlso Not risultato("Entita_GUID_2").ToString.Equals("") Then
            entita2GUID = risultato("Entita_GUID_2").ToString

            Entita = xRead.LeggiEntitaDaGUID(entita2GUID, objParametri_Server)

            entita2COD = CInt(Entita.Rows(0)("Entita_Cod"))
        End If

        If risultato("Entita_GUID_Risultato") IsNot Nothing AndAlso Not risultato("Entita_GUID_Risultato").ToString.Equals("") Then
            entitaRisultatoGUID = risultato("Entita_GUID_Risultato").ToString

            Entita = xRead.LeggiEntitaDaGUID(entitaRisultatoGUID, objParametri_Server)

            entitaRisultatoCOD = CInt(Entita.Rows(0)("Entita_Cod"))
        End If

        output.Add("entita1GUID", entita1GUID)
        output.Add("entita2GUID", entita2GUID)
        output.Add("entitaRisultatoGUID", entitaRisultatoGUID)

        output.Add("entita1COD", entita1COD)
        output.Add("entita2COD", entita2COD)
        output.Add("entitaRisultatoCOD", entitaRisultatoCOD)

        Return output
    End Function

    Public Function SalvaElementoGraficoIntersezioneDaRisultatoGEE(ByVal esecuzione_cod As Int32,
                                                                   ByVal configurazione_cod As Int32,
                                                                   ByVal objElaborazione As JObject,
                                                                   ByRef newIdEntita_OUT As Int32,
                                                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                   Optional ByVal messaggio As String = "") As Boolean
        Dim resp As Boolean

        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R
        Dim xReadElementoGrafico As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R
        Dim configurazioneLayer = xRead.LeggiLayerDaConfigurazione(configurazione_cod, objParametri_Server)

        Dim xReadDatiEsecuzione As New AgronicaCoreGisDAL.ProiezioniLayer_R

        Dim DatiEsecuzioneDT As DataTable = xReadDatiEsecuzione.LeggiDatiEsecuzionePerScrittureEntitaGEE(esecuzione_cod, objParametri_Server)

        Dim DatiLayer1 = DatiEsecuzioneDT.Select("LayerXConfig_Cod = 1").FirstOrDefault
        Dim DatiLayer2 = DatiEsecuzioneDT.Select("LayerXConfig_Cod = 2").FirstOrDefault

        Dim xWriteEntita As New AgronicaCoreGisBIZ.GIS_Entita_W
        Dim sequenza As New Agro_Sequenze

        Dim newIDEntita = sequenza.NuovoId_Tabella("GIS_Entita", 0, Int32.MaxValue, objParametri_Server, True)

        resp = xWriteEntita.scriviEntitaBase(newIDEntita,
                                             CInt(TipiEnumerativi.enum_GIS2012_TipoEntita.DATI_IMPORTATI),
                                             objParametri_Server, objParametri_Utenti)

        If Not resp Then
            Throw New Exception("Errore nell'inserimento dell'entità.")
        End If

        Dim DT As DataTable

        If CInt(DatiLayer1("LayerElementiGrafici_Cod")) = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI Then
            DT = xReadElementoGrafico.LeggiDatiMinimiDaCodiceEntita(CInt(DatiLayer1("Entita_Cod_1")), objParametri_Server)
        Else
            DT = xReadElementoGrafico.LeggiDescrizioneElementoDaCodiceEntita(CInt(DatiLayer1("Entita_Cod_1")), objParametri_Server)
        End If

        If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
            Throw New Exception("Errore nel reperimento dell'elemento grafico.")
        End If

        Dim parametriRiga As New Dictionary(Of Int32, String)

        If CInt(DatiEsecuzioneDT.Rows(0)("LayerAnalysisConfig_AlgorithmType_Cod")) = TipiEnumerativi.enum_TipoAlgoritmoProiezione.Proiezione_Vettoriale_su_Raster Then
            parametriRiga.Add(1, objElaborazione.SelectToken("geoJsonPolygon.features[0].properties.LayerAnalysisConfig_Response[0].RisultatoElaborazione.CoveragePercentage").ToString)
        End If

        If CInt(DatiEsecuzioneDT.Rows(0)("LayerAnalysisConfig_AlgorithmType_Cod")) = TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync Then
            parametriRiga.Add(1, objElaborazione("1.0").ToString)
        End If

        If CInt(DatiEsecuzioneDT.Rows(0)("LayerAnalysisConfig_AlgorithmType_Cod")) = TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync_Yearly Then
            parametriRiga.Add(1, messaggio)
        End If

        parametriRiga.Add(3, DT.Rows(0)("ElementoGrafico_Des").ToString)
        parametriRiga.Add(2, DatiLayer2("LayerElementiGrafici_Des").ToString)

        Dim ElementoGrafico_Des As String = preparaDescrizioneElementoGrafico(configurazioneLayer.LayerRisultato, parametriRiga)

        Dim xWrite As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_W

        Dim newIDElementoGrafico = sequenza.NuovoId_Tabella("GIS_ElementiGrafici", 0, Int32.MaxValue, objParametri_Server, True)

        resp = xWrite.scriviElementoGraficoBaseDaWKT(newIDElementoGrafico,
                                                     ElementoGrafico_Des,
                                                     newIDEntita,
                                                     configurazioneLayer.LayerRisultato.LayerElementiGrafici_Cod,
                                                     DatiLayer1("GeoData").ToString,
                                                     objParametri_Server)

        If Not resp Then
            Throw New Exception("Errore nell'inserimento dell'elemento grafico.")
        End If

        newIdEntita_OUT = newIDEntita




        Return resp
    End Function

    Public Function ValorizzaParametriVisibilitaEntita(ByVal entita_cod As Integer,
                                                       ByVal objElaborazione As JObject,
                                                       ByRef objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim resp As Boolean = True

        Dim xRead As New AgronicaCoreGisBIZ.GIS_Entita_R
        Dim xWrite As New AgronicaCoreGisBIZ.GIS_Entita_W

        Dim Parametri_entita = xRead.LeggiParametriVisualizzazioneDaEntita_Cod(entita_cod, objParametri_Server)

        If Parametri_entita.Equals("") Then
            Dim objParametriVisualizzazione = objElaborazione.SelectToken("geoJsonPolygon.features[0].properties.LayerAnalysisConfig_Response[0].RisultatoElaborazione.ParametriVisualizzazioneLayer")

            Dim params = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ParametriVisualizzazioneLayer)(objElaborazione.ToString)

            'DEBUG DEBUG DEBUG
            'params = New ParametriVisualizzazioneEntita With {
            '    .type = TypeVisualizzazioneEntita.CLOUD_STORAGE,
            '    .baseUrl = "https://storage.cloud.google.com/",
            '    .bucket = "gee_objects_private",
            '    .obj = "mapTilesForEE",
            '    .gsUriFile = ""
            '}
            'DEBUG DEBUG DEBUG

            resp = xWrite.SalvaParametriVisualizzazioneEntita(entita_cod,
                                                              Newtonsoft.Json.JsonConvert.SerializeObject(params),
                                                              objParametri_Server)

            If Not resp Then
                Throw New Exception("Impossibile salvare i parametri di visualizzazione per l'entità.")
            End If
        End If

        Return resp
    End Function

    'Overload per risposta upload Raster
    Public Function ValorizzaParametriVisibilitaEntita(ByVal entita_cod As Integer,
                                                       ByVal params As ParametriVisualizzazioneLayer,
                                                       ByRef objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim resp As Boolean = True

        Dim xWrite As New AgronicaCoreGisBIZ.GIS_Entita_W


        'DEBUG DEBUG DEBUG
        'params = New ParametriVisualizzazioneEntita With {
        '    .type = TypeVisualizzazioneEntita.CLOUD_STORAGE,
        '    .baseUrl = "https://storage.cloud.google.com/",
        '    .bucket = "gee_objects_private",
        '    .obj = "mapTilesForEE",
        '    .gsUriFile = ""
        '}
        'DEBUG DEBUG DEBUG

        resp = xWrite.SalvaParametriVisualizzazioneEntita(entita_cod,
                                                          Newtonsoft.Json.JsonConvert.SerializeObject(params),
                                                          objParametri_Server)

        If Not resp Then
                Throw New Exception("Impossibile salvare i parametri di visualizzazione per l'entità.")
            End If

        Return resp
    End Function

    Private Function preparaDescrizioneElementoGrafico(ByVal layer As ProiezioneLayer,
                                                       ByVal parametriRiga As Dictionary(Of Int32, String)) As String

        Dim partiDescrizione As New List(Of String)

        For Each tupla In parametriRiga
            Dim etichetta = layer.Params.Where(Function(s) s.GIS_LayerAnalysisConfig_AlgorithmType_Param_Cod = tupla.Key).FirstOrDefault.LayerElementiGrafici_Etichetta
            partiDescrizione.Add(String.Format("{0}§ {1}", etichetta, tupla.Value.ToString))
        Next

        Return String.Join("|", partiDescrizione)
    End Function

    Public Function AccodaEsecuzioneEsportazione(ByVal inData As EsportaShapeEntita_In,
                                                 ByRef objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W
        Dim sequenze As New Agro_Sequenze

        Dim resp As Boolean

        Dim newEsecuzioneId = sequenze.NuovoId_Tabella("GIS_LayerAnalysisConfig_Exec_Log",
                                                       0, Int32.MaxValue, objParametri_Server, True)

        Dim inDataObj As JObject = JObject.FromObject(inData)
        inDataObj.Add("UtenteOperazione", objParametri_Server.UtenteUsername)
        inDataObj.Add("LayerAnalysisConfig_Algorithm_Cod", TipiEnumerativi.enum_AlgoritmoProiezione.EsportazioneBulkLayer)

        resp = xWrite.InserisciAttivazioneEsecuzione(newEsecuzioneId, 0, 0, 0, 0, objParametri_Server, inDataObj.ToString)

        Return resp
    End Function

    Private Function InserisciAttivazioneEsecuzione(ByVal configurazioneProiezione_Cod As Int32,
                                                    ByVal entita_cod_1 As Int32,
                                                    ByVal entita_cod_2 As Int32,
                                                    ByVal entita_cod_risultato As Int32,
                                                    ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.ProiezioniLayer_W
        Dim sequenze As New Agro_Sequenze
        Dim resp As Boolean

        Dim newEsecuzioneId = sequenze.NuovoId_Tabella(
            "GIS_LayerAnalysisConfig_Exec_Log",
            0,
            Int32.MaxValue,
            objParametri_Server,
            True
            )

        resp = xWrite.InserisciAttivazioneEsecuzione(
            newEsecuzioneId,
            configurazioneProiezione_Cod,
            entita_cod_1,
            entita_cod_2,
            entita_cod_risultato,
            objParametri_Server
            )

        Return resp
    End Function

    Private Function GetLayerAnalysisConfigResponseAsArray(ByVal featureProperties As JToken) As JArray
        Dim responseToken = featureProperties("LayerAnalysisConfig_Response")

        If responseToken Is Nothing OrElse responseToken.Type = JTokenType.Null Then
            Return New JArray()
        End If

        If TypeOf responseToken Is JArray Then
            Return CType(responseToken, JArray)
        End If

        If TypeOf responseToken Is JObject Then
            Return New JArray(CType(responseToken, JObject))
        End If

        Return JArray.FromObject(responseToken)
    End Function
End Class

