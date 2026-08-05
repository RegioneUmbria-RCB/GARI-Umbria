Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Gis
Imports Newtonsoft.Json.Linq

Public Class AlgoritmoBaseWSMappe
    Inherits LogProvider
    Implements IAlgoritmoProiezione

    Protected LayerAnalysisConfig_Algorithm_Cod As Int32
    Protected LayerAnalysisConfig_AlgorithmType_Cod As Int32

    Protected Layer_1 As ProiezioneLayer
    Protected Layer_2 As ProiezioneLayer
    Protected Layer_Risultato As ProiezioneLayer
    Protected AttivoTuttoLayer As Boolean
    Protected LayerAnalysisConfig_GUID As String

    Protected ws_mappe_config As WS_Mappe_Config
    Protected _caller As WebApiCaller

    Protected Const ENDPOINT_CUSTOMOVERLAYBASE_INIZIALIZZACALENDARIO As String = "Maps/CustomMapOverlayBaseInizializzaCalendario"
    Protected Const ENDPOINT_RASTERCROP As String = "GdalData/RasterCropRequest"
    Protected Const ENDPOINT_RASTERIZED As String = "Maps/RequestRasterized"

    Public Sub LeggiLayerDaConfigurazioneAlgoritmo(LayerAnalysisConfig_Cod As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri) Implements IAlgoritmoProiezione.LeggiLayerDaConfigurazioneAlgoritmo
        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R

        Dim configurazione = xRead.LeggiLayerDaConfigurazione(LayerAnalysisConfig_Cod, objParametri)

        Me.Layer_1 = configurazione.Layer1
        Me.Layer_2 = configurazione.Layer2
        Me.Layer_Risultato = configurazione.LayerRisultato
        Me.AttivoTuttoLayer = configurazione.AttivoTuttiLayer
        Me.LayerAnalysisConfig_GUID = configurazione.ConfigurazioneProiezione_GUID
    End Sub

    Public Sub LeggiLayerDaParametriEsecuzione(ParametriEsecuzione As String) Implements IAlgoritmoProiezione.LeggiLayerDaParametriEsecuzione
        Dim parametriObj As JObject = JObject.Parse(ParametriEsecuzione)

        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R

        Me.Layer_1 = New ProiezioneLayer With {
            .LayerElementiGrafici_Cod = CInt(parametriObj("layerElementiGrafici_Cod"))
        }
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
                           Optional override_transazione As Boolean = False) As Boolean Implements IAlgoritmoProiezione.Esegui
        Return True
    End Function

    Protected Sub SetupDati(LayerAnalysisConfig_Cod As Int32,
                               Entita_cod_1 As Int32,
                               Entita_cod_2 As Int32,
                               Entita_cod_Risultato As Int32,
                               ByRef objParametri As AgronicaCoreParametri)

        LeggiLayerDaConfigurazioneAlgoritmo(LayerAnalysisConfig_Cod, objParametri)
        ValorizzaGUIDParametri(LayerAnalysisConfig_Cod, objParametri)
    End Sub

    Protected Sub ValorizzaGUIDParametri(ByVal LayerAnalysisConfig_Cod As Int32,
                                   ByRef objParametri_Server As AgronicaCoreParametri)
        Dim resp As Boolean

        If Me.LayerAnalysisConfig_GUID Is Nothing OrElse Me.LayerAnalysisConfig_GUID.Equals("") Then
            Me.LayerAnalysisConfig_GUID = Guid.NewGuid().ToString

            Dim xWriteProiezioni As New AgronicaCoreGisBIZ.ProiezioniLayer_W
            resp = xWriteProiezioni.SalvaGUIDConfigurazione(LayerAnalysisConfig_Cod, Me.LayerAnalysisConfig_GUID, objParametri_Server)

            If Not resp Then
                Throw New Exception("Impossibile salvare il GUID per la configurazione.")
            End If
        End If

        VerificaGUIDLayer(Layer_1, objParametri_Server)
        VerificaGUIDLayer(Layer_2, objParametri_Server)
        VerificaGUIDLayer(Layer_Risultato, objParametri_Server)

    End Sub

    Private Sub VerificaGUIDLayer(ByRef layer As ProiezioneLayer,
                                  ByRef objParametri_Server As AgronicaCoreParametri)

        Dim xWrite As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici

        If layer.LayerElementiGrafici_GUID Is Nothing OrElse layer.LayerElementiGrafici_GUID.Equals("") Then
            Dim newGuidLayer = Guid.NewGuid.ToString

            Dim resp = xWrite.SalvaGUIDLayer(layer.LayerElementiGrafici_Cod,
                                             layer.TipologiaLayer_cod,
                                             newGuidLayer,
                                             objParametri_Server)

            If Not resp Then
                Throw New Exception("Errore nell'aggiornamento del GUID del layer.")
            End If

            layer.LayerElementiGrafici_GUID = newGuidLayer
        End If

        For Each param In layer.Params
            If param.TipologiaLayer_struct_GUID Is Nothing OrElse param.TipologiaLayer_struct_GUID.Equals("") Then
                Dim newGuidParam = Guid.NewGuid.ToString

                Dim resp = xWrite.SalvaGUIDStruct(param.TipologiaLayer_struct_cod, newGuidParam, objParametri_Server)

                If Not resp Then
                    Throw New Exception("Errore nell'aggiornamento del GUID del parametro.")
                End If

                param.TipologiaLayer_struct_GUID = newGuidParam
            End If
        Next
    End Sub

    Protected Function LeggiGUIDEntita(ByVal entita_cod As Int32,
                                      ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim xRead As New AgronicaCoreGisBIZ.GIS_Entita_R
        Dim xWrite As New AgronicaCoreGisBIZ.GIS_Entita_W

        Dim GUID_entita = xRead.LeggiGUIDDaEntita_Cod(entita_cod, objParametri_Server)

        If GUID_entita.Equals("") Then
            GUID_entita = Guid.NewGuid().ToString

            Dim resp = xWrite.SalvaGUIDEntita(entita_cod, GUID_entita, objParametri_Server)

            If Not resp Then
                Throw New Exception("Impossibile salvare il GUID per l'entità.")
            End If
        End If

        Return GUID_entita
    End Function

    Protected Function CreaNuovoGUIDEsecuzione(ByVal Esecuzione_cod As Int32,
                                         ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim newGuidEsecuzione = Guid.NewGuid().ToString

        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

        Dim resp = xWrite.SalvaGUIDEsecuzione(Esecuzione_cod, newGuidEsecuzione, objParametri_Server)

        If Not resp Then
            Throw New Exception("Impossibile salvare il nuovo GUID per l'esecuzione.")
        End If

        Return newGuidEsecuzione

    End Function

    Protected Function GetPathToSave(ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim xCfgSitiR As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim dtCFG = xCfgSitiR.Leggi(0, "GestioneAllegati_Repository", "", "", objParametri_Server)
        Dim savePath As String = ""
        If dtCFG.Rows.Count > 0 Then
            savePath = IO.Path.Combine(dtCFG.Rows(0)("Valore"), "WSMappeRaster")
            If Not IO.Directory.Exists(savePath) Then
                IO.Directory.CreateDirectory(savePath)
            End If
        Else
            Throw New Exception("Path salvataggio file tif non trovato. impossibile proseguire")
        End If
        Return savePath
    End Function
End Class
