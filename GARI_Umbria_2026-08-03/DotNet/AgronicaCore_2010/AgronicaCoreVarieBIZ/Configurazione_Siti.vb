Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDTOStd.InData.Utility
Imports AgronicaCoreModelsSTD.Gis
Imports Newtonsoft.Json

Public Class Configurazione_Siti_BIZ_R
    Public Function leggiConfigurazioneEndpointMappeSatellitari(ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                ByRef objParametri_Server As AgronicaCoreParametri) As EndpointMappeSatellitari

        Dim endpoint As New EndpointMappeSatellitari With {
            .datiEndpoint = New EndpointGEE,
            .legacy_endpoint = ""
        }

        Dim utentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim isGestioneLegacy As Boolean = False

        Dim chiaveConfigurazione = CostantiPersonalizzate.MappeSatellitari_NuovaChiaveConf

        Dim listaImpostazioni = utentiImpostazioni.Leggi(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_GIS,
                                                         1,
                                                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                         "",
                                                         "",
                                                         objParametri_Utenti)

        If listaImpostazioni.Rows.Count > 0 Then

            Dim valori As String() = listaImpostazioni(0)("Impostazione_Valore_1").Split("§")

            Dim gestioneLegacyParam = valori.Where(Function(s) s.StartsWith("ckGestioneAnalisiMappeLegacy"))

            If gestioneLegacyParam.Count > 0 Then
                isGestioneLegacy = CBool(gestioneLegacyParam.FirstOrDefault.Split("^")(1))
            End If
            'Lavez - 04/12/2024 - CAI - Chiamata Performa 33856 - In caso l'utente non abbia l'impostazione in questione deve risultare che utilizza il nuovo GIS
            'Else
            '	Throw New Exception("Errore nella lettura dell'impostazione utente.")
        End If

        If isGestioneLegacy Then
            chiaveConfigurazione = CostantiPersonalizzate.MappeSatellitari_VecchiaChiaveConf
        End If

        Dim cfgRead As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim DT = cfgRead.Leggi(0, chiaveConfigurazione, "", "", objParametri_Server)

        If DT.Rows.Count <> 1 Then
            Throw New Exception("Errore nel recupero dell'endpoint.")
        End If

        Dim valoreConfigurazione = DT.Rows(0)("Valore").ToString

        If isGestioneLegacy Then
            endpoint.legacy_endpoint = String.Format("{0}{1}", valoreConfigurazione, CostantiPersonalizzate.MappeSatellitari_EndpointLegacySuffisso)
        Else
            endpoint.datiEndpoint = Newtonsoft.Json.JsonConvert.DeserializeObject(Of EndpointGEE)(System.Text.RegularExpressions.Regex.Unescape(valoreConfigurazione))
        End If

        Return endpoint
    End Function

    Public Function LeggiSeUsareWSMappe2024oGEE(ByRef objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim IsWSMappe2024 As Boolean = False

        Dim xCfgSitiR As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim dt = xCfgSitiR.Leggi(0, CostantiPersonalizzate.WS_Mappe_ConfKey, "", "", objParametri_Server)
        If dt.Rows.Count > 0 Then
            Dim ws_mappe_config = JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.InData.Gis.WS_Mappe_Config)(dt.Rows(0)("Valore").ToString)
            If ws_mappe_config.baseUrl <> "" Then
                IsWSMappe2024 = True
            End If
        End If
        Return IsWSMappe2024
    End Function

    Public Function leggiSatTenantName(ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim xCfgSitiR As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim dt = xCfgSitiR.Leggi(0, CostantiPersonalizzate.SAT_TenantName_ConfKey, "", "", objParametri_Server)
        If dt Is Nothing Or dt.Rows.Count = 0 Then
            Return Nothing
        End If

        Return dt.Rows(0)("Valore").ToString()
    End Function

    Public Function CheckSottoscrizioneServizioQDC(objServer As AgronicaCoreParametri) As Boolean
        Dim ret As Boolean = False
        Try
            Dim cfgSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim dt = cfgSiti.Leggi(0, "Verifica_Sottoscrizione_Servizio_QDC", "", "", objServer)
            If dt.Rows.Count > 0 Then
                ret = CType(dt.Rows(0)("Valore"), Boolean)
            End If
        Catch ex As Exception
            ret = False
            Throw ex
        End Try

        Return ret
    End Function


    Public Function GetConfigControlloServizioQDC(objServer As AgronicaCoreParametri) As ConfigurazioneControlloServizioQDC(Of Object)
        Dim ret As ConfigurazioneControlloServizioQDC(Of Object) = Nothing
        Try
            Dim cfgSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim dt = cfgSiti.Leggi(0, "Tipo_Verifica_Sottoscrizione_Servizio_QDC", "", "", objServer)
            If dt.Rows.Count > 0 Then
                ret = JsonConvert.DeserializeObject(Of ConfigurazioneControlloServizioQDC(Of Object))(dt.Rows(0)("Valore").ToString())
            End If
        Catch ex As Exception
            ret = Nothing
            Throw ex
        End Try

        Return ret
    End Function

End Class
Public Class Configurazione_Siti_BIZ_W

End Class
