Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDTOStd.InData.Notifiche
Imports Newtonsoft.Json

Public Class SistemiEsterni_RicezioneNotifiche_R
    Public Function LeggiElencoNotificheDaElaborare(Of T)(ByVal ID As Integer,
                                                          ByVal DataNotificaDa As DateTime,
                                                          ByRef ObjParametri As AgronicaCoreParametri,
                                                          Optional ByVal TipoOperazione As Integer = 0,
                                                          Optional ByVal Batchsize As Integer = 0,
                                                          Optional ByVal TagName As String = "",
                                                          Optional ByVal Anagrafica As Integer = 0,
                                                          Optional ByVal Terreni As Integer = 0,
                                                          Optional ByVal PCG As Integer = 0,
                                                          Optional ByVal PCG_Terreni As Integer = 0,
                                                          Optional ByVal Equipaggiamenti As Integer = 0,
                                                          Optional ByVal Lavoratori As Integer = 0,
                                                          Optional ByVal Gruppi_Appezzamenti As Integer = 0
                                                          ) As List(Of ObjNotifica(Of T))

        Dim messaggioErrore As String
        Dim DT As New DataTable
        Dim xResp As New List(Of ObjNotifica(Of T))
        Try
            Dim xReader As New AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_R

            If TagName <> "" Then
                If TipoOperazione = 0 Then
                    Throw New Exception("Specificare il tipooperazione quando di deve associare le notifiche ad un processo con TagName")
                End If
                If Batchsize = 0 Then
                    Throw New Exception("Specificare il numero di record quando di deve associare le notifiche ad un processo con TagName")
                End If

                Dim xWriter As New AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_W
                Select Case TipoOperazione
                    Case 1
                        'Creazione
                        xWriter.AssociaNotificheATagnameCreazione(TipoOperazione,
                                 Batchsize,
                                 TagName,
                                 Anagrafica,
                                 Terreni,
                                 PCG,
                                 PCG_Terreni,
                                 Equipaggiamenti,
                                 Lavoratori,
                                 Gruppi_Appezzamenti,
                                 ObjParametri,
                                 False)
                    Case 2
                        'Aggiornamento
                        xWriter.AssociaNotificheATagnameAggiornamenti(TipoOperazione,
                                                                 Batchsize,
                                                                 TagName,
                                                                 Anagrafica,
                                                                 Terreni,
                                                                 PCG,
                                                                 PCG_Terreni,
                                                                 Equipaggiamenti,
                                                                 Lavoratori,
                                                                 Gruppi_Appezzamenti,
                                                                 ObjParametri,
                                                                 False)
                    Case Else
                        Throw New Exception("Tipo operazione non mappato.")
                End Select

            End If

            DT = xReader.Leggi(ID,
                               DataNotificaDa,
                               0,
                               "Stato between 0 and 9",
                               "",
                               ObjParametri,
                               TagName,
                               Anagrafica,
                               Terreni,
                               PCG,
                               PCG_Terreni,
                               Equipaggiamenti,
                               Lavoratori,
                               TipoOperazione,
                               Gruppi_Appezzamenti)

            For Each row In DT.Rows
                xResp.Add(New ObjNotifica(Of T) With {
                            .id = row("ID"),
                            .stato = row("Stato"),
                            .nretry = row("N_Retry"),
                            .payload = JsonConvert.DeserializeObject(Of T)(row("Payload")),
                            .Priorita = If(row("Priorita") Is DBNull.Value, 0, row("Priorita"))
                })
            Next
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
        Return xResp
    End Function

    Public Function LeggiCodaNotificheDaElaborare(ByRef ObjParametri As AgronicaCoreParametri,
                                                  ByVal FiltroTipoOperazione_0Tutti_1Creazione_2Aggiornamento As Integer,
                                                  Optional ByVal cuaa As String = ""
                                                  ) As String

        Dim msgCoda As String = ""

        Try
            Dim xReader As New AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_R
            Dim DT As DataTable = xReader.Leggi(0,
                                                CostantiPersonalizzate.AGRODATAINIZIO,
                                                 0,
                                                 $"Stato BETWEEN 0 AND 9 AND Id_SistemaEsterno IN ({CInt(TipiEnumerativi.enum_SistemiEsterni.demetra)}, {CInt(TipiEnumerativi.enum_SistemiEsterni.NewAgri)})",
                                                 "",
                                                ObjParametri,
                                                 "",
                                                 0,
                                                 0,
                                                 0,
                                                 0,
                                                 0,
                                                 0,
                                                FiltroTipoOperazione_0Tutti_1Creazione_2Aggiornamento,
                                                0,
                                                cuaa)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                Dim listMsg As New List(Of String)

                If FiltroTipoOperazione_0Tutti_1Creazione_2Aggiornamento = 0 OrElse FiltroTipoOperazione_0Tutti_1Creazione_2Aggiornamento = 1 Then
                    Dim _wip = DT.Select("Stato <> 0 AND TipoOperazione = 'C'")
                    Dim countWIP As Integer = 0
                    If _wip IsNot Nothing AndAlso _wip.Any() Then
                        countWIP = _wip.Count()
                    End If
                    listMsg.Add(String.Format(AgronicaCoreDataProvider.My.Resources.Gias.XInserimentiInElaborazione, countWIP))

                    Dim _new = DT.Select("Stato = 0 And TipoOperazione = 'C'")
                    Dim countNew As Integer = 0
                    If _new IsNot Nothing AndAlso _new.Any() Then
                        countNew = _new.Count()
                    End If
                    listMsg.Add(String.Format(AgronicaCoreDataProvider.My.Resources.Gias.XInserimentiInCoda, countNew))
                End If

                If FiltroTipoOperazione_0Tutti_1Creazione_2Aggiornamento = 0 OrElse FiltroTipoOperazione_0Tutti_1Creazione_2Aggiornamento = 2 Then
                    Dim _wip = DT.Select("Stato <> 0 AND TipoOperazione = 'U'")
                    Dim countWIP As Integer = 0
                    If _wip IsNot Nothing AndAlso _wip.Any() Then
                        countWIP = _wip.Count()
                    End If
                    listMsg.Add(String.Format(AgronicaCoreDataProvider.My.Resources.Gias.XAggiornamentiInElaborazione, countWIP))

                    Dim _new = DT.Select("Stato = 0 AND TipoOperazione = 'U'")
                    Dim countNew As Integer = 0
                    If _new IsNot Nothing AndAlso _new.Any() Then
                        countNew = _new.Count()
                    End If
                    listMsg.Add(String.Format(AgronicaCoreDataProvider.My.Resources.Gias.XAggiornamentiInCoda, countNew))

                End If

                msgCoda = AgronicaCoreDataProvider.My.Resources.Gias.ImportPianiColturali + ": " + String.Join(", ", listMsg)
            End If

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

        Return msgCoda
    End Function

