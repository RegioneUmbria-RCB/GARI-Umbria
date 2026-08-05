Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDTOStd.InData.Shared.GridDto
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports AgronicaCoreUtility

Public Class VisteGrigliaBizService
    Inherits LogProvider

    Public Function CaricaViste(visteChiave As ChiaveVista, objP_utenti As String) As rispostaStandard(Of List(Of Vista))
        Dim utentiDBContext As AgronicaCoreParametri = Nothing
        Try
            utentiDBContext = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim visteDataLayer As New VisteGrigliaDALService()

            Dim dt As DataTable = visteDataLayer.CaricaViste("", "", utentiDBContext, visteChiave)

            Dim risultati As List(Of Vista) = TransformaDataTable(dt)

            Return RicavaRisultatoInFormatoJson(risultati)
        Catch ex As Exception
            If utentiDBContext IsNot Nothing Then
                Dim routine As String = Reflection.MethodBase.GetCurrentMethod().Name
                Scrivi_LOG(utentiDBContext, routine, ex.Message)
            End If

            Return RispostaStandardGenericModule.EccezioneIncontrata(Of List(Of Vista))(ex)
        End Try
    End Function

    Private Function TransformaDataTable(dt As DataTable) As List(Of Vista)
        Dim risultato As List(Of Vista) = (From dr In dt.Rows Select New Vista() With {
            .IdVista = dr("IdVista"),
            .Colonne = dr("ColonneJson"),
            .NomeUtente = dr("NomeUtente"),
            .NomeVista = dr("NomeVista"),
            .Stato = dr("StatoJson"),
            .GridId = dr("GridId"),
            .Predefinita = dr("Predefinita"),
            .FiltroJSON = dr("FiltroJSON").ToString,
            .FlagPubblica = dr("FlagPubblica")
        }).ToList()
        Return risultato
    End Function

    Private Function RicavaRisultatoInFormatoJson(Optional viste As List(Of Vista) = Nothing) As rispostaStandard(Of List(Of Vista))
        Dim risp As New rispostaStandard(Of List(Of Vista))

        Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
        risp.RispostaOK = True
        If viste IsNot Nothing Then
            risp.RispostaStringa = viste
        End If

        Return risp
    End Function


    Public Function ScriviViste(viste As SalvaVisteWrapper, objP_utenti As String) As RispostaStandard
        Dim utentiDBContext As AgronicaCoreParametri = Nothing
        Using scope As New TransactionScope()
            Try
                utentiDBContext = Utility.convertStringtoOBJparametri(objP_utenti)

                Dim visteDataLayer As New VisteGrigliaDALService()

                For Each vista In viste.VisteNuove
                    Varie.SanitizeTesto_MantieniVirgoletteECaratteriAccentati(vista.NomeVista)
                Next

                visteDataLayer.ScriviViste(viste, utentiDBContext)

                scope.Complete()
                scope.Dispose()

                Return RicavaRisultatoInFormatoJson()
            Catch ex As Exception
                scope.Dispose()
                If utentiDBContext IsNot Nothing Then
                    Dim routine As String = Reflection.MethodBase.GetCurrentMethod().Name
                    Scrivi_LOG(utentiDBContext, routine, ex.Message)
                End If

                Return RispostaStandardModule.EccezioneIncontrata(ex)
            End Try
        End Using
    End Function

    Public Function CancellaVista(vista As ChiaveVista, objP_utenti As String) As RispostaStandard
        Dim utentiDBContext As AgronicaCoreParametri = Nothing
        Try
            utentiDBContext = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim visteDataLayer As New VisteGrigliaDALService()
            visteDataLayer.CancellaVista(vista, utentiDBContext)

            Return RicavaRisultatoInFormatoJson()
        Catch ex As Exception
            If utentiDBContext IsNot Nothing Then
                Dim routine As String = Reflection.MethodBase.GetCurrentMethod().Name
                Scrivi_LOG(utentiDBContext, routine, ex.Message)
            End If

            Return RispostaStandardModule.EccezioneIncontrata(ex)
        End Try
    End Function

    Private Function RicavaRisultatoInFormatoJson() As RispostaStandard
        Dim risp As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
        risp.RispostaOK = True
        risp.RispostaStringa = "Operazione riuscita"

        Return risp
    End Function
End Class

Module RispostaStandardModule
    Public Function EccezioneIncontrata(ex As Exception) As RispostaStandard
        Dim risp As New RispostaStandard
        risp.RispostaOK = False
        risp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Return risp
    End Function
End Module

Module RispostaStandardGenericModule
    Public Function EccezioneIncontrata(Of T)(ex As Exception) As rispostaStandard(Of T)
        Dim risp As New rispostaStandard(Of T)
        risp.RispostaOK = False
        risp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Return risp
    End Function
End Module