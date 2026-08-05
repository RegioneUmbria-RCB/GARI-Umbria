Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreScadenziario
Imports AgronicaCoreUmaDal
Imports AgronicaCoreUmaDal.UMASetup_W
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class UMA_Lavorazioni
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Lavorazioni_LeggiTabella(inizioValidita As String, fineValidita As String, objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim AgronicaDAL As New UMA_Lavorazioni_R
        Dim alertTipologie As New Alert_Tipologia_R
        Dim MessaggioErrore As String
        Dim filtroAggiutivo As String = ""
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.UMA_Lavorazioni.Lavorazioni_LeggiTabella()"
        Try

            AgronicaDAL.ComponiFiltroAggiuntivoValidita(filtroAggiutivo, inizioValidita, fineValidita)

            Dim uf As DataTable = AgronicaDAL.Leggi("", filtroAggiutivo, "", objParametri_Server)

            Dim Dt As New DataTable
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Dt.Columns.Add(New DataColumn("Regione_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Lav_UMA_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Lav_UMA_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Inviato", GetType(Short)))
            Dt.Columns.Add(New DataColumn("DataInvio", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Data_Creazione", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Data_Modifica", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Username_Creazione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Username_Modifica", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Maggiorazione_Terreno_MedioTenace", GetType(Boolean)))
            Dt.Columns.Add(New DataColumn("GestioneTerzista", GetType(Boolean)))
            Dt.Columns.Add(New DataColumn("Utilizzata_Da_Consorzio_Bonifica", GetType(Boolean)))

            For Each elem As DataRow In uf.Rows
                If elem("Lav_UMA_Cod") <> "00000" Then
                    Dim d = Dt.NewRow
                    d("Regione_Cod") = elem("Regione_Cod")
                    d("Lav_UMA_Cod") = elem("Lav_UMA_Cod")
                    d("Lav_UMA_Des") = elem("Lav_UMA_Des")
                    d("Inviato") = elem("Inviato")
                    d("DataInvio") = elem("DataInvio")
                    d("Data_Creazione") = elem("Data_Creazione")
                    d("Data_Modifica") = elem("Data_Modifica")
                    d("Username_Creazione") = elem("Username_Creazione")
                    d("Username_Modifica") = elem("Username_Modifica")
                    d("Validita_Inizio") = elem("Validita_Inizio")
                    d("Validita_Fine") = elem("Validita_Fine")
                    d("Maggiorazione_Terreno_MedioTenace") = elem("Maggiorazione_Terreno_MedioTenace")
                    d("GestioneTerzista") = elem("GestioneTerzista")
                    d("Utilizzata_Da_Consorzio_Bonifica") = elem("Utilizzata_Da_Consorzio_Bonifica")

                    Dt.Rows.Add(d)
                End If
            Next

            Return Dt

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function

    'Public Function UF_ControlloCapiAllevabili(ByVal piva As String,
    '                                           ByVal richiestaCod As Integer,
    '                                           ByRef objParametri_Server As AgronicaCoreParametri) As String

    '    Dim nomeRoutine = "AgronicaCoreUmaBiz.UMA_UF.UF_ControlloCapiAllevabili()"

    '    Dim Dt As DataTable
    '    Dim dtRichiesta As DataTable
    '    Dim Dtlav As DataTable
    '    Dim DtAllevamenti As DataTable
    '    Dim dtUFProd As DataTable
    '    Dim leggiUFColture As New AgronicaCoreUmaDal.UMA_UF_Colture_R
    '    Dim UF As Decimal
    '    Dim UFL As Decimal
    '    Dim UFC As Decimal
    '    Dim UF_Tot As Decimal
    '    Dim UFL_Tot As Decimal
    '    Dim UFC_Tot As Decimal
    '    Dim UFProdotti_Media_Tot As Decimal
    '    Dim fabbisognoMin As Decimal
    '    Dim fabbisognoMax As Decimal
    '    Dim fabbisognoMedio As Decimal
    '    Dim fabbisognoDict As New Dictionary(Of Integer, (Integer, Decimal, String))
    '    Dim dictTemp As (Integer, Decimal, String)
    '    Dim Tot_fabbisogno_Capi As Decimal = 0
    '    Dim strCapiInEccesso As String = ""
    '    Dim ripartizioneUFProdotte As Decimal
    '    Dim capiAllevabili As Decimal
    '    Dim leggitestata As New UMA_Richieste_Testata_R
    '    Dim leggiLavorazioni As New UMA_Richieste_Lavorazioni_R
    '    Dim leggiUFProdotte As New UMA_Richieste_Allevamenti_UF_R
    '    Dim leggiAllevamenti As New UMA_Richieste_Allevamenti_R
    '    Dim irrigua As Boolean
    '    Dim allevatiMontagna As Boolean = False
    '    Dim nomeDirLogCalcoloCapiAllevabili As String = ""
    '    Dim nomeFileLogCalcoloCapiAllevabili As String = ""

    '    'Lettura chiave test UMA
    '    Dim objCfgSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
    '    Dim VCS_TestUMA = objCfgSiti.Leggi_Valore_JSON(Of AgronicaCoreVarieDAL.VCS_TestUMA)(objParametri_Server)

    '    If VCS_TestUMA.LogCalcoloCapiAllevabili = 1 Then
    '        nomeDirLogCalcoloCapiAllevabili = objParametri_Server.LogDirectory + "\CarburantiUMA"
    '        nomeFileLogCalcoloCapiAllevabili = String.Format("Log_CalcoloCapiAllevabili_{0}_{1}.txt",
    '                                                         Now().ToString("yyyy"),
    '                                                         Now().ToString("MM"))
    '    End If

    '    'Lettura dati pratica

    '    dtRichiesta = leggitestata.LeggiDaRichiestaCod(richiestaCod, objParametri_Server)

    '    'If Dt.Rows.Count > 0 Then
    '    '    allevatiMontagna = IIf(Dt.Rows.Item(0).Item("Allevati_Montagna") = 1, True, False)
    '    'End If

    '    'Lettura allevamenti indicati in pratica e calcolo fabbisogno UF

    '    DtAllevamenti = leggiAllevamenti.Leggi(piva,
    '                                           "",
    '                                           richiestaCod,
    '                                           "",
    '                                           "",
    '                                           enumSelezioneVariabile.Selezione_JoinDescrizioni,
    '                                           objParametri_Server)

    '    Dim capiConFabbisogniIndicati = False

    '    For Each allevamentoRow As DataRow In DtAllevamenti.Rows

    '        If UF_Latte_Indicate(allevamentoRow) Then

    '            fabbisognoMin = allevamentoRow.Item("ufl_min")
    '            fabbisognoMax = allevamentoRow.Item("ufl_max")

    '        ElseIf UF_Carne_Indicate(allevamentoRow) Then

    '            fabbisognoMin = allevamentoRow.Item("ufc_min")
    '            fabbisognoMax = allevamentoRow.Item("ufc_max")

    '        End If

    '        If fabbisognoMin > 0 AndAlso fabbisognoMax > 0 Then

    '            capiConFabbisogniIndicati = True

    '            fabbisognoMedio = (fabbisognoMin + fabbisognoMax) / 2

    '            fabbisognoMedio *= allevamentoRow.Item("Totale_Capi")

    '            If fabbisognoDict.ContainsKey(allevamentoRow.Item("UMA_AllGru_Cod")) Then
    '                dictTemp = fabbisognoDict.Item(allevamentoRow.Item("UMA_AllGru_Cod"))
    '                fabbisognoDict.Remove(allevamentoRow.Item("UMA_AllGru_Cod"))
    '                fabbisognoDict.Add(allevamentoRow.Item("UMA_AllGru_Cod"), (dictTemp.Item1 + allevamentoRow.Item("Totale_Capi"), dictTemp.Item2 + fabbisognoMedio, dictTemp.Item3))
    '            Else
    '                fabbisognoDict.Add(allevamentoRow.Item("UMA_AllGru_Cod"), (allevamentoRow.Item("Totale_Capi"), fabbisognoMedio, allevamentoRow.Item("UMA_AllGru_Des")))
    '            End If

    '            Tot_fabbisogno_Capi += fabbisognoMedio

    '        End If

    '        fabbisognoMin = 0
    '        fabbisognoMax = 0

    '    Next

    '    'Il controllo è obbligatorio se sono stati indicati capi che in configurazione prevedono UFL/UFC

    '    If capiConFabbisogniIndicati Then

    '        Dim messaggioLog = ""

    '        If VCS_TestUMA.LogCalcoloCapiAllevabili = 1 Then
    '            messaggioLog = String.Format("--- Calcolo capi allevabili (RichiestaCod:{0})", richiestaCod)
    '            Scrivi_LOG(nomeDirLogCalcoloCapiAllevabili, nomeFileLogCalcoloCapiAllevabili, objParametri_Server.LogDescrizioneUtente, "", messaggioLog)
    '        End If

    '        'Calcolo UF prodotte

    '        Dt = leggiUFProdotte.LeggiColtureUF(piva,
    '                                            richiestaCod,
    '                                            0,
    '                                            0,
    '                                            "",
    '                                            "",
    '                                            "",
    '                                            "",
    '                                            "",
    '                                            "",
    '                                            objParametri_Server)

    '        Dim xFiltroAggiuntivo As String = ""

    '        leggiUFColture.ComponiFiltroAggiuntivoValidita(xFiltroAggiuntivo, CDate(dtRichiesta.Rows.Item(0).Item("Validita_Inizio")).ToShortDateString, CDate(dtRichiesta.Rows.Item(0).Item("Validita_Fine")).ToShortDateString)

    '        For Each ufprod As DataRow In Dt.Rows

    '            If ufprod.Item("Tipo_Territorio") = 1 Then
    '                Dtlav = leggiLavorazioni.Leggi(piva,
    '                                               ufprod.Item("macrouso_Uma_Cod"),
    '                                               ufprod.Item("programmazione_cod"),
    '                                               richiestaCod,
    '                                               0,
    '                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
    '                                               objParametri_Server)
    '                If Dtlav.Select("Lavorazione_GIAS = 1").Count > 0 Then
    '                    irrigua = True
    '                Else
    '                    irrigua = False
    '                End If
    '            Else
    '                irrigua = True
    '            End If

    '            dtUFProd = leggiUFColture.LeggiUF(ufprod.Item("Occupazione_Cod"),
    '                                              ufprod.Item("Destinazione_Cod"),
    '                                              ufprod.Item("Uso_Cod"),
    '                                              ufprod.Item("Qualita_Cod"),
    '                                              irrigua,
    '                                              xFiltroAggiuntivo,
    '                                              "",
    '                                              objParametri_Server)

    '            If dtUFProd.Rows.Count > 0 Then
    '                UF = dtUFProd.Rows.Item(0).Item("UF")
    '                UFL = dtUFProd.Rows.Item(0).Item("UFL")
    '                UFC = dtUFProd.Rows.Item(0).Item("UFC")
    '            Else
    '                UF = 0
    '                UFL = 0
    '                UFC = 0
    '            End If

    '            UF_Tot += (CDec(ufprod.Item("Superf_Calcolo")) * UF)
    '            UFL_Tot += (CDec(ufprod.Item("Superf_Calcolo")) * UFL)
    '            UFC_Tot += (CDec(ufprod.Item("Superf_Calcolo")) * UFC)

    '            If VCS_TestUMA.LogCalcoloCapiAllevabili = 1 Then
    '                messaggioLog = String.Format("MacrousoCod:{0} - " &
    '                                             "ProgrammazioneCod:{1} - " &
    '                                             "OccupCod:{2} - " &
    '                                             "DestCod:{3} - " &
    '                                             "UsoCod:{4} - " &
    '                                             "QualCod:{5} - " &
    '                                             "SuperfCalcolo:{6} - " &
    '                                             "UF:{7} - UFL:{8} - UFC:{9}",
    '                                             ufprod.Item("macrouso_Uma_Cod"),
    '                                             ufprod.Item("programmazione_cod"),
    '                                             ufprod.Item("Occupazione_Cod"),
    '                                             ufprod.Item("Destinazione_Cod"),
    '                                             ufprod.Item("Uso_Cod"),
    '                                             ufprod.Item("Qualita_Cod"),
    '                                             ufprod.Item("Superf_Calcolo"),
    '                                             UF,
    '                                             UFL,
    '                                             UFC)
    '                Scrivi_LOG(nomeDirLogCalcoloCapiAllevabili, nomeFileLogCalcoloCapiAllevabili, objParametri_Server.LogDescrizioneUtente, "", messaggioLog)
    '            End If

    '        Next

    '        UFProdotti_Media_Tot = (UF_Tot + UFL_Tot + UFC_Tot) / 3

    '        'Calcolo finale capi allevabili

    '        Dim totaleCapiDichiarati = 0

    '        Dim totaleCapiGruppo = 0
    '        Dim fabbisognoMedioGruppo = 0
    '        Dim descrizioneGruppo = ""

    '        For Each x In fabbisognoDict.Values

    '            totaleCapiGruppo = x.Item1
    '            fabbisognoMedioGruppo = x.Item2
    '            descrizioneGruppo = x.Item3

    '            totaleCapiDichiarati += totaleCapiGruppo

    '            Dim capiAllevabiliGruppo As Decimal = 0
    '            ripartizioneUFProdotte = 0

    '            If Tot_fabbisogno_Capi > 0 AndAlso totaleCapiGruppo > 0 AndAlso fabbisognoMedioGruppo > 0 Then
    '                ripartizioneUFProdotte = fabbisognoMedioGruppo * UFProdotti_Media_Tot / Tot_fabbisogno_Capi
    '                capiAllevabiliGruppo = ripartizioneUFProdotte * totaleCapiGruppo / (fabbisognoMedioGruppo / 4)
    '                capiAllevabili += capiAllevabiliGruppo
    '            End If

    '            If VCS_TestUMA.LogCalcoloCapiAllevabili = 1 Then
    '                messaggioLog = String.Format("Gruppo:{0} - " &
    '                                         "CapiAllevabili:{1} - " &
    '                                         "CapiGruppo:{2} - " &
    '                                         "FabbMedioGruppo:{3} - " &
    '                                         "UFProdMediaTot:{4} - " &
    '                                         "TotFabbCapi:{5}",
    '                                         descrizioneGruppo,
    '                                         capiAllevabiliGruppo,
    '                                         totaleCapiGruppo,
    '                                         fabbisognoMedioGruppo,
    '                                         UFProdotti_Media_Tot,
    '                                         Tot_fabbisogno_Capi)
    '                Scrivi_LOG(nomeDirLogCalcoloCapiAllevabili, nomeFileLogCalcoloCapiAllevabili, objParametri_Server.LogDescrizioneUtente, "", messaggioLog)
    '            End If

    '        Next

    '        'If allevatiMontagna Then
    '        '    Dim riduzioneCapiAllevabili = capiAllevabili * PERCENTUALE_DIMINUZIONE_ALLEVATI_IN_MONTAGNA / 100
    '        '    If VCS_TestUMA.LogCalcoloCapiAllevabili = 1 Then
    '        '        messaggioLog = String.Format("RiduzioneZonaMontana: -{0}% [CapiAllevabiliOrigine:{1} - Riduzione:{2}]",
    '        '                                     PERCENTUALE_DIMINUZIONE_ALLEVATI_IN_MONTAGNA,
    '        '                                     capiAllevabili,
    '        '                                     riduzioneCapiAllevabili)
    '        '        Scrivi_LOG(nomeDirLogCalcoloCapiAllevabili, nomeFileLogCalcoloCapiAllevabili, objParametri_Server.LogDescrizioneUtente, "", messaggioLog)
    '        '    End If
    '        '    capiAllevabili -= riduzioneCapiAllevabili
    '        'End If

    '        Dim totaleCapiAllevabili = Math.Round(capiAllevabili, 0, MidpointRounding.AwayFromZero)

    '        If VCS_TestUMA.LogCalcoloCapiAllevabili = 1 Then
    '            messaggioLog = String.Format("TotaleCapiAllevabili:{0} - TotaleCapiDichiarati:{1}",
    '                                     totaleCapiAllevabili,
    '                                     totaleCapiDichiarati)
    '            Scrivi_LOG(nomeDirLogCalcoloCapiAllevabili, nomeFileLogCalcoloCapiAllevabili, objParametri_Server.LogDescrizioneUtente, "", messaggioLog)
    '        End If

    '        If totaleCapiDichiarati > totaleCapiAllevabili Then
    '            strCapiInEccesso = String.Format("Il numero di capi impostato sugli allevamenti soggetti a controllo ({0}) supera il numero dei capi allevabili ({1})",
    '                                             totaleCapiDichiarati,
    '                                             totaleCapiAllevabili)
    '        End If

    '    End If

    '    Return strCapiInEccesso

    'End Function

    Public Function Lavorazioni_SalvaGriglia(ByRef objParametri As AgronicaCoreParametri,
                                                       righeInseriteJson As String,
                                                       righeModificateJson As String,
                                                       righeCancellateJson As String) As RispostaStandard

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.UMA_Lavorazioni.UMA_Lavorazioni_SalvaGriglia()"

        '-------------------- Dichiarazioni di variabili ----------------------
        Dim MessaggioErrore As String
        Dim efConnString As String
        Dim righeCancellate As List(Of UMA_Lavorazioni_Dto)
        Dim righeModificate As List(Of UMA_Lavorazioni_Dto)
        Dim righeInserite As List(Of UMA_Lavorazioni_Dto)
        Dim Errori As String = String.Empty


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim risposta As New RispostaStandard

        ' Due istruzioni try/catch per prenedre nella transazione soltanto le righe rilevanti
        Try
            '------------------------------ Deserializza le righe di input ------------------------------
            Dim deserializerSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            righeCancellate = JsonConvert.DeserializeObject(Of List(Of UMA_Lavorazioni_Dto))(righeCancellateJson, deserializerSettings)
            righeModificate = JsonConvert.DeserializeObject(Of List(Of UMA_Lavorazioni_Dto))(righeModificateJson, deserializerSettings)
            righeInserite = JsonConvert.DeserializeObject(Of List(Of UMA_Lavorazioni_Dto))(righeInseriteJson, deserializerSettings)

            '------------------------------- Carica Entity Framework ----------------------------------------
            Dim gefutils As New Gias_EF_Utility
            efConnString = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Using scope As New TransactionScope()
            Try
                ' ------------------------------------- Salva i dati nel database -------------------------------------
                Dim context As New Gias_DeveloperServer_Entities(efConnString)
                Dim AgronicaDAL As New AgronicaCoreUmaDal.UMA_Lavorazioni_W

                '------------------------- Rimuovi le righe cancellate -------------------------
                If Not righeCancellate Is Nothing AndAlso righeCancellate.Count > 0 Then
                    AgronicaDAL.Rimuovi(righeCancellate, objParametri)
                End If

                '------------------------------ Verifica validità righe input ------------------------------    
                Errori = String.Empty
                'If righeInserite.Count() > 0 OrElse righeModificate.Count() > 0 Then
                '    ControlliValidita_UMASetup(righeInserite, righeModificate, objParametri, efConnString, Errori)
                'End If

                If (Not String.IsNullOrEmpty(Errori)) Then
                    risposta.RispostaOK = False
                    risposta.Errore = Errori
                    Return risposta
                End If

                If String.IsNullOrEmpty(Errori) Then

                    '------------------------- Inserici le righe modificate -------------------------
                    If Not righeModificate Is Nothing AndAlso righeModificate.Count > 0 Then
                        AgronicaDAL.Aggiorna(righeModificate, objParametri, Errori)
                    End If

                    '------------------------- Salva le nuove righe inserite -------------------------
                    If Not righeInserite Is Nothing AndAlso righeInserite.Count > 0 Then
                        Errori = AgronicaDAL.AggiungiNuovi(righeInserite, objParametri, Errori)
                    End If

                End If
                scope.Complete()
                scope.Dispose()

                If Errori.Length > 0 Then

                    risposta.RispostaOK = False
                    risposta.RispostaStringa = Errori
                    risposta.Errore = Errori

                Else

                    risposta.RispostaOK = True
                    risposta.RispostaStringa = "L'operazione è stata completata con successo."

                End If

                Return risposta
            Catch ex As Exception
                scope.Dispose()

                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
                Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
            End Try
        End Using
    End Function

End Class
