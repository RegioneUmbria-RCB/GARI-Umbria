Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelloInSviluppo
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Sat_Sentinel


    Public Function VerificheSuElaborazioniConfigurazioneUtente(FiltroElaborazioni As Integer, ListaConfigurazioni As String, ByVal DataRiferimentoElaborazioni As String, objParametri_Server As AgronicaCoreParametri) As ElaborazioniConfigurazioneUtente_List

        Dim letturaSituazioneElaborazioni As New AgronicaCoreGisDAL.Gis_Sat_Sentinel_Overlay_R
        Dim dtSituazioneElaborazioni As DataTable =
            letturaSituazioneElaborazioni.leggiRitagliDaElaborare(
                True,
                DataRiferimentoElaborazioni,
                FiltroElaborazioni,
                " cfg.Gis_Sat_Sentinel_User_Config_COD in (" & ListaConfigurazioni & ") ",
                "",
                objParametri_Server
             )


        Dim rval As New ElaborazioniConfigurazioneUtente_List
        rval.ListaElaborazioni = (
            From t In dtSituazioneElaborazioni.AsEnumerable
            Select New ElaborazioniConfigurazioneUtente_Elemento With {
                    .Gis_Sat_Sentinel_User_Config_COD = CInt(t("Gis_Sat_Sentinel_User_Config_COD")),
                    .DataRiferimentoElaborazione = CStr(t("DataRiferimentoElaborazioni")),
                    .NumeroElaborazioni = CInt(t("Conteggio")),
                    .StatoElaborazione = CStr(t("Stato"))
                    }
                ).ToList


        Return rval

    End Function

    Public Function customMapOverlayBaseInizializzaCalendario(PoligonoWKT As String, ByVal DataInizio As Date, ByVal datafine As Date, ByVal ObjParametri_Server As AgronicaCoreParametri) As Gis_Sat_Sentinel_Overlay_list

        Dim filtroAggiuntivoDate As String = ""

        If DataInizio <> AGRODATAINIZIO Then
            filtroAggiuntivoDate &= " AND P.DataRiferimento > " & UtilityProvider.Agro_SQL_SaveDate(DataInizio)
        End If


        If datafine <> AGRODATAFINE Then
            filtroAggiuntivoDate &= " AND P.DataRiferimento < " & UtilityProvider.Agro_SQL_SaveDate(datafine)
        End If

        Dim LetturaDatiOverlay As New AgronicaCoreGisDAL.Gis_Sat_Sentinel_Overlay_R
        Dim dtLetturaDatiOverlay As DataTable =
            LetturaDatiOverlay.Leggi(PoligonoWKT, filtroAggiuntivoDate, "", ObjParametri_Server)


        ''test data... 
        Dim rval As New Gis_Sat_Sentinel_Overlay_list
        Dim tmpDate As List(Of String) = (From dd In dtLetturaDatiOverlay.AsEnumerable
                                          Select CStr(dd("DataRiferimento"))).Distinct.ToList

        rval.ListaOverlayer = (From t In tmpDate Select New Gis_Sat_Sentinel_Overlay With {.DataRiferimento = t, .Passaggi = New List(Of Gis_Sat_Sentinel_Overlay_Passaggio)}).ToList()


        For Each ov In rval.ListaOverlayer

            Dim tTile As List(Of String) = (From aTile In dtLetturaDatiOverlay.AsEnumerable
                                            Where aTile("DataRiferimento") = ov.DataRiferimento
                                            Select CStr(aTile("Tile")) & "|" & CStr(aTile("url")) & "|" & CStr(aTile("GEORiferimento_COD").ToString())).Distinct.ToList()

            For Each curTile In tTile

                Dim vTile As String() = curTile.Split("|")

                Dim passaggio As Gis_Sat_Sentinel_Overlay_Passaggio = New Gis_Sat_Sentinel_Overlay_Passaggio With {
                                .Sensore = New List(Of Gis_Sat_Sentinel_Overlay_Sensore),
                                .Tile = New Gis_Sat_Sentinel_Overlay_Tile With {.Tile = vTile(0), .GEORiferimento_COD = vTile(2)},
                                .url = vTile(1)
                                }


                Dim tSensori As List(Of String) = (From aTile In dtLetturaDatiOverlay.AsEnumerable
                                                   Where aTile("DataRiferimento") = ov.DataRiferimento _
                                                    And aTile("url") = vTile(1)
                                                   Select CStr(aTile("Sensore"))).Distinct.ToList()

                For Each sens In tSensori
                    passaggio.Sensore.Add(New Gis_Sat_Sentinel_Overlay_Sensore With {.Descrizione = sens, .CodiceSensore = sens})
                Next

                ov.Passaggi.Add(passaggio)

            Next

        Next


        Return rval

    End Function


    Public Function customMapOverlayBaseInizializzaCalendario_flat(PoligonoWKT As String, ByVal ObjParametri_Server As AgronicaCoreParametri) As List(Of Gis_Sat_Sentinel_Overlay_Flat)

        Dim listaOverlay As Gis_Sat_Sentinel_Overlay_list =
            customMapOverlayBaseInizializzaCalendario(PoligonoWKT, AGRODATAINIZIO, AGRODATAFINE, ObjParametri_Server)


        Dim FlatList As New List(Of Gis_Sat_Sentinel_Overlay_Flat)

        For Each ov1 As Gis_Sat_Sentinel_Overlay In listaOverlay.ListaOverlayer

            For Each ovData As Gis_Sat_Sentinel_Overlay_Passaggio In ov1.Passaggi



                For Each curSensore As Gis_Sat_Sentinel_Overlay_Sensore In ovData.Sensore

                    FlatList.Add(New Gis_Sat_Sentinel_Overlay_Flat With {.SensoreElaborazione = curSensore.CodiceSensore, .DataRiferimento = ov1.DataRiferimento, .Tile = ovData.Tile.Tile, .url = ovData.url})

                Next


            Next

        Next

        Return FlatList
    End Function

End Class
