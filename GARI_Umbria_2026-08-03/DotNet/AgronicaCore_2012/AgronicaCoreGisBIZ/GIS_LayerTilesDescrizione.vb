Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreVarieBIZ

Public Class GIS_LayerTilesDescrizione_R

    Public Function LeggiLayerTilesDescrizione(ByVal LayerTiles_Cod As Int32,
                                               ByVal TipologiaLayer_cod As Int32,
                                               ByVal LayerElementiGrafici_Cod As Int32,
                                               ByVal LayerTilesDescrizione_Cod As Int32,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As rispostaStandard(Of List(Of TipologiaLabel))

        Dim resp As New rispostaStandard(Of List(Of TipologiaLabel))

        Dim ltdList As New List(Of TipologiaLabel)
        Dim DT As New DataTable

        Dim xRead As New AgronicaCoreGisDAL.GIS_LayerTilesDescrizione_R

        Try

            DT = xRead.Leggi(LayerTiles_Cod,
                             TipologiaLayer_cod,
                             LayerElementiGrafici_Cod,
                             LayerTilesDescrizione_Cod,
                             "",
                             "",
                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             objParametri)

            For Each row In DT.Rows
                Dim tipologiaLabel As New TipologiaLabel With {
                    .LayerElementiGrafici_Cod = Convert.ToInt32(row("LayerElementiGrafici_Cod")),
                    .LayerTilesDescrizione_Cod = Convert.ToInt32(row("LayerTilesDescrizione_Cod")),
                    .TipologiaLayer_cod = Convert.ToInt32(row("TipologiaLayer_cod")),
                    .LayerTiles_Cod = Convert.ToInt32(row("LayerTiles_Cod")),
                    .valore_min = Convert.ToDouble(row("Valore_a")),
                    .valore_max = Convert.ToDouble(row("Valore_da")),
                    .valore_associato = Convert.ToDouble(row("Valore_associato")),
                    .colore = row("Colore_Base").ToString,
                    .label = row("LayerTilesDescrizione_Des").ToString
                    }

                ltdList.Add(tipologiaLabel)
            Next

            resp.RispostaOK = True
            resp.RispostaStringa = ltdList

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = ex.Message

        End Try

        Return resp

    End Function

End Class
Public Class GIS_LayerTilesDescrizione_W

    Public Function SalvaLayerTilesDescrizione(ByVal newLTDList As List(Of TipologiaLabel),
                                               ByVal necessariaTransazione As Boolean,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As RispostaStandard
        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisDAL.GIS_LayerTilesDescrizione_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            If necessariaTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri)
            End If

            For Each newLTD In newLTDList

                If newLTD.LayerTilesDescrizione_Cod <> 0 Then
                    Throw New Exception("Impossibile salvare un nuovo dettaglio tema con ID prevalorizzato.")
                End If

                If newLTD.LayerTiles_Cod = 0 _
                    Or newLTD.TipologiaLayer_cod = 0 _
                    Or newLTD.LayerElementiGrafici_Cod = 0 Then

                    Throw New Exception("Un dettaglio tema deve referenziare un layer, un elemento grafico ed una tipologia layer.")

                End If

                resp.RispostaOK = xWrite.Scrivi(newLTD,
                                            CostantiPersonalizzate.AGRODATAINIZIO,
                                            CostantiPersonalizzate.AGRODATAFINE,
                                            objParametri)

                If Not resp.RispostaOK Then
                    Throw New Exception("Errore nel salvataggio del dettaglio tema.")
                End If


            Next

            resp.RispostaStringa = "Operazione completata con successo."

            If necessariaTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            End If
        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            resp.RispostaOK = False
            resp.Errore = ex.Message

        Finally
            If necessariaTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
            End If
        End Try

        Return resp
    End Function

    Public Function AggiornaLayerTilesDescrizione(ByVal LayerTilesDescrizioneList As List(Of TipologiaLabel),
                                                  ByVal necessariaTransazione As Boolean,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As RispostaStandard
        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisDAL.GIS_LayerTilesDescrizione_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            If necessariaTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri)
            End If

            For Each LayerTilesDescrizione In LayerTilesDescrizioneList

                If LayerTilesDescrizione.LayerTilesDescrizione_Cod = 0 Then
                    Throw New Exception("Impossibile aggiornare un dettaglio tema senza identificatore.")
                End If

                If LayerTilesDescrizione.LayerTiles_Cod = 0 _
                    Or LayerTilesDescrizione.TipologiaLayer_cod = 0 _
                    Or LayerTilesDescrizione.LayerElementiGrafici_Cod = 0 Then

                    Throw New Exception("Un dettaglio tema deve referenziare un layer, un elemento grafico ed una tipologia layer.")

                End If

                resp.RispostaOK = xWrite.Aggiorna(LayerTilesDescrizione, objParametri)

                If Not resp.RispostaOK Then
                    Throw New Exception("Errore nell'aggiornamento del dettaglio tema.")
                End If

            Next

            resp.RispostaStringa = "Operazione completata con successo."

            If necessariaTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            End If
        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            resp.RispostaOK = False
            resp.Errore = ex.Message

        Finally
            If necessariaTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
            End If
        End Try

        Return resp
    End Function

    Public Function EliminaLayerTilesDescrizione(ByVal LayerTilesDescrizioneList As List(Of TipologiaLabel),
                                                 ByVal necessariaTransazione As Boolean,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As RispostaStandard
        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisDAL.GIS_LayerTilesDescrizione_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            If necessariaTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri)
            End If

            For Each LayerTilesDescrizione In LayerTilesDescrizioneList

                If LayerTilesDescrizione.LayerTilesDescrizione_Cod = 0 Then
                    Throw New Exception("Impossibile eliminare un dettaglio tema senza identificatore.")
                End If

                resp.RispostaOK = xWrite.Elimina(LayerTilesDescrizione, objParametri)

                If Not resp.RispostaOK Then
                    Throw New Exception("Errore nell'aggiornamento del dettaglio tema.")
                End If
            Next

            resp.RispostaStringa = "Operazione completata con successo."

            If necessariaTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            End If
        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            resp.RispostaOK = False
            resp.Errore = ex.Message

        Finally
            If necessariaTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
            End If
        End Try

        Return resp
    End Function

    Public Function DeleteAllTilesDescriptionsPerLayer(pivaSuperUser As String,
                                                      layerCod As Int32,
                                                      objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim objLayerTiles As New AgronicaCoreGisDAL.GIS_LayerTilesDescrizione_W
        Return objLayerTiles.DeleteAllTilesDescriptionsPerLayer(pivaSuperUser, layerCod, objParametriServer)
    End Function
End Class
