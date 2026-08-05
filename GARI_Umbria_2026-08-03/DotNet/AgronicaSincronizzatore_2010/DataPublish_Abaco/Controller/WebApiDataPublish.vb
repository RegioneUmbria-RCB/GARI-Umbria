Imports System.Net.Http
Imports System.Web
Imports System.Text
Imports Newtonsoft.Json

Public Class WebApiDataPublish
    Inherits WebApiCaller
    Implements IDisposable

    Private Const ENDPOINT_ANAGRAFICA As String = "/anagrafica/"
    Private Const ENDPOINT_CATASTO As String = "/terreni/"
    Private Const ENDPOINT_PCG As String = "/pcg/"
    Private Const ENDPOINT_PCG_CASTATO As String = "/pcg-terreni/"
    Private Const ENDPOINT_AGGIORNAMENTO As String = "/aggiornamento/"
    Private Const ENDPOINT_ACKNOWLEDGE_SYNC As String = "/sync/"
    Private Const ENDPOINT_EQUIPAGGIAMENTI As String = "/equipaggiamenti/"
    Private Const ENDPOINT_LAVORATORI As String = "/lavoratori/"
    Private Const ENDPOINT_GRUPPI_APPEZZAMENTI As String = "/gruppi_appezzamenti/"

    Public Sub New(ByVal baseUrl As String, ByVal token As Tuple(Of String, String))
        MyBase.New(baseUrl, token, NameOf(WebApiDataPublish))
    End Sub

#Region "Creazione"

    Public Function ChiamaWSAnagrafica(ByVal cuaa As String) As Anagrafica
        Return ChiamaWS(Of Anagrafica)(HttpMethod.Get, Nothing, Nothing, "", "", getUrlEndpoint(ENDPOINT_ANAGRAFICA, cuaa))
    End Function
    Public Function ChiamaWSPianoColturaleGrafico(ByVal cuaa As String, ByVal campagna As String) As PianoColturaleGrafico
        Dim ret As New PianoColturaleGrafico
        Try
            Dim batchCall = ChiamaWS(Of PianoColturaleGrafico)(HttpMethod.Get, Nothing, Nothing, "", "", getUrlEndpoint(ENDPOINT_PCG, cuaa, campagna))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                Dim pars As New Dictionary(Of String, String)
                pars.Add("nextKey", batchCall.nextKey)
                batchCall = ChiamaWS(Of PianoColturaleGrafico)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_PCG, cuaa, campagna))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function
    Public Function ChiamaWSTerreni(ByVal cuaa As String) As Terreni
        Dim ret As New Terreni
        Try
            Dim batchCall = ChiamaWS(Of Terreni)(HttpMethod.Get, Nothing, Nothing, "", "", getUrlEndpoint(ENDPOINT_CATASTO, cuaa))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                Dim pars As New Dictionary(Of String, String)
                pars.Add("nextKey", batchCall.nextKey)
                batchCall = ChiamaWS(Of Terreni)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_CATASTO, cuaa))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try

        Return ret
    End Function
    Public Function ChiamaWSAppezzamentiTerreni(ByVal cuaa As String, ByVal campagna As String) As AppezzamentiTerreni
        Dim ret As New AppezzamentiTerreni
        Try
            Dim batchCall = ChiamaWS(Of AppezzamentiTerreni)(HttpMethod.Get, Nothing, Nothing, "", "", getUrlEndpoint(ENDPOINT_PCG_CASTATO, cuaa, campagna))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                Dim pars As New Dictionary(Of String, String)
                pars.Add("nextKey", batchCall.nextKey)
                batchCall = ChiamaWS(Of AppezzamentiTerreni)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_PCG_CASTATO, cuaa, campagna))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ChiamaWSNotificaOK(ByVal id_signal As String, ByVal esito As SyncAcknowledge) As SyncAcknowledgeResponse
        Dim ret As SyncAcknowledgeResponse = Nothing
        Try
            Return ChiamaWS(Of SyncAcknowledgeResponse)(HttpMethod.Put, Nothing, Nothing, JsonConvert.SerializeObject(esito), "application/json", getUrlEndpoint(ENDPOINT_ACKNOWLEDGE_SYNC, id_signal))
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ChiamaWSEquipaggiamenti(ByVal cuaa As String) As EquipaggiamentiCUAA
        Dim ret As New EquipaggiamentiCUAA
        Try
            Dim batchCall = ChiamaWS(Of EquipaggiamentiCUAA)(HttpMethod.Get, Nothing, Nothing, "", "", getUrlEndpoint(ENDPOINT_EQUIPAGGIAMENTI, cuaa))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                Dim pars As New Dictionary(Of String, String)
                pars.Add("nextKey", batchCall.nextKey)
                batchCall = ChiamaWS(Of EquipaggiamentiCUAA)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_EQUIPAGGIAMENTI, cuaa))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret

    End Function

    Public Function ChiamaWSLavoratori(ByVal cuaa As String) As LavoratoriCUAA
        Dim ret As New LavoratoriCUAA
        Try
            Dim batchCall = ChiamaWS(Of LavoratoriCUAA)(HttpMethod.Get, Nothing, Nothing, "", "", getUrlEndpoint(ENDPOINT_LAVORATORI, cuaa))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                Dim pars As New Dictionary(Of String, String)
                pars.Add("nextKey", batchCall.nextKey)
                batchCall = ChiamaWS(Of LavoratoriCUAA)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_LAVORATORI, cuaa))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret

    End Function

    Public Function ChiamaWSGruppiAppezzamenti(ByVal cuaa As String) As GruppoAppezzamentoWS
        Dim ret As New GruppoAppezzamentoWS
        Try
            Dim batchCall = ChiamaWS(Of GruppoAppezzamentoWS)(HttpMethod.Get, Nothing, Nothing, "", "", getUrlEndpoint(ENDPOINT_GRUPPI_APPEZZAMENTI, cuaa))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                Dim pars As New Dictionary(Of String, String)
                pars.Add("nextKey", batchCall.nextKey)
                batchCall = ChiamaWS(Of GruppoAppezzamentoWS)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_GRUPPI_APPEZZAMENTI, cuaa))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret

    End Function


