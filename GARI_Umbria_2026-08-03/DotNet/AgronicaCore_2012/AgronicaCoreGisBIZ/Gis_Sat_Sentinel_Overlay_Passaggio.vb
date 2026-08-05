Public Class Gis_Sat_Sentinel_Overlay_Passaggio_W

    Public Function Scrivi(
           DataRiferimento As Date,
           Tile As String,
           url As String,
           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Const NomeRoutine As String = "GIS_Entita_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)

            Dim Scrittura As New AgronicaCoreGisDAL.Gis_Sat_Sentinel_Overlay_Passaggio_W

            Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

            Dim Gis_Sat_Sentinel_Overlay_Passaggio_COD As Integer =
            objSequenze.NuovoId_Tabella(
                "Gis_Sat_Sentinel_Overlay_Passaggio",
                0,
                2000000000,
                objParametri)

            Scrittura.Scrivi(Gis_Sat_Sentinel_Overlay_Passaggio_COD, DataRiferimento, Tile, url, objParametri)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)


        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            '//////////////////////////////////////////////////////////////////////



            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try


        Return xRisp


    End Function
End Class



