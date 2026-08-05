Imports System.Text
Imports Agronica.Helpers.TFS
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCorePerformaDAL
Imports AgronicaCoreVarieBIZ

Public Class GestoreCommesseLotti


    Public Function SincronizzaPerformaTFS(inData As SincronizzaPerformaTFSIn) As RispostaStandard
        Dim r As New RispostaStandard

        Try


            Dim rvalTfsRiportoTempiPadri As RispostaStandard =
                TfsRiportoTempiPadri(inData)


            Dim rvalPerformaTfsScambioDati As RispostaStandard =
                PerformaTfsScambioDati(inData)


            ''versione precedenti con riporto tempi
            'Dim rvalRiportoTempi As RispostaStandard =
            '    PerformaRiportoTempiDaWorkItems(inData)

            r.RispostaOK = (rvalPerformaTfsScambioDati.RispostaOK And rvalTfsRiportoTempiPadri.RispostaOK)
            r.RispostaStringa &= " " & rvalPerformaTfsScambioDati.RispostaStringa & " " & rvalTfsRiportoTempiPadri.RispostaStringa
            r.Errore &= rvalPerformaTfsScambioDati.Errore & " " & rvalTfsRiportoTempiPadri.Errore

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    Private Function TfsRiportoTempiPadri(inData As SincronizzaPerformaTFSIn) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim stbOk As New StringBuilder
            Dim stbErr As New StringBuilder

            Dim messaggioBaseRiportoTempiPadre As String =
                "Esito Riporto dei tempi Funzionalità/Attività per commessa/lotto con codice: "

            If inData.impostaSommatoriaTempiSuElementoPadre Then

                Dim tfsHelper As New TFSConnector(inData.vstsCollectionUrl)

                'Per ciascun lotto richiesto

                Dim oLetturaLottiDaAggiornare As New GestoreCommesseLottiDAL_R
                Dim rValLotti As rispostaStandard(Of DataTable) = PerformaTfsScambioDatiLetturaListaLotti(inData, oLetturaLottiDaAggiornare)
                If Not rValLotti.RispostaOK Then
                    Throw New Exception(rValLotti.Errore)
                End If

                Dim dtLottiDaAggiornare As DataTable =
                    rValLotti.RispostaStringa

                Dim giveFeedback As Boolean

                'Esegue una query TEAM per ciascuna commessa/lotto per recuperare i workItem e leggere i tempi
                For Each riga As DataRow In dtLottiDaAggiornare.Rows


                    'Query Team e calcolo totale delle ore, commessa/lotto per volta
                    Dim inDataSomma As New TFSConnector.ConfigurazioneWIQL
                    inData.cfgSincroOre.listaFiltri.Find(Function(o) o.campo = "Commessa").valore = riga("CodCommessa")
                    inData.cfgSincroOre.listaFiltri.Find(Function(o) o.campo = "Task").valore = riga("CodSottocommessa")



                    'Lettura funzionalità associata ai lotti di performa da gestire
                    Dim ListaFunzionalita As rispostaStandard(Of List(Of TFSConnector.SmallWorkItem)) =
                        tfsHelper.ListaWorkItems(0, inData.cfgSincroOre, TFSConnector.TipoEstrazione.Funzionalita)


                    'estrazione dei figli per ciascun elemento
                    For Each SingoloItemFunzionalita In ListaFunzionalita.RispostaStringa


                        Dim MessaggioErrore As String = ""
                        Dim rval As Boolean = True
                        Dim SommatoriaStimaOriginaleElementiFigli As Double = 0
                        Dim SommatoriaOreRimanentiElementiFigli As Double = 0
                        giveFeedback = False
                        Try

                            'Leggo la gearchia e riporto i work items
                            Dim ListaElementiFigli As rispostaStandard(Of List(Of TFSConnector.SmallWorkItem)) =
                            tfsHelper.ListaWorkItems(SingoloItemFunzionalita.id, inData.cfgSincroOre, TFSConnector.TipoEstrazione.Gerarchia)

                            'sommatoria delle ore e riporto
                            SommatoriaStimaOriginaleElementiFigli = ListaElementiFigli.RispostaStringa.Sum(Function(item) item.StimaOriginale)
                            If SommatoriaStimaOriginaleElementiFigli <> SingoloItemFunzionalita.StimaOriginale Then
                                giveFeedback = True
                                TfsElaboraSommatoriaFigliRiportaPadre(SingoloItemFunzionalita.id, "Microsoft.VSTS.Scheduling.OriginalEstimate", SommatoriaStimaOriginaleElementiFigli, tfsHelper)
                            End If

                            SommatoriaOreRimanentiElementiFigli = ListaElementiFigli.RispostaStringa.Sum(Function(item) item.OreRimanenti)
                            If SommatoriaOreRimanentiElementiFigli <> SingoloItemFunzionalita.OreRimanenti Then
                                giveFeedback = True
                                TfsElaboraSommatoriaFigliRiportaPadre(SingoloItemFunzionalita.id, "Microsoft.VSTS.Scheduling.RemainingWork", SommatoriaOreRimanentiElementiFigli, tfsHelper)
                            End If


                        Catch ex As Exception
                            rval = False
                            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                        End Try

                        If giveFeedback Then
                            PerformaRiportoTempiFeedBack(messaggioBaseRiportoTempiPadre, rval, MessaggioErrore, stbOk, stbErr, riga, SommatoriaStimaOriginaleElementiFigli.ToString & " - " & SommatoriaOreRimanentiElementiFigli.ToString, SingoloItemFunzionalita.id, TipoFeedback.RiportoTempiFunzionalitaAttivita)
                        End If

                    Next
                    'SingoloItemFunzionalita

                Next
                'Commessa/lotto su datatable

                r.RispostaOK = True
                r.RispostaStringa = stbOk.ToString
                r.Errore = stbErr.ToString

            Else

                r.RispostaOK = True
                r.RispostaStringa = "Nessuna elaborazione richiesta per Riporto dei tempi Funzionalità/Attività"

            End If


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r

    End Function

    Private Function PerformaTfsScambioDatiLetturaListaLotti(inData As SincronizzaPerformaTFSIn, ByVal oLetturaLottiDaAggiornare As GestoreCommesseLottiDAL_R) As rispostaStandard(Of DataTable)

        Dim r As New rispostaStandard(Of DataTable)
        Try

            'Leggi da performa i dati per il riporto

            Dim dtLottiDaAggiornare As DataTable =
                oLetturaLottiDaAggiornare.LeggiListaLotti(inData.queryLottiXFiltroAggiuntivo, inData.queryLottiXOrderBy, inData.objParametri_Performa)

            r.RispostaStringa = dtLottiDaAggiornare
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True)
        Finally

        End Try

        Return r

    End Function

    Private Function PerformaTfsScambioDati(inData As SincronizzaPerformaTFSIn) As RispostaStandard

        Dim r As New RispostaStandard

        Try


            Dim messaggioBaseGiornateLotto As String =
                "Esito Sincro delle giornate per commessa con codice: "

            Dim messaggioBaseGiornateIndividuali As String =
                "Esito Sincro delle giornate individuali con codice: "

            Dim messaggioBaseAggiornaTFS As String =
                "Esito Aggiornamento delle priorità per id: "

            Dim tfsHelper As New TFSConnector(inData.vstsCollectionUrl)


            Dim PerformaUpdater As New GestoreCommesseLottiDAL_W

            Dim stbOk As New StringBuilder
            Dim stbErr As New StringBuilder

            Dim oLetturaLottiDaAggiornare As New GestoreCommesseLottiDAL_R
            Dim rValLotti As rispostaStandard(Of DataTable) = PerformaTfsScambioDatiLetturaListaLotti(inData, oLetturaLottiDaAggiornare)
            If Not rValLotti.RispostaOK Then
                Throw New Exception(rValLotti.Errore)
            End If

            Dim dtLottiDaAggiornare As DataTable =
                rValLotti.RispostaStringa

            'Esegue una query TEAM per ciascuna commessa/lotto per recuperare i workItem e leggere i tempi
            For Each riga As DataRow In dtLottiDaAggiornare.Rows

                'Query Team e calcolo totale delle ore, commessa/lotto per volta
                Dim inDataSomma As New TFSConnector.ConfigurazioneWIQL
                inData.cfgSincroOre.listaFiltri.Find(Function(o) o.campo = "Commessa").valore = riga("CodCommessa")
                inData.cfgSincroOre.listaFiltri.Find(Function(o) o.campo = "Task").valore = riga("CodSottocommessa")

                Dim listaWorkItemXLotto As rispostaStandard(Of List(Of TFSConnector.SmallWorkItem)) =
                    tfsHelper.ListaWorkItems(0, inData.cfgSincroOre, TFSConnector.TipoEstrazione.Attivita)

                If inData.elaboraSommatorieOrePerforma Then
                    PerformaTfsScambioDatiAggiornaGiornateTotaliSuDBPerforma(
                    messaggioBaseGiornateLotto,
                    inData,
                    r,
                    PerformaUpdater,
                    stbOk,
                    stbErr,
                    riga,
                    listaWorkItemXLotto)
                End If


                If inData.impostaOrdinePrioritaSuTFS Then
                    PerformaTfsScambioDatiAggiornaPriorityTfsDaPerforma(
                    messaggioBaseAggiornaTFS,
                    inData,
                    r,
                    tfsHelper,
                    stbOk,
                    stbErr,
                    riga,
                    listaWorkItemXLotto)
                End If


                If inData.elaboraSommatorieOrePerforma Then
                    PerformaTfsScambioDatiAggiornaGiornateIndividualiLottoSuDBPerforma(
                    oLetturaLottiDaAggiornare,
                    PerformaUpdater,
                    messaggioBaseGiornateIndividuali,
                    inData,
                    r,
                    stbOk,
                    stbErr,
                    riga,
                    listaWorkItemXLotto)
                End If

            Next
            'lotto in performa

            r.RispostaStringa &= stbOk.ToString
            r.Errore &= stbErr.ToString

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r

    End Function

    Private Sub TfsElaboraSommatoriaFigliRiportaPadre(idElemenoDaScrivere As Integer, CampoDaScrivere As String, valore As Double, tfsUpdater As TFSConnector)
        TFSTestUpdateWorkItem(CampoDaScrivere, idElemenoDaScrivere, valore, tfsUpdater)
    End Sub


    Private Sub TFSTestUpdateWorkItem(campo As String, id As Integer, valore As Double, tfsUpdater As TFSConnector)

        Dim inData1 As TFSConnector.AggiornaCampiWorkItemIn = New TFSConnector.AggiornaCampiWorkItemIn With {
                .id = id,
                .listaCampiDaAggiornare = New List(Of TFSConnector.ConfigurazioneWIQLCampo)({
                    New TFSConnector.ConfigurazioneWIQLCampo With {
                        .campo = campo,
                        .valore = valore,
                        .valoreTrovatoSuTFS = True
                    }
                })
            }

        Dim rvalAggiorna As RispostaStandard =
            tfsUpdater.AggiornaCampiWorkItem(inData1)

    End Sub

    Private Sub PerformaTfsScambioDatiAggiornaPriorityTfsDaPerforma(messaggioBase As String, inData As SincronizzaPerformaTFSIn, rvalComplessivo As RispostaStandard, tfsUpdater As TFSConnector, stbOk As StringBuilder, stbErr As StringBuilder, riga As DataRow, listaWorkItemXLotto As rispostaStandard(Of List(Of TFSConnector.SmallWorkItem)))


        For Each ww As TFSConnector.SmallWorkItem In listaWorkItemXLotto.RispostaStringa

            Dim priorityDaAggiornare As Integer = riga("priorita")

            If priorityDaAggiornare > 0 AndAlso (Not ww.Trovato_OrdineDiPriorita OrElse ww.OrdineDiPriorita <> priorityDaAggiornare) Then

                Dim inData1 As TFSConnector.AggiornaCampiWorkItemIn = New TFSConnector.AggiornaCampiWorkItemIn With {
                        .id = ww.id,
                        .listaCampiDaAggiornare = New List(Of TFSConnector.ConfigurazioneWIQLCampo)({
                            New TFSConnector.ConfigurazioneWIQLCampo With {
                                .campo = inData.cfgSincroPriority.campoPerAggregazioni,
                                .valore = priorityDaAggiornare,
                                .valoreTrovatoSuTFS = ww.Trovato_OrdineDiPriorita
                            }
                        })
                    }

                Dim rvalAggiorna As RispostaStandard =
                    tfsUpdater.AggiornaCampiWorkItem(inData1)

                PerformaRiportoTempiFeedBack(messaggioBase, rvalAggiorna.RispostaOK, rvalAggiorna.Errore, stbOk, stbErr, riga, priorityDaAggiornare, ww.id, TipoFeedback.PrioritaImpostata)

            End If

        Next

    End Sub

    Private Sub PerformaTfsScambioDatiAggiornaGiornateIndividualiLottoSuDBPerforma(
        oLetturaLottiDaAggiornare As GestoreCommesseLottiDAL_R,
        PerformaUpdater As GestoreCommesseLottiDAL_W,
        messaggioBase As String, inData As SincronizzaPerformaTFSIn, rvalComplessivo As RispostaStandard, stbOk As StringBuilder, stbErr As StringBuilder, riga As DataRow, listaWorkItemXLotto As rispostaStandard(Of List(Of TFSConnector.SmallWorkItem)))

        'leggo le assegnazioni per il lotto
        Dim dtAssegnazioni As DataTable =
            oLetturaLottiDaAggiornare.LeggiListaAssegnazioniLotto(riga("CodCommessa"), riga("CodSottoCommessa"), "", "", inData.objParametri_Performa)

        'aggiorno i tempi sulle assegnazioni individuali di ciascuno
        For Each AssegnatarioPerforma As DataRow In dtAssegnazioni.Rows

            Dim rval As Boolean
            Dim messaggioErrore As String = ""
            Dim sOreTotali As String = "-1"

            Try
                If AssegnatarioPerforma("email") = "" Then
                    Throw New Exception("Nessuna email trovata per " & AssegnatarioPerforma("UtenteAssegnato"))
                End If
                Dim sommaOreAssegnatario As Double =
                    SommaOreDaEmail(AssegnatarioPerforma("email"), listaWorkItemXLotto)

                Dim ggAssegnatario As Double = Math.Round(sommaOreAssegnatario / 8, 0)
                sOreTotali = ggAssegnatario.ToString

                rval = AggiornaParametroDocumentoPerforma(
                    emum_Manager_Performa_DocCommesse_Parametri.Dati_Sez_Sottocommesse_PM_gg_a_FinireTaskXUtente, enum_Manager_TipiDatiSQLVariant.system_Real,
                    inData, PerformaUpdater, AssegnatarioPerforma, sOreTotali)

            Catch ex As Exception
                rval = False
                MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            End Try

            rvalComplessivo.RispostaOK = (rvalComplessivo.RispostaOK And rval)

            PerformaRiportoTempiFeedBack(messaggioBase, rval, messaggioErrore, stbOk, stbErr, AssegnatarioPerforma, sOreTotali, 0, TipoFeedback.TotaleGiornateResidue)


        Next
        'assegnatario

    End Sub

    Private Function SommaOreDaEmail(email As String, listaWorkItemXLotto As rispostaStandard(Of List(Of TFSConnector.SmallWorkItem))) As Double

        Dim listaXUtente As List(Of TFSConnector.SmallWorkItem) = (
            From ll In listaWorkItemXLotto.RispostaStringa
            Where ll.AssegnatoA_email = email
        ).ToList()

        Dim somma As Double = listaXUtente.Sum(Function(o) o.OreRimanenti)
        Return somma
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="messaggioBase"></param>
    ''' <param name="inData"></param>
    ''' <param name="rvalComplessivo"></param>
    ''' <param name="PerformaUpdater"></param>
    ''' <param name="stbOk"></param>
    ''' <param name="stbErr"></param>
    ''' <param name="riga"></param>
    ''' <param name="listaWorkItemXLotto"></param>
    Private Shared Sub PerformaTfsScambioDatiAggiornaGiornateTotaliSuDBPerforma(messaggioBase As String, inData As SincronizzaPerformaTFSIn, rvalComplessivo As RispostaStandard, PerformaUpdater As GestoreCommesseLottiDAL_W, stbOk As StringBuilder, stbErr As StringBuilder, riga As DataRow, listaWorkItemXLotto As rispostaStandard(Of List(Of TFSConnector.SmallWorkItem)))

        'converto le ore in giornate
        Dim somatoriaOreRimanentiLotto As Double = listaWorkItemXLotto.RispostaStringa.Sum(Function(o) o.OreRimanenti)
        somatoriaOreRimanentiLotto = Math.Round(somatoriaOreRimanentiLotto / 8, 0)

        'update del dato sul peforma (gg a finire intero task)
        Dim sOreTotali As String = somatoriaOreRimanentiLotto.ToString()
        Dim rval As Boolean
        Dim MessaggioErrore As String = ""
        Try

            If Not listaWorkItemXLotto.RispostaOK Then
                Throw New Exception(listaWorkItemXLotto.Errore)
            End If

            rval = AggiornaParametroDocumentoPerforma(
                emum_Manager_Performa_DocCommesse_Parametri.Dati_Sez_Sottocommesse_PM_gg_a_FinireTask, enum_Manager_TipiDatiSQLVariant.system_Real,
                inData, PerformaUpdater, riga, sOreTotali)

        Catch ex As Exception
            rval = False
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        rvalComplessivo.RispostaOK = (rvalComplessivo.RispostaOK And rval)

        PerformaRiportoTempiFeedBack(messaggioBase, rval, MessaggioErrore, stbOk, stbErr, riga, sOreTotali, 0, TipoFeedback.TotaleGiornateResidue)

    End Sub



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="IDParametroModelloDocumento">es: emum_Manager_Performa_DocCommesse_Parametri.Dati_Sez_Sottocommesse_PM_gg_a_FinireTask</param>
    ''' <param name="tipoDato">es: enum_Manager_TipiDatiSQLVariant.system_Real</param>
    ''' <param name="inData"></param>
    ''' <param name="PerformaUpdater"></param>
    ''' <param name="riga"></param>
    ''' <param name="valoreParametro"></param>
    ''' <returns></returns>
    Private Shared Function AggiornaParametroDocumentoPerforma(IDParametroModelloDocumento As Integer, tipoDato As enum_Manager_TipiDatiSQLVariant, inData As SincronizzaPerformaTFSIn, PerformaUpdater As GestoreCommesseLottiDAL_W, riga As DataRow, valoreParametro As String) As Boolean
        Return PerformaUpdater.AggiornaParametroDocumentoPerforma(
                IDDocumento:=riga("IDDocumento"),
                IDParametroModelloDocumento:=IDParametroModelloDocumento,
                IDRiga:=riga("IDRiga"),
                Progressivo:=riga("Progressivo"),
                valoreParametro,
                tipoDato,
                TabellaDD:="DDParametriDocumentiDefault",
                inData.objParametri_Performa
            )
    End Function

    Public Function PerformaRiportoTempiDaWorkItems(inData As SincronizzaPerformaTFSIn) As RispostaStandard
        Dim r As New RispostaStandard

        Try


            Dim tfsHelper As New TFSConnector(inData.vstsCollectionUrl)

            'Leggi da performa i dati per il riporto
            Dim oLottiDaAggiornare As New AgronicaCorePerformaDAL.GestoreCommesseLottiDAL_R
            Dim dtLottiDaAggiornare As DataTable =
                oLottiDaAggiornare.LeggiListaLotti(inData.queryLottiXFiltroAggiuntivo, inData.queryLottiXOrderBy, inData.objParametri_Performa)

            Dim PerformaUpdater As New AgronicaCorePerformaDAL.GestoreCommesseLottiDAL_W

            Dim stbOk As New StringBuilder
            Dim stbErr As New StringBuilder


            Dim messaggioBase As String =
            "Esito Sincro delle giornate per commessa con codice: "

            'Esegue una query TEAM per ciascuna commessa/lotto per recuperare i workItem e leggere i tempi
            For Each riga As DataRow In dtLottiDaAggiornare.Rows

                'Query Team e calcolo totale delle ore, commessa/lotto per volta
                Dim inDataSomma As New TFSConnector.ConfigurazioneWIQL
                inData.cfgSincroOre.listaFiltri.Find(Function(o) o.campo = "Commessa").valore = riga("CodCommessa")
                inData.cfgSincroOre.listaFiltri.Find(Function(o) o.campo = "Task").valore = riga("CodSottocommessa")

                Dim oreTotali As rispostaStandard(Of Double) =
                    tfsHelper.SommatoriaTempiConFiltri(inData.cfgSincroOre)

                'converto le ore in giornate
                oreTotali.RispostaStringa = Math.Round(oreTotali.RispostaStringa / 8, 0)

                'update del dato sul peforma (gg a finire intero task)
                Dim sOreTotali As String = oreTotali.RispostaStringa.ToString()
                Dim rval As Boolean
                Dim MessaggioErrore As String = ""
                Try

                    If Not oreTotali.RispostaOK Then
                        Throw New Exception(oreTotali.Errore)
                    End If

                    rval = PerformaUpdater.AggiornaParametroDocumentoPerforma(
                            IDDocumento:=riga("IDDocumento"),
                            IDParametroModelloDocumento:=emum_Manager_Performa_DocCommesse_Parametri.Dati_Sez_Sottocommesse_PM_gg_a_FinireTask,
                            IDRiga:=riga("IDRiga"),
                            Progressivo:=riga("Progressivo"),
                            sOreTotali,
                            TipiEnumerativi.enum_Manager_TipiDatiSQLVariant.system_Real,
                            TabellaDD:="DDParametriDocumentiDefault",
                            inData.objParametri_Performa
                        )
                Catch ex As Exception
                    rval = False
                    MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                End Try

                r.RispostaOK = (r.RispostaOK And rval)

                PerformaRiportoTempiFeedBack(messaggioBase, rval, MessaggioErrore, stbOk, stbErr, riga, sOreTotali, 0, TipoFeedback.TotaleGiornateResidue)

            Next

            r.RispostaStringa &= stbOk.ToString
            r.Errore &= stbErr.ToString

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    Private Enum TipoFeedback
        TotaleGiornateResidue = 1
        PrioritaImpostata = 2
        RiportoTempiFunzionalitaAttivita = 3
    End Enum

    Private Shared Sub PerformaRiportoTempiFeedBack(messaggioBase As String, rval As Boolean, messaggioErrore As String, stbOk As StringBuilder, stbErr As StringBuilder, riga As DataRow, sDato As String, TFSWorkItemID As Integer, TipoFeedback As TipoFeedback)


        Dim err As String
        Dim utenteAssegnato As String = ""
        If riga.Table.Columns.Contains("utenteAssegnato") Then
            utenteAssegnato = " [" & riga("utenteAssegnato").ToString() & "]"
        End If

        Dim rigaTxt As String

        Select Case TipoFeedback
            Case TipoFeedback.TotaleGiornateResidue
                rigaTxt = riga("CodCommessa") & ", lotto: " & riga("CodSottoCommessa") & "; totale giornate residue" & utenteAssegnato & ": " & sDato & ". "
            Case TipoFeedback.PrioritaImpostata
                rigaTxt = riga("CodCommessa") & ", lotto: " & riga("CodSottoCommessa") & "; ID Work Item in TFS: " & TFSWorkItemID & ", Priorità impostata: " & sDato & ". "
            Case TipoFeedback.RiportoTempiFunzionalitaAttivita
                rigaTxt = riga("CodCommessa") & ", lotto: " & riga("CodSottoCommessa") & "; ID Funzionalità in TFS: " & TFSWorkItemID & ", [Stima Originale, Lavoro Rimanente], dato impostato: [" & sDato & "]. "
        End Select

        If rval Then
            err = "Positivo"
            stbOk.Append(messaggioBase)
            stbOk.Append(rigaTxt)
            stbOk.Append(err)
            stbOk.AppendLine("")
        Else
            err = "Negativo, Dettagli Errore:"
            stbErr.Append(messaggioBase)
            stbErr.Append(rigaTxt)
            stbErr.Append(err)
            stbErr.Append(messaggioErrore)
            stbErr.AppendLine("")
        End If

    End Sub

    Public Class SincronizzaPerformaTFSIn
        Public Property impostaSommatoriaTempiSuElementoPadre As Boolean
        Public Property elaboraSommatorieOrePerforma As Boolean
        Public Property impostaOrdinePrioritaSuTFS As Boolean
        Public Property objParametri_Performa As AgronicaCoreParametri
        Public Property stringaConnessionePerforma As String
        Public Property queryLottiXFiltroAggiuntivo As String
        Public Property queryLottiXOrderBy As String
        Public Property vstsCollectionUrl As String
        Public Property cfgSincroOre As TFSConnector.ConfigurazioneWIQL
        Public Property cfgSincroPriority As TFSConnector.ConfigurazioneWIQL
    End Class

End Class