#End Region

#Region "Modifiche"
    Public Function ChiamaWSVariazioniAnagrafica(ByVal cuaa As String, ByVal dataRif As Date) As AnagraficaChangeLog
        Dim ret As New AnagraficaChangeLog
        Dim wRet As New List(Of Anagrafica)
        Try
            Dim pars As New Dictionary(Of String, String)
            SetParsKeyValue("ref_date", dataRif.ToString("yyyy-MM-dd"), pars)
            Dim batchCall = ChiamaWS(Of AnagraficaChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_ANAGRAFICA, "", "", ENDPOINT_AGGIORNAMENTO))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                SetParsKeyValue("nextKey", batchCall.nextKey, pars)
                batchCall = ChiamaWS(Of AnagraficaChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_ANAGRAFICA, "", "", ENDPOINT_AGGIORNAMENTO))
                ret.records.AddRange(batchCall.records)
            End While
            wRet = ret.records.Where(Function(x) x.cuaa = cuaa).ToList()
            If wRet IsNot Nothing AndAlso wRet.Count > 0 Then
                ret.records = wRet
            End If
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ChiamaWSVariazioniCatasto(ByVal cuaa As String, ByVal dataRif As DateTime) As TerreniChangeLog
        Dim ret As New TerreniChangeLog
        Try
            Dim pars As New Dictionary(Of String, String)
            SetParsKeyValue("ref_timestamp", dataRif.ToString("yyyy-MM-ddThh:mm:ssZ"), pars)
            Dim batchCall = ChiamaWS(Of TerreniChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_CATASTO, cuaa, "", ENDPOINT_AGGIORNAMENTO))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                SetParsKeyValue("nextKey", batchCall.nextKey, pars)
                batchCall = ChiamaWS(Of TerreniChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_CATASTO, cuaa, "", ENDPOINT_AGGIORNAMENTO))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ChiamaWSVariazioniPCGTotale(ByVal dataRif As Date) As PianoColturaleGraficoChangeLog
        Dim ret As New PianoColturaleGraficoChangeLog
        Try
            Dim pars As New Dictionary(Of String, String)
            SetParsKeyValue("ref_date", dataRif.ToString("yyyy-MM-dd"), pars)
            Dim batchCall = ChiamaWS(Of PianoColturaleGraficoChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_PCG, "", "", ENDPOINT_AGGIORNAMENTO))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                SetParsKeyValue("nextKey", batchCall.nextKey, pars)
                batchCall = ChiamaWS(Of PianoColturaleGraficoChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_PCG, "", "", ENDPOINT_AGGIORNAMENTO))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ChiamaWSVariazioniPCG_CUAA(ByVal cuaa As String, ByVal Campagna As Integer, ByVal dataRif As DateTime) As PianoColturaleGraficoChangeLog
        Dim ret As New PianoColturaleGraficoChangeLog
        Try
            Dim pars As New Dictionary(Of String, String)
            SetParsKeyValue("ref_timestamp", dataRif.ToString("yyyy-MM-ddTHH:mm:ssZ"), pars)
            SetParsKeyValue("campaign", Campagna, pars)
            Dim batchCall = ChiamaWS(Of PianoColturaleGraficoChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_PCG, cuaa, "", ENDPOINT_AGGIORNAMENTO))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                SetParsKeyValue("nextKey", batchCall.nextKey, pars)
                batchCall = ChiamaWS(Of PianoColturaleGraficoChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_PCG, cuaa, "", ENDPOINT_AGGIORNAMENTO))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ChiamaWSVariazioniPCGCatasto_Totale(ByVal dataRif As Date) As AppezzamentoTerrenoChangeLog
        Dim ret As New AppezzamentoTerrenoChangeLog
        Try
            Dim pars As New Dictionary(Of String, String)
            SetParsKeyValue("ref_date", dataRif.ToString("yyyy-MM-dd"), pars)
            Dim batchCall = ChiamaWS(Of AppezzamentoTerrenoChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_PCG_CASTATO, "", "", ENDPOINT_AGGIORNAMENTO))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                SetParsKeyValue("nextKey", batchCall.nextKey, pars)
                batchCall = ChiamaWS(Of AppezzamentoTerrenoChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_PCG_CASTATO, "", "", ENDPOINT_AGGIORNAMENTO))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ChiamaWSVariazioniPCGCatasto_CUAA(ByVal cuaa As String, ByVal Campagna As Integer, ByVal dataRif As DateTime) As AppezzamentoTerrenoChangeLog
        Dim ret As New AppezzamentoTerrenoChangeLog
        Try
            Dim pars As New Dictionary(Of String, String)
            SetParsKeyValue("ref_timestamp", dataRif.ToString("yyyy-MM-ddTHH:mm:ssZ"), pars)
            SetParsKeyValue("campaign", Campagna, pars)
            Dim batchCall = ChiamaWS(Of AppezzamentoTerrenoChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_PCG_CASTATO, cuaa, "", ENDPOINT_AGGIORNAMENTO))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                SetParsKeyValue("nextKey", batchCall.nextKey, pars)
                batchCall = ChiamaWS(Of AppezzamentoTerrenoChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_PCG_CASTATO, cuaa, "", ENDPOINT_AGGIORNAMENTO))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ChiamaWSVariazioniEquipaggiamenti_CUAA(ByVal cuaa As String, ByVal dataRif As DateTime) As EquipaggiamentoCUAAChangeLog
        Dim ret As New EquipaggiamentoCUAAChangeLog
        Try
            Dim pars As New Dictionary(Of String, String)
            SetParsKeyValue("ref_timestamp", dataRif.ToString("yyyy-MM-ddTHH:mm:ssZ"), pars)
            Dim batchCall = ChiamaWS(Of EquipaggiamentoCUAAChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_EQUIPAGGIAMENTI, cuaa, "", ENDPOINT_AGGIORNAMENTO))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                SetParsKeyValue("nextKey", batchCall.nextKey, pars)
                batchCall = ChiamaWS(Of EquipaggiamentoCUAAChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_EQUIPAGGIAMENTI, cuaa, "", ENDPOINT_AGGIORNAMENTO))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ChiamaWSVariazioniLavoratori_CUAA(ByVal cuaa As String, ByVal dataRif As DateTime) As LavoratoriCUAAChangeLog
        Dim ret As New LavoratoriCUAAChangeLog
        Try
            Dim pars As New Dictionary(Of String, String)
            SetParsKeyValue("ref_timestamp", dataRif.ToString("yyyy-MM-ddTHH:mm:ssZ"), pars)
            Dim batchCall = ChiamaWS(Of LavoratoriCUAAChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_LAVORATORI, cuaa, "", ENDPOINT_AGGIORNAMENTO))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                SetParsKeyValue("nextKey", batchCall.nextKey, pars)
                batchCall = ChiamaWS(Of LavoratoriCUAAChangeLog)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_LAVORATORI, cuaa, "", ENDPOINT_AGGIORNAMENTO))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function


    Public Function ChiamaWSVariazioniGruppiAppezzamenti(ByVal cuaa As String, ByVal dataRif As DateTime) As GruppoAppezzamentoWS
        Dim ret As New GruppoAppezzamentoWS
        Try
            Dim pars As New Dictionary(Of String, String)
            SetParsKeyValue("ref_timestamp", dataRif.ToString("yyyy-MM-ddTHH:mm:ssZ"), pars)
            Dim batchCall = ChiamaWS(Of GruppoAppezzamentoWS)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_GRUPPI_APPEZZAMENTI, cuaa, "", ENDPOINT_AGGIORNAMENTO))
            ret.records.AddRange(batchCall.records)
            While batchCall.nextKey <> ""
                SetParsKeyValue("nextKey", batchCall.nextKey, pars)
                batchCall = ChiamaWS(Of GruppoAppezzamentoWS)(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_GRUPPI_APPEZZAMENTI, cuaa, "", ENDPOINT_AGGIORNAMENTO))
                ret.records.AddRange(batchCall.records)
            End While
        Catch ex As DataPublishException
            ret = Nothing
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret

    End Function


#End Region

    Private Sub SetParsKeyValue(ByVal keyName As String, ByVal keyValue As String, ByRef pars As Dictionary(Of String, String))
        If Not pars.ContainsKey(keyName) Then
            pars.Add(keyName, keyValue)
        Else
            pars(keyName) = keyValue
        End If
    End Sub

    Private Function getUrlEndpoint(ByVal Endpoint As String, ByVal cuaa As String, Optional ByVal campagna As String = "", Optional ByVal Aggiornamento As String = "") As String
        Dim r As String = Endpoint
        r &= If(cuaa <> "", cuaa, "")
        r &= If(Aggiornamento <> "", Aggiornamento, "")
        r &= If(campagna <> "", "/" & campagna, "")
        Return r
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        MyBase.Dispose()
    End Sub
End Class

Public Class AnagraficaReq
    Public Property cuaa As String
End Class

Public Class AnagraficaChangeLogReq
    Public Property ref_date As String
    Public Property nextKey As String
End Class



