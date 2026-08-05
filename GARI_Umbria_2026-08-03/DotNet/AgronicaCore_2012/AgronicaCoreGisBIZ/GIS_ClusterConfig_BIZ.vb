Imports AgronicaCoreDataProvider
Imports AgronicaCoreGisDAL
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreDTOStd
Imports InData.Gis
Public Class GIS_ClusterConfig_BIZ_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Recupera il profilo di configurazione per un utente/layer e lo mappa in un oggetto DTO.
    ''' </summary>
    ''' <param name="tipoAlgoritmoCod">Tipo algoritmo (fisso a 1).</param>
    ''' <param name="layerCod">ID del layer.</param>
    ''' <param name="utente">Username.</param>
    ''' <param name="objParametri">Oggetto di contesto.</param>
    ''' <returns>Un DTO GisClusterConfigSave_InData completo, o Nothing se non trovato.</returns>
    Public Function LeggiConfigurazione(
        ByVal tipoAlgoritmoCod As Integer,
        ByVal layerCod As Integer,
        ByVal utente As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As GisClusterConfigSave_InData

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Clustering_Config_R.LeggiConfigurazione()"

        Try

            Dim dalConfig As New GIS_LayerElementiGrafici_Clustering_Config_R()

            Dim dtConfig As DataTable = dalConfig.LeggiConfigurazione(tipoAlgoritmoCod, layerCod, utente, objParametri)

            ' Mappa il DataTable in un oggetto DTO strutturato.
            Return MapDataTableToDto(dtConfig)

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Function

    ''' <summary>
    ''' Metodo helper privato che trasforma un DataTable in un DTO (GisClusterConfigSave_InData).
    ''' </summary>
    Private Function MapDataTableToDto(ByVal dt As DataTable) As GisClusterConfigSave_InData
        ' Se il DAL non restituisce dati, significa che non esiste un profilo, quindi restituisce Nothing.
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return Nothing
        End If

        ' Tutti i dati della testata sono uguali in ogni riga del DataTable a causa della JOIN.
        ' Quindi, li prendiamo dalla prima riga per costruire l'oggetto principale.
        Dim firstRow = dt.Rows(0)

        ' Crea l'oggetto DTO principale (la "testata").
        Dim configDto As New GisClusterConfigSave_InData() With {
            .LayerElementiGraficiConfigTypeCod = CInt(firstRow("LayerElementiGrafici_Clustering_Type_Cod")),
            .LayerElementiGraficiCod = CInt(firstRow("LayerElementiGrafici_Cod")),
            .Utente = firstRow("Utente").ToString(),
            .LivelloZoomMassimoVisualizzazioneRaggruppata = CInt(firstRow("Livello_Zoom_Massimo_Visualizzazione_Raggruppata"))
        }

        ' Ciclo su tutte le righe del DataTable per estrarre i dettagli.
        For Each row As DataRow In dt.Rows
            ' Aggiungiamo un dettaglio alla lista solo se la sua chiave primaria non è nulla.
            ' Questo gestisce il caso di una configurazione che esiste ma non ha dettagli.
            If Not IsDBNull(row("LayerElementiGrafici_Clustering_Config_Detail_Cod")) Then
                configDto.Details.Add(New GisClusterConfigDetail_InData() With {
                    .Id = CInt(row("LayerElementiGrafici_Clustering_Config_Detail_Cod")),
                    .Description = row("Descrizione").ToString(),
                    .Parameters = row("Parametri").ToString()
                })
            End If
        Next

        Return configDto
    End Function

End Class

Public Class GIS_ClusterConfig_BIZ_W
    Inherits AgronicaCoreDataProvider.DataProvider
End Class