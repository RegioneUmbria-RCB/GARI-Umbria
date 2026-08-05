Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.exceptions

Public Class InfoOperazione
    Public IsTrattamento As Boolean = False
    Public IsFertilizzazione As Boolean = False
    Public IsLavorazione As Boolean = False
    Public IsRaccolta As Boolean = False
    Public IsSemina As Boolean = False
    Public IsVisita As Boolean = False
    Public isNonUtilizzo As Boolean = False
    Public IsAbbattimento As Boolean = False
    Public IsRilievo As Boolean = False
    Public IsZoo As Boolean = False
    Public IsIrrigazione As Boolean = False
    Public IsFertirrigazione As Boolean = False

    Public IsCarico As Boolean = False
    Public IsScarico As Boolean = False
    Public IsRegistrazione As Boolean = False

    Public Cau_Mov As String = ""
    Public Elem_Cod As Integer = 0

    Public BaseCode As Integer = 0
    Public TopCode As Integer = 2000000000

    Public classType As String = ""

    Public controlloPolverulenti As Boolean = False
    Public TipoOperazioneAgenda As enum_Tipo_Operazione_Agenda = 0

    Public TipoCentroDiCosto As AgronicaCoreModelsSTD.attivita.centri_di_costo.Tipo = AgronicaCoreModelsSTD.attivita.centri_di_costo.Tipo.Esercizio

    ''' <summary>
    ''' Apertura operazioni create con il vecchio che potrebbero avere N avversità salvate: in questi casi prendiamo solo la prima
    ''' </summary>
    Public PregressoConMultiAvversita As Boolean = False
End Class

Public Class VerificaDPIMultiAttivita
    ''' <summary>
    ''' <value name="TotCU_Trattamenti_MultiAttivita_xDistinta"> ((PIVA, SA_COD, APPEZZA, ID_REG), CU_xDISTINTA)
    ''' MI SALVO IL TOT DI RAME/Ha USATO NEI TRATTAMENTI X OGNI DISTINTA --> DA PASSARE AL VERIFICA CONCIMAZIONE
    ''' </value>
    ''' </summary>
    Public TotCUxHa_Trattamenti_MultiAttivita_xDistinta As New Dictionary(Of (String, Integer, Integer, Integer), Decimal)

    ''' <summary>
    ''' <value name="TotCU_Fertilizzazione_MultiAttivita_xDistinta"> ((PIVA, SA_COD, APPEZZA, ID_REG), CU_xDISTINTA)
    ''' MI SALVO IL TOT DI RAME/Ha USATO NELLE CONCIMAZIONI X OGNI DISTINTA --> DA PASSARE AL VERIFICA TRATTAMENTI
    ''' </value>
    ''' </summary>
    Public TotCUxHa_Fertilizzazione_MultiAttivita_xDistinta As New Dictionary(Of (String, Integer, Integer, Integer), Decimal)

    ''' <summary>
    ''' <value name="dic_CUTrattamenti_xDistintaxLavCod"> ((PIVA, SA_COD, APPEZZA, ID_REG), LAV_COD), CU_xDISTINTAxLAV_COD)
    ''' MI SALVO IL TOT DI RAME/Ha USATO NELLE CONCIMAZIONI X OGNI DISTINTA X OGNI LAV_COD --> DA PASSARE AL VERIFICA TRATTAMENTI, COSI' DA POTER CONTARE I MULTI TRATTAMENTI
    ''' </value>
    ''' </summary>
    Public dic_CUxHaTrattamenti_xDistintaxLavCod As New Dictionary(Of ((String, Integer, Integer, Integer), Integer), Decimal)

    ''' <summary>
    ''' <value name="dic_ProdottiRealixLavCod"> (LAV_COD, LISTA di PRO_COD)
    ''' MI SALVO I PROODOTTI REALI UTILIZZATI IN CIASCUNA ATTIVITA' PRIMA DELLA SOMMATORIA, PER FILTRARE SUI MESSAGGI DEL VERIFICA DPI E MOSTRARE SOLO QUELLI REALI X LAV_COD 
    ''' </value>
    ''' </summary>
    Public dic_ProdottiRealixLavCod As New Dictionary(Of Integer, List(Of Integer))
End Class

Public Class Utility_Agenda
    Public Shared Function GetInfoOperazione(ByVal lav_cod As Integer, ByVal tipoAttivita As Attivita.Tipo_Attivita) As InfoOperazione

        Dim InfoOperazione = New InfoOperazione

        With InfoOperazione

            Select Case lav_cod

                Case LAVCOD_DISTRIBUZIONE_CONCIME,
                     LAVCOD_SARCHIATURA_CONCIMAZIONE,
                     LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                     LAVCOD_CONCIMAZIONE_FOGLIARE,
                     LAVCOD_FERTIRRIGAZIONE,
                     LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                    .IsFertilizzazione = True
                    .IsFertirrigazione = lav_cod = LAVCOD_FERTIRRIGAZIONE
                    .Cau_Mov = CAU_LAVORAZIONE
                    .Elem_Cod = FERTILIZZANTI

                    .classType = costanti.ClassType.DettaglioFertilizzazione

                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                     LAVCOD_DISERBO,
                     LAVCOD_DISSECCAMENTO,
                     LAVCOD_GEODISINFESTAZIONE,
                     LAVCOD_CONCIA_SEME,
                     LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                     LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                     LAVCOD_TRATTAMENTO_POST_RACCOLTA,
                     LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA,
                     LAVCOD_REINNESCO_TRAPPOLE

                    .IsTrattamento = True
                    .Cau_Mov = CAU_TRATTAMENTO
                    .Elem_Cod = FORMULATI

                    .classType = costanti.ClassType.DettaglioTrattamento

                    Select Case lav_cod
                        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                             LAVCOD_DISERBO,
                             LAVCOD_DISSECCAMENTO,
                             LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                             LAVCOD_TRATTAMENTO_POST_RACCOLTA,
                             LAVCOD_CONCIA_SEME

                            .controlloPolverulenti = True
                        Case Else

                            .controlloPolverulenti = False
                    End Select

                    Select Case lav_cod
                        Case LAVCOD_TRATTAMENTO_POST_RACCOLTA,
                             LAVCOD_CONCIA_SEME
                            .TipoCentroDiCosto = centri_di_costo.Tipo.ProdottoDaTrattare
                        Case Else
                            .TipoCentroDiCosto = centri_di_costo.Tipo.Esercizio
                    End Select

                Case LAVCOD_RACCOLTA

                    .IsRaccolta = True
                    .Cau_Mov = CAU_RILIEVO_RACCOLTA
                    .Elem_Cod = TRASFORMATI_VEGETALI 'DT: fisso, non si gestiscono più i semilavorati

                    .classType = costanti.ClassType.DettaglioRaccolta

                Case LAVCOD_SEMINA,
                     LAVCOD_TRAPIANTO,
                     LAVCOD_SOVESCIO,
                     LAVCOD_SOD_SEDDING

                    .IsSemina = True
                    .Cau_Mov = CAU_LAVORAZIONE
                    .Elem_Cod = SEMENTI

                    .classType = costanti.ClassType.DettaglioSemina

                Case LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO,'--- DICHIARAZIONI
                     LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO

                    .isNonUtilizzo = True
                    If lav_cod = LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO Then
                        .Cau_Mov = CAU_LAVORAZIONE
                    ElseIf lav_cod = LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO Then
                        .Cau_Mov = CAU_TRATTAMENTO
                    End If

                Case LAVCOD_ABBATTIMENTOIMPIANTI
                    .IsAbbattimento = True
                    .Cau_Mov = CAU_LAVORAZIONE

                Case LAVCOD_ANDANAMENTO, LAVCOD_ARATURA, LAVCOD_DEFOGLIAZIONE, LAVCOD_ASPORTAZIONE_ORGANI_INFETTI, LAVCOD_ASSOLCATURA,
                     LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE, LAVCOD_DISSODAMENTO,
                     LAVCOD_ERPICATURA, LAVCOD_ESTIRPATURA, LAVCOD_ESPIANTO, LAVCOD_FALCIACONDIZIONATURA,
                     LAVCOD_FALCIATURA_ERBAI, LAVCOD_FORMAZIONE_ARGINELLI, LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA,
                     LAVCOD_IMBALLO_FIENO_ROTOLI, LAVCOD_INTERRAMENTO_PAGLIE, LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA,
                     LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI, LAVCOD_MESSA_DIMORA_PIANTE,
                     LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE, LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA,
                     LAVCOD_POTATURA_VERDE, LAVCOD_PRESSATURA, LAVCOD_RACCOLTA_LEGNA_POTATURA, LAVCOD_RACCOLTA_MANUALE, LAVCOD_RACCOLTA_MECCANICA,
                     LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA, LAVCOD_RIPPATURA, LAVCOD_RIPUNTATURA,
                     LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_RULLATURA, LAVCOD_SARCHIATURA, LAVCOD_SCARIFICATURA,
                     LAVCOD_SCASSO, LAVCOD_TRINCIATURA, LAVCOD_VANGATURA, LAVCOD_ZAPPATURA,
                     LAVCOD_GEBIATURA, LAVCOD_ROMPICROSTA, LAVCOD_LAVORAZIONE_CONBINATA,
                     LAVCOD_ERPICATURA_ROTANTE, LAVCOD_INTERVENTO_ANTIBRINA, LAVCOD_ALTRE_OPERAZIONI, LAVCOD_STRIGLIATURA,
                     LAVCOD_PIRODISERBO, LAVCOD_ABBATTIMENTOIMPIANTI,
                     LAVCOD_PASCOLAMENTO_PROPRIO, LAVCOD_PASCOLAMENTO_TERZI

                    .IsLavorazione = True
                    .Cau_Mov = CAU_LAVORAZIONE

                Case LAVCOD_COSTI_CDG '--- GESTIONE COSTI

                Case LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI  '--- LIQUIDAZIONE SOCI

                Case LAVCOD_SCARICO, LAVCOD_CARICO, LAVCOD_VENDITA, LAVCOD_ACQUISTO, LAVCOD_TRASFERIMENTO '--- GESTIONE MAGAZZINI

                    Select Case lav_cod

                        Case LAVCOD_CARICO

                            .IsCarico = True
                            .Cau_Mov = CAU_CARICO

                        Case LAVCOD_SCARICO

                            .IsScarico = True
                            .Cau_Mov = CAU_SCARICO

                    End Select

                Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA  '--- DOCUMENTI CONTABILI

                    Select Case lav_cod

                        Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA
                            .IsCarico = True
                            .IsRegistrazione = True
                            .Cau_Mov = CAU_CARICO

                        Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA
                            .IsScarico = True
                            .IsRegistrazione = True
                            .Cau_Mov = CAU_SCARICO

                    End Select

                Case LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA '--- NOTE ACCREDITO

                    Select Case lav_cod

                        Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
                            .IsScarico = True
                            .IsRegistrazione = True
                            .Cau_Mov = CAU_SCARICO

                        Case LAVCOD_NOTA_ACCREDITO_EMESSA
                            .IsCarico = True
                            .IsRegistrazione = True
                            .Cau_Mov = CAU_CARICO

                    End Select

                Case LAVCOD_DISTRIBUZIONE_INSETTI  '--- DISTRIBUZIONE INSETTI
                    .IsTrattamento = True
                    .Cau_Mov = CAU_TRATTAMENTO
                    .Elem_Cod = INSETTI

                    .classType = costanti.ClassType.DettaglioTrattamento

                Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                    .IsTrattamento = True
                    .Cau_Mov = CAU_TRATTAMENTO
                    .Elem_Cod = TRAPPOLE

                                   'TODO_DT: capire come gestire con Federica/Scatto
                'Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA ', --- INSTALLAZIONE TRAPPOLE
                '    .IsTrattamento = True
                '    .Cau_Mov = CAU_TRATTAMENTO
                '    .Elem_Cod = TRAPPOLE


                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_DANNI_RACCOLTA, LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_RILIEVO_PIOGGE, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA  '--- RILIEVI
                    .IsRilievo = True

                    Select Case lav_cod
                        Case LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_DANNI_RACCOLTA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                            .Cau_Mov = CAU_RILIEVO_RACCOLTA
                        Case Else
                            .Cau_Mov = CAU_RILIEVO_CAMPO
                    End Select

                    .classType = costanti.ClassType.DettaglioRilievo

                Case LAVCOD_VISITA ', LAVCOD_VISITA_GENERICA, LAVCOD_MONITORAGGIO_TEMPI_RIENTRO '--- RILIEVI
                    .IsVisita = True
                    .Cau_Mov = CAU_VISITE_ISPETTIVE
                    .Elem_Cod = 0

                Case LAVCOD_IRRIGAZIONE  '--- IRRIGAZIONE
                    .IsIrrigazione = True
                    .Cau_Mov = CAU_LAVORAZIONE
                    .Elem_Cod = 0

                    .classType = costanti.ClassType.DettaglioIrrigazione

                Case LAVCOD_CURA '--- CURA

                Case LAVCOD_MANUTENZIONE_MACCHINE, LAVCOD_REVISIONE_MACCHINE '--- MANUTENZIONE MACCHINE

                Case LAVCOD_GESTIONE_RIFIUTI  '--- GESTIONE RIFIUTI

                Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_NASCITA_ANIMALI, LAVCOD_ACQUISTO_ANIMALI '--- ZOO: CARICO
                    .IsZoo = True
                    .Cau_Mov = CAU_CARICO
                    .Elem_Cod = ZOO_CONSISTENZA

                Case LAVCOD_SPOSTAMENTI_ZOO  '--- ZOO: SPOSTAMENTI
                    .IsZoo = True

                Case LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI, '--- ZOO: ALIMENTAZIONE
                        LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI,
                        LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI,
                        LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI
                    .IsZoo = True
                    .Cau_Mov = CAU_ALIMENTAZIONE

                Case LAVCOD_PESATURA_ANIMALI '--- ZOO: PESATURA
                    .IsZoo = True
                    .Cau_Mov = CAU_PESATURA_ANIMALI

                Case LAVCOD_MORTE_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_VENDITA_ANIMALI  '--- ZOO: SCARICO
                    .IsZoo = True
                    .Cau_Mov = CAU_SCARICO
                    .Elem_Cod = ZOO_CONSISTENZA

                Case LAVCOD_ALTRE_LAVORAZIONI_ZOO  '--- ZOO: ALTRE LAVORAZIONI
                    .IsZoo = True

            End Select

            Select Case lav_cod
                Case LAVCOD_DISTRIBUZIONE_INSETTI, LAVCOD_TRATTAMENTO_POST_RACCOLTA
                    .PregressoConMultiAvversita = True
            End Select
        End With

        InfoOperazione.TipoOperazioneAgenda = 0

        Select Case tipoAttivita

            Case Attivita.Tipo_Attivita.QuadernoDiCampagna
                InfoOperazione.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna

            Case Attivita.Tipo_Attivita.Ricetta
                InfoOperazione.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta

        End Select

        Return InfoOperazione

    End Function

    ''' <param name="currentAttivitaDes"></param> attivita.job.descrizione // agenda.Des_Lib
    ''' <param name="provenienza_ex"></param> da quale blocco di controlli proviene l'errore 
    '''                                       (importante se si tratta di un Warning (SI/NO) --> se l'utente decide di 
    '''                                       procedere (SI) riusciamo a capire che dobbiamo saltare i warning per un determinato blocco) 
    Public Shared Function generaErroreGias(severity As ErroreGias_Severity,
                                            currentAttivitaDes As String,
                                            messaggio As String,
                                            provenienza_ex As String) As ErroreGias

        'Se mi viene passata la descrizione l'aggiungo prima del messaggio
        If currentAttivitaDes <> "" Then
            'Se si tratta di un alert evidenzio in grassetto l'attività che ha generato il messaggio 
            If severity = ErroreGias_Severity.WarningBloccante Or severity = ErroreGias_Severity.Warning Then
                currentAttivitaDes = "<b>" & currentAttivitaDes & "</b>"
            End If
            messaggio = currentAttivitaDes & ": " & messaggio
        End If

        Dim objErroreGias As New ErroreGias With {
            .severity = severity,
            .tipo = ErroreGias_Tipo.Generico,
            .messaggio = messaggio,
            .ex = provenienza_ex
        }

        Return (objErroreGias)
    End Function

    Public Shared Function rimuoviDuplicatiMessaggioErroreVerificaDPI(errore As String) As String

        Dim listaErrori As New List(Of String)
        Dim newErrore As String = errore

        If errore IsNot Nothing AndAlso errore.Contains("<br>") Then
            listaErrori = Strings.Split(errore, "<br>").ToList()

            listaErrori = listaErrori.Distinct.ToList()

            newErrore = String.Join("<br>", listaErrori)
        End If

        Return newErrore

    End Function

    Public Shared Function esisteAlmenoUnaMovimentazione(lista_Attivita As List(Of Attivita)) As Boolean

        Dim listaMovimentazioni As New List(Of RilevamentoDiMagazzino)
        For Each attivita In lista_Attivita
            Dim infoOperazione As InfoOperazione = GetInfoOperazione(attivita.job.primaryKey.codice, Tipo_Attivita.QuadernoDiCampagna)

            If infoOperazione.IsRaccolta = False Then
                Dim risorsaProdottoList As List(Of risorse.Risorsa) = attivita.risorse.FindAll(Function(c) (c.classType = infoOperazione.classType))

                For Each ris In risorsaProdottoList

                    If TypeOf (ris) Is risorse.RisorsaProdotto Then
                        If IsNothing(CType(ris, risorse.RisorsaProdotto).MagazziniMovimentazioni) Then
                            'In caso mi dovesse arrivare MagazziniMovimentazioni nothing, provvedo a trasformarlo in una lista vuota
                            CType(ris, risorse.RisorsaProdotto).MagazziniMovimentazioni = New List(Of RilevamentoDiMagazzino)
                        End If
                        listaMovimentazioni.AddRange(CType(ris, risorse.RisorsaProdotto).MagazziniMovimentazioni)
                    End If
                Next
            End If
        Next

        If listaMovimentazioni.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Shared Function GetDescrizioneProdotto(Elem_Cod As Integer,
                                                   Mat_Cod As Integer, Pro_Cod As Integer,
                                                   Piva As String,
                                                   Av_Cod As Integer, Av_Gru_Cod As Integer,
                                                   objParametri_Server As AgronicaCoreParametri) As String

        Dim descrizioneProdotto = ""

        Select Case Elem_Cod
            Case FORMULATI
                Dim objDes As New AgronicaCoreMetaSchemaDAL.Formulati_R
                descrizioneProdotto = objDes.FrDes_from_FrCod(Pro_Cod, objParametri_Server)
            Case FERTILIZZANTI
                Dim objDes As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
                descrizioneProdotto = objDes.FerDes_from_FerCod(Pro_Cod, objParametri_Server)
            Case SEMENTI
                Dim objDes As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                descrizioneProdotto = objDes.MatDes_from_MatCod(Piva, SEMENTI, Mat_Cod, "", "", "", objParametri_Server)
            Case INSETTI
                Dim objDes As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R
                descrizioneProdotto = objDes.InsDes_from_InsCod(Pro_Cod, objParametri_Server)
            Case INNESCHI

                If Av_Cod > 0 AndAlso Av_Gru_Cod = 0 Then
                    Dim objDes As New AgronicaCoreMetaSchemaDAL.Avversita_R
                    descrizioneProdotto = objDes.AvDes_from_AvCod(Av_Cod, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                ElseIf Av_Cod = 0 AndAlso Av_Gru_Cod > 0 Then
                    Dim objDes As New AgronicaCoreMetaSchemaDAL.GruppoAvversita_R
                    descrizioneProdotto = objDes.AvGruDes_from_AvGruCod(Av_Gru_Cod, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                End If

        End Select

        Return descrizioneProdotto

    End Function

    Public Shared Function GetDescrizioneUDM(UDM_Cod As Integer,
                                              objParametri_Server As AgronicaCoreParametri) As String

        Dim objDesUM As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        Dim descrizioneUDM As String = objDesUM.UdmDes_from_UdmCod(UDM_Cod, "", objParametri_Server)

        Return descrizioneUDM
    End Function

#Region "Controlli Impostazioni Utente"
    Public Shared Sub getSeverityDaValoreImpostazione_0Warning_1WarningBloccante(ByRef severity As Integer,
                                                                                 enumImpostazioni As enum_Impostazioni_Utenti,
                                                                                 objParametri_Utenti As AgronicaCoreParametri,
                                                                                 Optional ByRef ValoreImpostazione As Integer = 0)
        'Reset 
        severity = -1

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        ValoreImpostazione = CInt(objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enumImpostazioni, objParametri_Utenti.UtenteUsername, objParametri_Utenti))

        Select Case ValoreImpostazione
            Case 0
                severity = ErroreGias_Severity.Warning
            Case 1
                severity = ErroreGias_Severity.WarningBloccante
            Case Else
                Throw New Exception("ValoreImpostazione =  " & ValoreImpostazione & " NON è contemplato in getSeverityDaValoreImpostazione_0Warning_1WarningBloccante, cambia funzione")
        End Select
    End Sub

    Public Shared Sub getSeverityDaValoreImpostazione_0NessunBlocco_1WarningBloccante_2Warning(ByRef severity As Integer,
                                                                                               enumImpostazioni As enum_Impostazioni_Utenti,
                                                                                               objParametri_Utenti As AgronicaCoreParametri,
                                                                                               Optional ByRef ValoreImpostazione As Integer = 0)
        'Reset 
        severity = -1

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        ValoreImpostazione = CInt(objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enumImpostazioni, objParametri_Utenti.UtenteUsername, objParametri_Utenti))

        Select Case ValoreImpostazione
            Case 0
                'Nessun blocco --> chi richiama questa funzione deve controllare che ValoreImpostazione <> 0
            Case 1
                severity = ErroreGias_Severity.WarningBloccante
            Case 2
                severity = ErroreGias_Severity.Warning
            Case Else
                Throw New Exception("ValoreImpostazione =  " & ValoreImpostazione & " NON è contemplato in getSeverityDaValoreImpostazione_0NessunBlocco_1Warning_2WarningBloccante, cambia funzione")
        End Select
    End Sub

    Public Shared Sub getSeverityDaValoreImpostazione_0NessunBlocco_1Warning_2WarningBloccante(ByRef severity As Integer,
                                                                                               enumImpostazioni As enum_Impostazioni_Utenti,
                                                                                               objParametri_Utenti As AgronicaCoreParametri,
                                                                                               Optional ByRef ValoreImpostazione As Integer = 0)
        'Reset 
        severity = -1

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        ValoreImpostazione = CInt(objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enumImpostazioni, objParametri_Utenti.UtenteUsername, objParametri_Utenti))

        Select Case ValoreImpostazione
            Case 0
                'Nessun blocco --> chi richiama questa funzione deve controllare che ValoreImpostazione <> 0
            Case 1
                severity = ErroreGias_Severity.Warning
            Case 2
                severity = ErroreGias_Severity.WarningBloccante
            Case Else
                Throw New Exception("ValoreImpostazione =  " & ValoreImpostazione & " NON è contemplato in getSeverityDaValoreImpostazione_0NessunBlocco_1Warning_2WarningBloccante, cambia funzione")
        End Select
    End Sub

    Public Shared Sub getSeverityDaValoreImpostazione_0NessunBlocco_1WarningBloccante(ByRef severity As Integer,
                                                                                      enumImpostazioni As enum_Impostazioni_Utenti,
                                                                                      objParametri_Utenti As AgronicaCoreParametri,
                                                                                      Optional ByRef ValoreImpostazione As Integer = 0)
        'Reset 
        severity = -1

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        ValoreImpostazione = CInt(objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enumImpostazioni, objParametri_Utenti.UtenteUsername, objParametri_Utenti))

        Select Case ValoreImpostazione
            Case 0
                'Nessun blocco --> chi richiama questa funzione deve controllare che ValoreImpostazione <> 0
            Case 1
                severity = ErroreGias_Severity.WarningBloccante
            Case Else
                Throw New Exception("ValoreImpostazione =  " & ValoreImpostazione & " NON è contemplato in getSeverityDaValoreImpostazione_0Warning_1WarningBloccante, cambia funzione")
        End Select
    End Sub

#End Region


#Region "Funzioni di estrazione di parti del vecchio modello"
    Public Shared Function GetMovimentoCampagna(agenda As Operazione_Agenda, infoOperazione As InfoOperazione) As Movimento
        Dim MovimentoCampagna As Movimento = agenda.Movimenti.FindAll(Function(c) (c.Cau_Mov = infoOperazione.Cau_Mov)).FirstOrDefault
        Return MovimentoCampagna
    End Function
#End Region

    Public Shared Function extraiXMLAgenda(agenda As Operazione_Agenda,
                                           infoOperazione As InfoOperazione,
                                           ByRef Dpi_Cod As Integer,
                                           ByRef Dpi_PubblicoPrivato As Integer
                                           ) As String

        Dim Helper As New Agenda_Operazione_Helper
        Dim strXML As String = ""

        If infoOperazione.IsTrattamento = True Then
            strXML = Helper.GeneraXML_CAU_TRATTAMENTO(agenda, Dpi_Cod, Dpi_PubblicoPrivato)
        End If

        If infoOperazione.IsFertilizzazione = True Then
            strXML = Helper.GeneraXML_CAU_LAVORAZIONI(agenda, Dpi_Cod, Dpi_PubblicoPrivato)
        End If

        If infoOperazione.IsRaccolta = True Then
            strXML = Helper.GeneraXML_CAU_LAVORAZIONI(agenda)
        End If

        If strXML = "" Then
            'Non dovrebbe entrare qui
            Throw New Exception("Stringa XML CheckDPI non valorizzata")
        End If

        strXML = Replace(strXML, "TipoOperazioneDB=""1""", "TipoOperazioneDB=""" & agenda.Tipo_Operazione & """")

        Return strXML

    End Function

    Public Shared Sub estraiCodiciAttivita_x_CentriAziendali(QueryStringFiltrino As String,
                                                             ByRef codiciAttivita_x_CentriAziendali_List As List(Of AgronicaCoreModelsSTD.attivita.CodiciAttivita_x_CentriAziendali))


        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim qs = QueryStringFiltrino.Split("&")

        For Each x In qs
            If x.Contains("lista_Codici_Attivita_x_CentriAziendali") Then
                Dim dumm = x.Replace("lista_Codici_Attivita_x_CentriAziendali=", "")

                codiciAttivita_x_CentriAziendali_List = JsonConvert.DeserializeObject(Of List(Of AgronicaCoreModelsSTD.attivita.CodiciAttivita_x_CentriAziendali))(dumm, settings)
                Exit Sub
            End If
        Next

        codiciAttivita_x_CentriAziendali_List = Nothing

    End Sub

    ''' <summary>
    ''' Le Trappole Nuove ora sono sotto l'elem_cod = 191 (FORMULATI)
    ''' </summary>
    ''' <param name="InfoOperazione"></param>
    ''' <returns></returns>
    Public Shared Function isInsettiUtiliTrappole(InfoOperazione As InfoOperazione) As Boolean

        If InfoOperazione.Elem_Cod = INSETTI OrElse InfoOperazione.Elem_Cod = TRAPPOLE Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Shared Function eseguiCheckDPI(InfoOperazione As InfoOperazione, attivita As Attivita) As Boolean

        'Casanova 21/10/2024 evito di fare il CheckDPI per la Raccolta che attualmente controlla solo se viene rispettata la Data Utile Prima Raccolta
        '(Data Carenza dei Trattamenti) perchè viene già fatta precedentemente da codice e rifare le riletture da DB su clienti con molti impianti e con diversi trattamenti registrati 
        'diventa deleterio per il salvataggio infatti va in timeout la chiamata (riferimento alla chiamata 32706 di Regione Umbria)
        If (InfoOperazione.IsTrattamento AndAlso
            attivita.disciplinare IsNot Nothing AndAlso
            attivita.disciplinare.codice <> enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta.ToString("d") AndAlso
            InfoOperazione.TipoCentroDiCosto = centri_di_costo.Tipo.Esercizio AndAlso
            Not isInsettiUtiliTrappole(InfoOperazione)) OrElse
            (InfoOperazione.IsFertilizzazione) Then
            Return True
        Else
            Return False
        End If

    End Function

End Class