End Class
Public Class SistemiEsterni_RicezioneNotifiche_W
    Public Function RegistraNotificheCUAA(ByVal IdExtSys As Integer,
                                          ByVal elencoNotifiche As List(Of CUAAObj),
                                          ByRef objParametriServer As AgronicaCoreParametri) As RispostaStandard

        Dim messaggioErrore As String
        Dim DT As New DataTable
        Dim xResp As New RispostaStandard

        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Dim xWriter As New AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_W

        If elencoNotifiche.Count <= 0 Then
            Throw New Exception("Elenco notifiche vuoto")
        End If

        Try
            'AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
            '                                                                        FlagTransazioneLocale,
            '                                                                        objParametriServer)
            For Each notifica In elencoNotifiche
                xWriter.Scrivi(IdExtSys,
                               JsonConvert.SerializeObject(notifica),
                               notifica.CUAA,
                               notifica.Operazione,
                               If(notifica.dettaglio.anagrafica IsNot Nothing, 1, 0),
                               If(notifica.dettaglio.catasto IsNot Nothing, 1, 0),
                               If(notifica.dettaglio.pcg IsNot Nothing, 1, 0),
                               If(notifica.dettaglio.pcg_catasto IsNot Nothing, 1, 0),
                               If(notifica.dettaglio.equipaggiamenti IsNot Nothing, 1, 0),
                               If(notifica.dettaglio.lavoratori IsNot Nothing, 1, 0),
                               If(notifica.dettaglio.gruppi_appezzamenti IsNot Nothing, 1, 0),
                               notifica.Priorita,
                               objParametriServer)
            Next

            'AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametriServer)

            xResp.RispostaOK = True
        Catch ex As Exception
            'AgronicaCoreDataProvider.ConnessioniTransazioni.RollBackTransazione(objParametriServer)
            xResp.RispostaOK = False
            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
            xResp.Errore = messaggioErrore

            'Finally
            '    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale,
            '                                                                              objParametriServer)
        End Try

        Return xResp

    End Function

    Public Function AggiornaNotificheCUAA(ByVal elencoNotifiche As List(Of ObjNotifica(Of CUAAObj)),
                                          ByRef objParametriServer As AgronicaCoreParametri) As RispostaStandard

        Dim messaggioErrore As String
        Dim DT As New DataTable
        Dim xResp As New RispostaStandard

        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Dim xWriter As New AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_W

        If elencoNotifiche.Count <= 0 Then
            Throw New Exception("Elenco notifiche vuoto")
        End If

        Try
            'AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
            '                                                                        FlagTransazioneLocale,
            '                                                                        objParametriServer)
            For Each notifica In elencoNotifiche
                xWriter.Modifica(notifica.id, notifica.stato, notifica.nretry, "", objParametriServer)
            Next

            'AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametriServer)

            xResp.RispostaOK = True
        Catch ex As Exception
            'AgronicaCoreDataProvider.ConnessioniTransazioni.RollBackTransazione(objParametriServer)
            xResp.RispostaOK = False
            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
            xResp.Errore = messaggioErrore

            'Finally
            '    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale,
            '                                                                              objParametriServer)
        End Try

        Return xResp

    End Function

    Public Function AggiornaEsitoNotificaCUAA(ByVal notifica As ObjNotifica(Of CUAAObj),
                                          ByVal EsitoText As String,
                                          ByRef objParametriServer As AgronicaCoreParametri) As RispostaStandard

        Dim messaggioErrore As String
        Dim DT As New DataTable
        Dim xResp As New RispostaStandard

        Dim xWriter As New AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_W

        Try

            xResp.RispostaOK = xWriter.Modifica(notifica.id, notifica.stato, notifica.nretry, EsitoText, objParametriServer)
        Catch ex As Exception
            xResp.RispostaOK = False
            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
            xResp.Errore = messaggioErrore
        End Try

        Return xResp

    End Function

    Public Function AggiornaStatisticheNotificaNotificaCUAA(ByVal notifica As ObjNotifica(Of CUAAObj),
                                                                    ByVal Inizio_Anagrafica As DateTime,
                                                                    ByVal Fine_Anagrafica As DateTime,
                                                                    ByVal Inizio_Catasto As DateTime,
                                                                    ByVal Fine_Catasto As DateTime,
                                                                    ByVal Inizio_PCG As DateTime,
                                                                    ByVal Fine_PCG As DateTime,
                                                                    ByVal N_App As Integer,
                                                                    ByVal Inizio_Lavoratori As DateTime,
                                                                    ByVal Fine_Lavoratori As DateTime,
                                                                    ByVal Inizio_Equipaggiamenti As DateTime,
                                                                    ByVal Fine_Equipaggiamenti As DateTime,
                                                                    ByVal Inizio_Gruppi_Appezzamenti As DateTime,
                                                                    ByVal Fine_Gruppi_Appezzamenti As DateTime,
                                                                    ByRef objParametriServer As AgronicaCoreParametri,
                                                                    Optional ByVal LogError As Boolean = True) As RispostaStandard

        Dim messaggioErrore As String
        Dim DT As New DataTable
        Dim xResp As New RispostaStandard

        Dim xWriter As New AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_W

        Try

            xResp.RispostaOK = xWriter.AggiornaStatisticheNotifica(notifica.id,
                                                                   objParametriServer,
                                                                   Inizio_Anagrafica,
                                                                   Fine_Anagrafica,
                                                                   Inizio_Catasto,
                                                                   Fine_Catasto,
                                                                   Inizio_PCG,
                                                                   Fine_PCG,
                                                                   N_App,
                                                                   Inizio_Lavoratori,
                                                                   Fine_Lavoratori,
                                                                   Inizio_Equipaggiamenti,
                                                                   Fine_Equipaggiamenti,
                                                                   Inizio_Gruppi_Appezzamenti,
                                                                   Fine_Gruppi_Appezzamenti,
                                                                   LogError
                                                                   )


        Catch ex As Exception
            xResp.RispostaOK = False
            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
            xResp.Errore = messaggioErrore
        End Try

        Return xResp

    End Function
End Class
