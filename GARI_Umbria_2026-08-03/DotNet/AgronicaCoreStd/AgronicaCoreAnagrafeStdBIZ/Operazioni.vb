Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreAnagrafeStdDAL.Operazioni_R
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate
Imports AgronicaCoreDataProviderSTD.TipiEnumerativi

Public Class Operazioni

    Private ReadOnly dbContext As GiasDbContext

    Public Sub New(dbcontext As GiasDbContext)
        Me.dbContext = dbcontext
    End Sub

    Public Shared Function OperazioniRilieviDisponibiliDaTipoRicetta(qs_tipo_ricetta As enum_TipoRicetta_DB) As List(Of Integer)


        Dim filtro As New List(Of Integer)

        Select Case qs_tipo_ricetta
            Case enum_TipoRicetta_DB.Standard_Destinazioni
                filtro.Add(LAVCOD_FASI_FENOLOGICHE)
                filtro.Add(LAVCOD_RILIEVO_INDICI_MATURITA)
                filtro.Add(LAVCOD_RILIEVO_AVVERSITA_CAMPO)
        End Select

        Return filtro
    End Function
    ''' <summary>
    ''' la funzione elenca in una lista le operazioni disponibili su Gias APP.
    ''' </summary>
    ''' <param name="qs_tipo_ricetta"></param>
    ''' <returns></returns>
    ''' <remarks>a mano a mano che si rilasciano operazioni, decommentare in modo che venga aggiunta in lista (oppure aggiungere in lista, se assente)</remarks>
    Public Shared Function OperazioniDisponibiliDaTipoRicetta(qs_tipo_ricetta As enum_TipoRicetta_DB) As List(Of Integer)

        Dim filtro As New List(Of Integer)

        Select Case qs_tipo_ricetta
            Case enum_TipoRicetta_DB.Standard_Destinazioni

                'mancano rispetto sotto irrigazione e trappole, i rilievi sono in lista separata                
                filtro.Add(LAVCOD_ALTRE_OPERAZIONI)
                filtro.Add(LAVCOD_DISERBO)
                filtro.Add(LAVCOD_TRATTAMENTO_ANTIPARASSITARIO)
                filtro.Add(LAVCOD_TRATTAMENTO_FITOREGOLATORE)
                filtro.Add(LAVCOD_GEODISINFESTAZIONE)
                filtro.Add(LAVCOD_CONCIA_SEME)
                filtro.Add(LAVCOD_DISSECCAMENTO)
                filtro.Add(LAVCOD_DISTRIBUZIONE_CONCIME)
                filtro.Add(LAVCOD_FERTIRRIGAZIONE)
                filtro.Add(LAVCOD_TRATTAMENTO_ANTIBUTTERATURA)
                filtro.Add(LAVCOD_CONCIMAZIONE_FOGLIARE)
                filtro.Add(LAVCOD_DISTRIBUZIONE_AMMENDANTI)
                filtro.Add(LAVCOD_SARCHIATURA_CONCIMAZIONE)
                filtro.Add(LAVCOD_ARATURA)
                filtro.Add(LAVCOD_ANDANAMENTO)
                filtro.Add(LAVCOD_ASPORTAZIONE_ORGANI_INFETTI)
                filtro.Add(LAVCOD_ASSOLCATURA)
                filtro.Add(LAVCOD_CARICO_MANUALE_FRUTTA)
                filtro.Add(LAVCOD_CIMATURA)
                filtro.Add(LAVCOD_DIRADAMENTO_MANUALE)
                filtro.Add(LAVCOD_DISSODAMENTO)
                filtro.Add(LAVCOD_ERPICATURA)
                filtro.Add(LAVCOD_ERPICATURA_ROTANTE)
                filtro.Add(LAVCOD_ESPIANTO)
                filtro.Add(LAVCOD_ESTIRPATURA)
                filtro.Add(LAVCOD_FALCIACONDIZIONATURA)
                filtro.Add(LAVCOD_FALCIATURA_ERBAI)
                filtro.Add(LAVCOD_FORMAZIONE_ARGINELLI)
                filtro.Add(LAVCOD_FRANGIZOLLATURA)
                filtro.Add(LAVCOD_FRESATURA)
                filtro.Add(LAVCOD_GEBIATURA)
                filtro.Add(LAVCOD_IMBALLO_FIENO_ROTOLI)
                filtro.Add(LAVCOD_INTERRAMENTO_PAGLIE)
                filtro.Add(LAVCOD_INTERVENTO_ANTIBRINA)
                filtro.Add(LAVCOD_LAVORAZIONE_CONBINATA)
                filtro.Add(LAVCOD_LAVORAZIONE_TRA_FILA)
                filtro.Add(LAVCOD_LAVORAZIONE_SU_FILA)
                filtro.Add(LAVCOD_LEGATURA)
                filtro.Add(LAVCOD_LIVELLAMENTO)
                filtro.Add(LAVCOD_MANUTENZIONE_ARGINI)
                filtro.Add(LAVCOD_MESSA_DIMORA_PIANTE)
                filtro.Add(LAVCOD_MIETITREBBIATURA)
                filtro.Add(LAVCOD_MINIMUM_TILLAGE)
                filtro.Add(LAVCOD_PACCIAMATURA)
                filtro.Add(LAVCOD_POTATURA_SECCA)
                filtro.Add(LAVCOD_POTATURA_VERDE)
                filtro.Add(LAVCOD_PRESSATURA)
                filtro.Add(LAVCOD__RACCOLTA_LEGNA_POTATURA)
                filtro.Add(LAVCOD_RANGHINATURA)
                filtro.Add(LAVCOD_RINCALZATURA)
                filtro.Add(LAVCOD_RIPPATURA)
                filtro.Add(LAVCOD_RIPUNTATURA)
                filtro.Add(LAVCOD_RIVOLTAMENTO_FORAGGIO)
                filtro.Add(LAVCOD_ROMPICROSTA)
                filtro.Add(LAVCOD_RULLATURA)
                filtro.Add(LAVCOD_SARCHIATURA)
                filtro.Add(LAVCOD_SCARIFICATURA)
                filtro.Add(LAVCOD_SCASSO)
                filtro.Add(LAVCOD_SOD_SEDDING)
                filtro.Add(LAVCOD_TRINCIATURA)
                filtro.Add(LAVCOD_VANGATURA)
                filtro.Add(LAVCOD_ZAPPATURA)
                filtro.Add(LAVCOD_SEMINA)
                filtro.Add(LAVCOD_SOVESCIO)
                filtro.Add(LAVCOD_TRAPIANTO)
                filtro.Add(LAVCOD_RACCOLTA)
                filtro.Add(LAVCOD_PIRODISERBO)
                'filtro.Add(LAVCOD_RILIEVO_AVVERSITA_CAMPO)


                'Case Else


                '    filtro.Add(LAVCOD_DISERBO)
                '    filtro.Add(LAVCOD_TRATTAMENTO_ANTIPARASSITARIO)
                '    filtro.Add(LAVCOD_TRATTAMENTO_FITOREGOLATORE)
                '    filtro.Add(LAVCOD_GEODISINFESTAZIONE)
                '    filtro.Add(LAVCOD_CONCIA_SEME)
                '    filtro.Add(LAVCOD_DISSECCAMENTO)
                '    filtro.Add(LAVCOD_CONFUSIONE_SESSUALE)
                '    filtro.Add(LAVCOD_DISORIENTAMENTO_SESSUALE)
                '    'filtro.Add(LAVCOD_INSTALLAZIONE_TRAPPOLE)
                '    filtro.Add(LAVCOD_CATTURE_MASSA)
                '    filtro.Add(LAVCOD_DISTRIBUZIONE_CONCIME)
                '    filtro.Add(LAVCOD_FERTIRRIGAZIONE)
                '    filtro.Add(LAVCOD_TRATTAMENTO_ANTIBUTTERATURA)
                '    filtro.Add(LAVCOD_CONCIMAZIONE_FOGLIARE)
                '    filtro.Add(LAVCOD_DISTRIBUZIONE_AMMENDANTI)
                '    filtro.Add(LAVCOD_SARCHIATURA_CONCIMAZIONE)
                '    filtro.Add(LAVCOD_IRRIGAZIONE)
                '    filtro.Add(LAVCOD_ARATURA)
                '    filtro.Add(LAVCOD_ANDANAMENTO)
                '    filtro.Add(LAVCOD_ASPORTAZIONE_ORGANI_INFETTI)
                '    filtro.Add(LAVCOD_ASSOLCATURA)
                '    filtro.Add(LAVCOD_CARICO_MANUALE_FRUTTA)
                '    filtro.Add(LAVCOD_CIMATURA)
                '    filtro.Add(LAVCOD_DIRADAMENTO_MANUALE)
                '    filtro.Add(LAVCOD_DISSODAMENTO)
                '    filtro.Add(LAVCOD_ERPICATURA)
                '    filtro.Add(LAVCOD_ERPICATURA_ROTANTE)
                '    filtro.Add(LAVCOD_ESPIANTO)
                '    filtro.Add(LAVCOD_ESTIRPATURA)
                '    filtro.Add(LAVCOD_FALCIACONDIZIONATURA)
                '    filtro.Add(LAVCOD_FALCIATURA_ERBAI)
                '    filtro.Add(LAVCOD_FORMAZIONE_ARGINELLI)
                '    filtro.Add(LAVCOD_FRANGIZOLLATURA)
                '    filtro.Add(LAVCOD_FRESATURA)
                '    filtro.Add(LAVCOD_GEBIATURA)
                '    filtro.Add(LAVCOD_IMBALLO_FIENO_ROTOLI)
                '    filtro.Add(LAVCOD_INTERRAMENTO_PAGLIE)
                '    filtro.Add(LAVCOD_INTERVENTO_ANTIBRINA)
                '    filtro.Add(LAVCOD_LAVORAZIONE_CONBINATA)
                '    filtro.Add(LAVCOD_LAVORAZIONE_TRA_FILA)
                '    filtro.Add(LAVCOD_LAVORAZIONE_SU_FILA)
                '    filtro.Add(LAVCOD_LEGATURA)
                '    filtro.Add(LAVCOD_LIVELLAMENTO)
                '    filtro.Add(LAVCOD_MANUTENZIONE_ARGINI)
                '    filtro.Add(LAVCOD_MESSA_DIMORA_PIANTE)
                '    filtro.Add(LAVCOD_MIETITREBBIATURA)
                '    filtro.Add(LAVCOD_MINIMUM_TILLAGE)
                '    filtro.Add(LAVCOD_PACCIAMATURA)
                '    filtro.Add(LAVCOD_POTATURA_SECCA)
                '    filtro.Add(LAVCOD_POTATURA_VERDE)
                '    filtro.Add(LAVCOD_PRESSATURA)
                '    filtro.Add(LAVCOD__RACCOLTA_LEGNA_POTATURA)
                '    filtro.Add(LAVCOD_RANGHINATURA)
                '    filtro.Add(LAVCOD_RINCALZATURA)
                '    filtro.Add(LAVCOD_RIPPATURA)
                '    filtro.Add(LAVCOD_RIPUNTATURA)
                '    filtro.Add(LAVCOD_RIVOLTAMENTO_FORAGGIO)
                '    filtro.Add(LAVCOD_ROMPICROSTA)
                '    filtro.Add(LAVCOD_RULLATURA)
                '    filtro.Add(LAVCOD_SARCHIATURA)
                '    filtro.Add(LAVCOD_SCARIFICATURA)
                '    filtro.Add(LAVCOD_SCASSO)
                '    filtro.Add(LAVCOD_SOD_SEDDING)
                '    filtro.Add(LAVCOD_TRINCIATURA)
                '    filtro.Add(LAVCOD_VANGATURA)
                '    filtro.Add(LAVCOD_ZAPPATURA)
                '    'filtro.Add(LAVCOD_SEMINA)
                '    filtro.Add(LAVCOD_SOVESCIO)
                '    'filtro.Add(LAVCOD_TRAPIANTO)
                '    'filtro.Add(LAVCOD_RILIEVO_AVVERSITA_CAMPO)

        End Select


        Return filtro


    End Function


    ''' <summary>
    ''' Estrae lista Operazioni
    ''' </summary>
    ''' <param name="dbcontext"></param>
    ''' <returns>lista di specie</returns>
    Public Function EstraiListaOperazioni(TipoRicetta As enum_TipoRicetta_DB, listaGruppiOperazioni As List(Of Integer), TipoRicetta_APP As enum_TipoRicetta_APP) As List(Of AgronicaCoreModelloSTD.Operazione)

        Dim xLettura As New Operazioni_R()

        Dim ListaOperazioniNonFiltata As List(Of AgronicaCoreModelloSTD.Operazione) =
            xLettura.EstraiListaOperazioni(dbContext, listaGruppiOperazioni)

        Dim rval As List(Of AgronicaCoreModelloSTD.Operazione)

        Select Case TipoRicetta_APP
            Case enum_TipoRicetta_APP.RilieviNatiSuAPP
                rval = ListaOperazioniNonFiltata.Where(Function(op) OperazioniRilieviDisponibiliDaTipoRicetta(TipoRicetta).Contains(op.lav_cod)).ToList
            Case Else
                rval = ListaOperazioniNonFiltata.Where(Function(op) OperazioniDisponibiliDaTipoRicetta(TipoRicetta).Contains(op.lav_cod)).ToList
        End Select

        Return rval

    End Function


    Public Function EstraiListaOperazioniAttivita(ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Extra_Campagna As Integer) As List(Of AgronicaCoreModelloSTD.OperazioneAttivita)

        Dim AttivitaBIZ = New AgronicaCoreAnagrafeStdBIZ.Attivita(dbContext)
        Dim listaAttivitaOperazioni = AttivitaBIZ.EstraiListaAttivitaPerOperazione(Piva)

        Dim listaAttivita = AttivitaBIZ.EstraiListaAttivitaFiltrataPerOperazione(Piva, 0)
        For Each attivita In listaAttivita
            If listaAttivitaOperazioni.Where(Function(x) x.attivita IsNot Nothing AndAlso x.attivita.Cod = attivita.Cod).Count = 0 Then
                listaAttivitaOperazioni.Add(
                    New AgronicaCoreModelloSTD.OperazioneAttivita With {
                        .attivita = attivita
                    })
            End If
        Next

        ' se Extra_Campagna = 1 escludo operazioni di campagna non associate ad attivita
        If Extra_Campagna = 0 Then
            Dim listaOperazioni = EstraiListaOperazioni(enum_TipoRicetta_DB.Standard_Destinazioni, New List(Of Integer), enum_TipoRicetta_APP.Ricette)
            For Each operazione In listaOperazioni
                If listaAttivitaOperazioni.Where(Function(x) x.operazione IsNot Nothing AndAlso x.operazione.lav_cod = operazione.lav_cod).Count = 0 Then
                    listaAttivitaOperazioni.Add(
                    New AgronicaCoreModelloSTD.OperazioneAttivita With {
                        .operazione = operazione
                    })
                End If
            Next
        End If

        ' Rimuovo le attività da escludere per il centro aziendale
        Dim listaAttivitaEscluse = AttivitaBIZ.EstraiListaAttivitaFiltrataPerCentroAziendale(Piva, Sa_Cod, 0)
        If listaAttivitaEscluse.Count > 0 Then
            Dim listaCodiciAttivita As List(Of Integer) = (From a In listaAttivitaEscluse Select a.Cod).ToList()
            listaAttivitaOperazioni = listaAttivitaOperazioni.Where(Function(x) x.attivita Is Nothing OrElse Not listaCodiciAttivita.Contains(x.attivita.Cod)).ToList()
        End If

        ' Lascio solo le attività da includere per il centro aziendale
        Dim listaAttivitaIncluse = AttivitaBIZ.EstraiListaAttivitaFiltrataPerCentroAziendale(Piva, Sa_Cod, 1)
        If listaAttivitaIncluse.Count > 0 Then
            Dim listaCodiciAttivita As List(Of Integer) = (From a In listaAttivitaIncluse Select a.Cod).ToList()
            listaAttivitaOperazioni = listaAttivitaOperazioni.Where(Function(x) x.attivita Is Nothing OrElse listaCodiciAttivita.Contains(x.attivita.Cod)).ToList()
        End If

        Return listaAttivitaOperazioni.OrderBy(Function(x) x.Descrizione).ToList()

    End Function

    Public Function LeggiOperazioni() As List(Of APP_Operazioni)

        Dim xLettura = New Operazioni_R()
        Return xLettura.Leggi(dbContext)

    End Function

    Public Function LeggiOperazione(lav_cod As Integer) As APP_Operazioni

        Dim xLettura = New Operazioni_R()
        Return xLettura.Leggi(dbContext, lav_cod)

    End Function
    Public Sub ScriviOperazioni(listOperazioni As List(Of APP_Operazioni), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numOperazioni As Integer = listOperazioni.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Operazioni_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each operazione In listOperazioni
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numOperazioni
            xScrittura.Scrivi(dbContext, operazione, commit)
        Next

    End Sub

    Public Sub CancellaOperazioni(operazione As APP_Operazioni)

        Dim xScrittura = New Operazioni_W()
        xScrittura.Cancella(dbContext, operazione)

    End Sub

End Class
