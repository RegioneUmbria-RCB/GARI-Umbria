Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate
Imports AgronicaCoreModelloSTD
Imports AgronicaCoreContabStdDAL
Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports System.Text

Public Class TempiRisorse

    ''' <summary>
    ''' Lettura tempi risorse
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="Ricetta"></param>
    ''' <param name="TempiRisorse_Cod"></param>
    ''' <returns></returns>
    Public Function Leggi(dbContext As GiasDbContext, Ricetta As AgronicaCoreModelloSTD.Ricette, TempiRisorse_Cod As Integer) As AgronicaCoreModelloSTD.TempiRisorse

        If Ricetta IsNot Nothing AndAlso Ricetta.ricetta_cod <> 0 Then
            Dim letturaRiferimenti As New TempiRisorse_Riferimenti_R
            Dim listaRiferimenti = letturaRiferimenti.Leggi(dbContext, Ricetta.ricetta_cod)
            If listaRiferimenti.Count > 0 Then
                TempiRisorse_Cod = listaRiferimenti.First().Id_Cdg_Generale_Rif
            End If
        End If

        If TempiRisorse_Cod <> 0 Then
            Dim lettura As New TempiRisorse_R
            Dim letturaImprese As New Imprese_R
            Dim letturaCentri As New Reg_Impianti_R
            Dim letturaAttivita As New Attivita_R
            Dim letturaProgetto As New ImputazioniFasi_R
            Dim letturaImpianto As New Reg_Impianti_R
            Dim letturaPersone As New Contatti_R
            Dim letturaMacchine As New Parco_Macchine_R
            Dim letturaMovimenti As New TempiRisorse_Movimenti_R
            Dim listaTempiRisorse = lettura.Leggi(dbContext, TempiRisorse_Cod)
            Dim listaMovimenti = letturaMovimenti.Leggi(dbContext, TempiRisorse_Cod)

            If listaTempiRisorse.Count > 0 AndAlso listaMovimenti.Count > 0 Then
                Dim appTempiRisorse = listaTempiRisorse.FirstOrDefault
                Dim TempiRisorse As New AgronicaCoreModelloSTD.TempiRisorse
                TempiRisorse.Id_Cdg_Generale = appTempiRisorse.Id_Cdg_Generale
                TempiRisorse.DataInserimento = appTempiRisorse.Data_Inserimento
                TempiRisorse.Bozza = appTempiRisorse.Bozza
                TempiRisorse.Note = appTempiRisorse.Note
                Dim azienda = letturaImprese.Leggi(dbContext, appTempiRisorse.Piva)
                If azienda.Count > 0 Then
                    TempiRisorse.Azienda = azienda.FirstOrDefault
                End If
                Dim listaCentri = letturaCentri.EstraiListaCentriAziendali(dbContext, appTempiRisorse.Piva)
                If Ricetta IsNot Nothing AndAlso TempiRisorse.RicettaOperazione Is Nothing Then
                    TempiRisorse.Ricetta = Ricetta
                    TempiRisorse.RicettaOperazione = Ricetta.RicetteOperazioni.FirstOrDefault
                End If
                Dim Movimenti As New List(Of TempiRisorse_Movimenti)
                Dim Impianti As New List(Of APP_Reg_Impianti)
                Dim ListaPersone As New List(Of String)
                For Each appMovimento In listaMovimenti
                    Dim Movimento As New TempiRisorse_Movimenti
                    Movimento.Id_CDG_Movimenti = appMovimento.Id_CDG_Movimenti
                    If TempiRisorse.Attivita Is Nothing Then
                        Dim attivita = letturaAttivita.Leggi(dbContext, appMovimento.Id_Attivita, 0).FirstOrDefault
                        TempiRisorse.Attivita = New Attivita With {.Cod = attivita.Id_Attivita, .Descrizione = attivita.Desc}
                        If appMovimento.Id_Imputazione <> 0 Then
                            Dim progetto = letturaProgetto.Leggi(dbContext, appMovimento.Id_Attivita, appMovimento.Id_Imputazione, "").FirstOrDefault
                            TempiRisorse.Progetto = progetto
                        End If
                        If appMovimento.sa_cod <> 0 Then
                            Dim centro = listaCentri.Find(Function(i) i.Piva = appMovimento.Piva AndAlso i.Sa_Cod = appMovimento.sa_cod)
                            TempiRisorse.CentroAziendale = centro
                        Else
                            TempiRisorse.CentroAziendale = listaCentri.FirstOrDefault
                        End If
                    End If
                    If appMovimento.id_reg <> 0 Then
                        Dim impianto = letturaImpianto.LeggiImpianto(dbContext, appMovimento.Piva, appMovimento.sa_cod, appMovimento.appezza, appMovimento.id_reg)
                        Movimento.Impianto = impianto
                        If impianto IsNot Nothing AndAlso Not Impianti.Contains(impianto) Then
                            Impianti.Add(impianto)
                        End If
                    End If
                    Movimento.Attivita = TempiRisorse.Attivita
                    Movimento.Progetto = TempiRisorse.Progetto
                    Movimento.DataOraInizio = appMovimento.Data_Ora_Inizio
                    Movimento.DataOraFine = appMovimento.Data_Ora_Fine
                    Movimento.Qta = appMovimento.Qta
                    If appMovimento.Cod_RisUm <> 0 Then
                        Dim persona = letturaPersone.Leggi(dbContext, appTempiRisorse.Piva, appMovimento.Cod_RisUm).FirstOrDefault
                        If persona IsNot Nothing Then
                            Movimento.RisorsaAziendale = New Risorsa_Contatti With {.Risorsa_Des = persona.Cognome & " " & persona.Nome, .APP_Contatto = persona}
                            If Not ListaPersone.Contains(Movimento.RisorsaAziendale.Risorsa_Des) Then
                                ListaPersone.Add(Movimento.RisorsaAziendale.Risorsa_Des)
                            End If
                        End If
                    ElseIf appMovimento.Mac_Cod <> 0 Then
                        Dim macchina = letturaMacchine.Leggi(dbContext, appTempiRisorse.Piva, appMovimento.Mac_Cod, appMovimento.Data_Ora_Inizio.Date).FirstOrDefault
                        If macchina IsNot Nothing Then
                            Movimento.RisorsaAziendale = New Risorsa_Macchine With {.Risorsa_Des = macchina.Mac_Des, .Macchina = macchina}
                        End If
                    End If
                    If Movimento.RisorsaAziendale IsNot Nothing Then
                        Movimenti.Add(Movimento)
                    End If
                Next
                TempiRisorse.Movimenti = Movimenti
                TempiRisorse.Impianti = Impianti
                TempiRisorse.DescrizionePersone = String.Join(", ", ListaPersone.ToArray())
                Return TempiRisorse
            End If
        End If

        Return Nothing

    End Function

    Public Function LeggiAttivita(dbContext As GiasDbContext, Piva As String) As List(Of APP_CDG_Generale)
        Dim TempiRisorseDAL As New AgronicaCoreContabStdDAL.TempiRisorse_R
        Return TempiRisorseDAL.Leggi(dbContext, Piva)
    End Function

    Public Function LeggiAttivitaMovimenti(dbContext As GiasDbContext, Piva As String) As List(Of APP_CDG_Movimenti)
        Dim TempiRisorseDAL As New AgronicaCoreContabStdDAL.TempiRisorse_Movimenti_R
        Return TempiRisorseDAL.Leggi(dbContext, Piva)
    End Function

    Public Function LeggiAttivitaOperazioni(dbContext As GiasDbContext, Piva As String) As List(Of APP_Riferimenti_Interventi_Cdg)
        Dim TempiRisorseDAL As New AgronicaCoreContabStdDAL.TempiRisorse_Riferimenti_R
        Return TempiRisorseDAL.Leggi(dbContext, Piva)
    End Function

    Public Function LeggiPerEsclusioneBozze(dbContext As GiasDbContext, Piva As String) As List(Of Integer)
        Dim TempiRisorseDAL As New AgronicaCoreContabStdDAL.TempiRisorse_R
        Return TempiRisorseDAL.LeggiBozze(dbContext, Piva)
    End Function

    Public Function LeggiPerCancellazione(dbContext As GiasDbContext, Piva As String) As List(Of Integer)
        Dim TempiRisorseDAL As New AgronicaCoreContabStdDAL.TempiRisorse_R
        Return TempiRisorseDAL.LeggiPerCancellazione(dbContext, Piva)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="ricette_attivita"></param>
    ''' <param name="gestione_bozze"></param>
    ''' <param name="Ricerca"></param>    
    ''' <returns></returns>
    Public Function EstraiListaRicetteAttivita(dbContext As GiasDbContext, ricette_attivita As Boolean, gestione_bozze As Boolean, Ricerca As String, tipoRicetta As enum_TipoRicetta_APP) As List(Of Ricette_Attivita)

        Dim listaRicette = New List(Of AgronicaCoreModelloSTD.Ricette_Operazioni)
        Dim listaAttivita = EstraiListaAttivita(dbContext, False, 0, Ricerca)


        If Not ricette_attivita AndAlso tipoRicetta = enum_TipoRicetta_APP.RilieviNatiSuAPP Then
            Dim leggiRicette As New AgronicaCoreContabStdBIZ.Ricette_Operazioni
            Dim stato = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita
            listaRicette = leggiRicette.EstraiListaOperazioni(dbContext, tipoRicetta, stato, "", Ricerca)
        End If


        If ricette_attivita Then
            Dim leggiRicette As New AgronicaCoreContabStdBIZ.Ricette_Operazioni
            Dim stato = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita
            listaRicette = leggiRicette.EstraiListaOperazioni(dbContext, tipoRicetta, stato, "", Ricerca)
            Dim escludiRicette As List(Of Integer) = (From r In listaAttivita Where r.RicettaOperazione IsNot Nothing Select r.RicettaOperazione.Ricetta_Operazione_Cod).ToList
            listaRicette = (From ricetta In listaRicette Where Not escludiRicette.Contains(ricetta.Ricetta_Operazione_Cod) Select ricetta).ToList()

            ' forza lo stato a bozza se non ci sono attività collegate
            If gestione_bozze Then
                Dim AttivitaBIZ As New AgronicaCoreAnagrafeStdBIZ.Attivita(dbContext)
                Dim scritturaStato As New Ricette_Operazioni_W
                For Each ricetta In listaRicette
                    Dim listaAttivitaOperazione = AttivitaBIZ.EstraiListaAttivitaFiltrataPerOperazione(ricetta.CentroAziendale.Piva, ricetta.Operazione.lav_cod)
                    If listaAttivitaOperazione.Count > 0 AndAlso ricetta.Bozza = 0 Then
                        scritturaStato.ImpostaBozzaSuOperazione(dbContext, ricetta.Ricetta_Operazione_Cod, 1)
                        ricetta.Bozza = 1
                    End If
                Next
            End If

        End If

        Dim listaRicetteAttivita As List(Of Ricette_Attivita)
        listaRicetteAttivita = (
                From ricetta In listaRicette
                Select New Ricette_Attivita With {.Ricetta = ricetta, .DataRiferimento = ricetta.DataRiferimento}
            ).Concat(
                From attivita In listaAttivita
                Select New Ricette_Attivita With {.Ricetta = attivita.RicettaOperazione, .Attivita = attivita, .DataRiferimento = attivita.DataInserimento}
        ).OrderBy(Function(f) f.DataRiferimento).ToList()

        Return listaRicetteAttivita

    End Function

    ''' <summary>
    ''' Lettura attività con possibilità di filtrare solo quelle extra campagna
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="extraCampagna"></param>
    Public Function EstraiListaAttivita(dbContext As GiasDbContext, extraCampagna As Boolean, Id_Cdg_Generale As Integer, Ricerca As String) As List(Of AgronicaCoreModelloSTD.TempiRisorse)
        Dim leggiTempiRisorse As New AgronicaCoreContabStdBIZ.TempiRisorse
        Dim leggiAttivita As New AgronicaCoreContabStdDAL.TempiRisorse_R
        Dim leggiAttivitaRicette As New AgronicaCoreContabStdDAL.TempiRisorse_Riferimenti_R
        Dim leggiRicette As New AgronicaCoreContabStdBIZ.Ricette
        Dim listaAttivita As New List(Of AgronicaCoreModelloSTD.TempiRisorse)

        Dim attivitaRicette = leggiAttivitaRicette.Leggi(dbContext, 0)
        Dim listaAttivitaApp = leggiAttivita.Leggi(dbContext, Id_Cdg_Generale)
        Dim letturaImpianti As New AgronicaCoreAnagrafeStdDAL.Reg_Impianti_R

        For Each attivitaAPP In listaAttivitaApp
            Dim ricettaAttivita = attivitaRicette.Find(Function(i) i.Id_Cdg_Generale_Rif = attivitaAPP.Id_Cdg_Generale)
            If Not extraCampagna OrElse ricettaAttivita Is Nothing Then

                Dim attivita = leggiTempiRisorse.Leggi(dbContext, Nothing, attivitaAPP.Id_Cdg_Generale)

                If attivita IsNot Nothing Then

                    Dim listaImpianti As List(Of APP_Reg_Impianti) = Nothing

                    If ricettaAttivita IsNot Nothing Then
                        attivita.Ricetta = leggiRicette.Leggi(dbContext, ricettaAttivita.Ricetta_Cod, ricettaAttivita.Ricetta_Operazione_Cod)
                        attivita.RicettaOperazione = attivita.Ricetta.RicetteOperazioni.FirstOrDefault
                        If attivita.RicettaOperazione IsNot Nothing Then
                            attivita.RicettaOperazione.CentroAziendale = attivita.CentroAziendale
                            listaImpianti = letturaImpianti.LeggiDatoRicettaOperazioneCod(dbContext, attivita.RicettaOperazione.Ricetta_Operazione_Cod)
                        End If
                    Else
                        listaImpianti = letturaImpianti.LeggiDatoTempiRisorseCod(dbContext, attivita.Id_Cdg_Generale)
                    End If

                    ' aggiungo descrizione impianti
                    If listaImpianti IsNot Nothing Then
                        attivita.DescrizioneImpianti = String.Join(" | ", (From iii In listaImpianti Select iii.app_nome).ToArray)
                    End If

                    listaAttivita.Add(attivita)

                End If
            End If
        Next

        ' filtro ricerca libero
        If Not String.IsNullOrEmpty(Ricerca) Then
            listaAttivita = listaAttivita.Where(Function(i) String.IsNullOrEmpty(Ricerca) OrElse i.TestoRicerca.ToLower.Contains(Ricerca.ToLower)).ToList()
        End If

        Return listaAttivita

    End Function

    ''' <summary>
    ''' Scrittura dei tempi risorse
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="TempiRisorse"></param>
    Public Sub Scrivi(dbContext As GiasDbContext, TempiRisorse As AgronicaCoreModelloSTD.TempiRisorse, username As String)

        Dim APP_CDG_Generale As New APP_CDG_Generale
        Dim APP_CDG_Riferimenti_Interventi As APP_Riferimenti_Interventi_Cdg = Nothing
        Dim listaMovimenti As New List(Of APP_CDG_Movimenti)

        Dim agroSequenze As New AgronicaCoreDataProviderSTD.Agro_Sequenze

        If TempiRisorse.Id_Cdg_Generale = 0 Then
            TempiRisorse.Id_Cdg_Generale =
                -(agroSequenze.NuovoId_Tabella_EF(dbContext, "tempirisorse", AgroSequenzeBase0, AgroSequenzeEndUpperBound))
            dbContext.SaveChanges()
        End If

        APP_CDG_Generale.Id_Cdg_Generale = TempiRisorse.Id_Cdg_Generale
        APP_CDG_Generale.Data_Inserimento = TempiRisorse.DataInserimento
        APP_CDG_Generale.Piva = TempiRisorse.Azienda.piva
        APP_CDG_Generale.Bozza = TempiRisorse.Bozza
        APP_CDG_Generale.Note = TempiRisorse.Note

        For Each movimento In TempiRisorse.Movimenti

            Dim APP_CDG_Movimento As New APP_CDG_Movimenti

            If movimento.Id_CDG_Movimenti = 0 Then
                movimento.Id_CDG_Movimenti =
                    -(agroSequenze.NuovoId_Tabella_EF(dbContext, "tempirisorse_movimenti", AgroSequenzeBase0, AgroSequenzeEndUpperBound))
                dbContext.SaveChanges()
            End If

            APP_CDG_Movimento.Id_CDG_Movimenti = movimento.Id_CDG_Movimenti
            APP_CDG_Movimento.Id_Cdg_Generale = TempiRisorse.Id_Cdg_Generale
            APP_CDG_Movimento.Piva = TempiRisorse.Azienda.piva
            APP_CDG_Movimento.Id_Attivita = movimento.Attivita.Cod
            If TempiRisorse.CentroAziendale IsNot Nothing Then
                APP_CDG_Movimento.sa_cod = TempiRisorse.CentroAziendale.Sa_Cod
            End If
            If movimento.Progetto IsNot Nothing Then
                APP_CDG_Movimento.Id_Imputazione = movimento.Progetto.Imputazione_Cod
            ElseIf movimento.Impianto IsNot Nothing Then
                APP_CDG_Movimento.sa_cod = movimento.Impianto.sa_cod
                APP_CDG_Movimento.appezza = movimento.Impianto.appezza
                APP_CDG_Movimento.id_reg = movimento.Impianto.id_reg
                APP_CDG_Movimento.progetto_cod = movimento.Impianto.progetto_cod
            End If
            APP_CDG_Movimento.Data_Ora_Inizio = movimento.DataOraInizio
            APP_CDG_Movimento.Data_Ora_Fine = movimento.DataOraFine
            APP_CDG_Movimento.Qta = movimento.Qta

            If TypeOf movimento.RisorsaAziendale Is Risorsa_Contatti Then
                Dim risorsa = CType(movimento.RisorsaAziendale, Risorsa_Contatti)
                APP_CDG_Movimento.Cod_RisUm = risorsa.APP_Contatto.Cod_RisUm
                APP_CDG_Movimento.NrBadge = risorsa.APP_Contatto.NrBadge
            ElseIf TypeOf movimento.RisorsaAziendale Is Risorsa_Macchine Then
                Dim risorsa = CType(movimento.RisorsaAziendale, Risorsa_Macchine)
                APP_CDG_Movimento.Mac_Cod = risorsa.Macchina.Mac_Cod
            End If

            listaMovimenti.Add(APP_CDG_Movimento)

        Next

        If TempiRisorse.Ricetta IsNot Nothing Then

            APP_CDG_Riferimenti_Interventi = New APP_Riferimenti_Interventi_Cdg
            APP_CDG_Riferimenti_Interventi.Id_Cdg_Generale_Rif = TempiRisorse.Id_Cdg_Generale
            APP_CDG_Riferimenti_Interventi.Piva = TempiRisorse.Azienda.piva
            APP_CDG_Riferimenti_Interventi.Ricetta_Cod = TempiRisorse.Ricetta.ricetta_cod
            APP_CDG_Riferimenti_Interventi.Ricetta_Operazione_Cod = TempiRisorse.RicettaOperazione.Ricetta_Operazione_Cod

        End If

        Using transaction = dbContext.Database.BeginTransaction()

            Try

                If TempiRisorse.Id_Cdg_Generale <> 0 Then
                    CancellaTempiRisorse(dbContext, TempiRisorse.Id_Cdg_Generale)
                End If

                ScritturaDatiComuni(APP_CDG_Generale, username)
                dbContext.APP_CDG_Generale.Add(APP_CDG_Generale)
                dbContext.SaveChanges()

                For Each movimento In listaMovimenti
                    ScritturaDatiComuni(movimento, username)
                    dbContext.APP_CDG_Movimenti.Add(movimento)
                Next
                dbContext.SaveChanges()

                If APP_CDG_Riferimenti_Interventi IsNot Nothing Then

                    ScritturaDatiComuni(APP_CDG_Riferimenti_Interventi, username)
                    dbContext.APP_Riferimenti_Interventi_Cdg.Add(APP_CDG_Riferimenti_Interventi)
                    dbContext.SaveChanges()

                    ' aggiorna stato bozza ricetta collegata
                    Dim scritturaStato As New Ricette_Operazioni_W
                    scritturaStato.ImpostaBozzaSuOperazione(dbContext, APP_CDG_Riferimenti_Interventi.Ricetta_Operazione_Cod, TempiRisorse.Bozza)

                End If

                dbContext.SaveChanges()
                transaction.Commit()

            Catch ex As Exception
                transaction.Rollback()
                Throw ex
            End Try
        End Using

    End Sub

    Public Sub CancellaTempiRisorse(dbContext As GiasDbContext, Id_Cdg_Generale As Integer)

        Dim TempiRisorseW As New TempiRisorse_W
        Dim TempiRisorseMovimentiW As New TempiRisorse_Movimenti_W
        Dim TempiRisorseRiferimentiW As New TempiRisorse_Riferimenti_W

        TempiRisorseRiferimentiW.Cancella(dbContext, Id_Cdg_Generale)
        TempiRisorseMovimentiW.Cancella(dbContext, Id_Cdg_Generale)
        TempiRisorseW.Cancella(dbContext, Id_Cdg_Generale)

    End Sub

    Public Sub ImpostaBozzaRicettaAttivita(dbContext As GiasDbContext, Ricetta_Operazione As APP_Ricette_Operazioni)

        If Ricetta_Operazione IsNot Nothing Then
            Dim TempiRisorseRiferimentiW As New TempiRisorse_Riferimenti_R
            Dim listaRiferimenti = TempiRisorseRiferimentiW.Leggi(dbContext, Ricetta_Operazione.Ricetta_Cod)
            If listaRiferimenti.Count > 0 Then
                Dim TempiRisorseW As New TempiRisorse_W
                Dim Id_Cdg_Generale = listaRiferimenti.First().Id_Cdg_Generale_Rif
                TempiRisorseW.ImpostaBozzaSuAttivita(dbContext, Id_Cdg_Generale, Ricetta_Operazione.Bozza)
            End If
        End If

    End Sub

    Public Sub ImpostaInterventiDaInviare(dbContext As GiasDbContext, data_riferimento As Date)

        Dim listaInterventi = EstraiListaRicetteAttivita(dbContext, True, False, "", enum_TipoRicetta_APP.NatiSuAPP)

        For Each intervento In listaInterventi
            Dim differenza_ore = data_riferimento.Subtract(intervento.DataRiferimento).TotalHours
            If differenza_ore > 24 Then
                If intervento.Ricetta IsNot Nothing AndAlso intervento.Ricetta.Bozza = 1 Then
                    Dim RicetteOperazioniW As New Ricette_Operazioni_W
                    RicetteOperazioniW.ImpostaBozzaSuOperazione(dbContext, intervento.Ricetta.Ricetta_Operazione_Cod, 0)
                End If
                If intervento.Attivita IsNot Nothing AndAlso intervento.Attivita.Bozza = 1 Then
                    Dim TempiRisorseW As New TempiRisorse_W
                    TempiRisorseW.ImpostaBozzaSuAttivita(dbContext, intervento.Attivita.Id_Cdg_Generale, 0)
                End If
            End If
        Next

    End Sub

    Public Sub CancellaTempiRisorse(dbContext As GiasDbContext, Piva As String)

        Dim TempiRisorseW As New TempiRisorse_W
        Dim TempiRisorseMovimentiW As New TempiRisorse_Movimenti_W
        Dim TempiRisorseRiferimentiW As New TempiRisorse_Riferimenti_W

        TempiRisorseRiferimentiW.Cancella(dbContext, Piva)
        TempiRisorseMovimentiW.Cancella(dbContext, Piva)
        TempiRisorseW.Cancella(dbContext, Piva)

    End Sub

    Public Sub CancellaAttivitaConTransazione(dbContext As GiasDbContext, Id_Cdg_Generale As Integer)

        Using transaction = dbContext.Database.BeginTransaction()
            Try
                CancellaTempiRisorse(dbContext, Id_Cdg_Generale)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw ex
            End Try
        End Using

    End Sub

    Private Sub ScritturaDatiComuni(oggettoDoveScrivere As APP_Agronica_Entity, username As String)

        Dim dataora As Date = System.DateTime.Now()

        oggettoDoveScrivere.Data_Creazione = dataora
        oggettoDoveScrivere.Data_Modifica = dataora
        oggettoDoveScrivere.Username_Creazione = username
        oggettoDoveScrivere.Username_Modifica = username

        'valori non memorizzati danno luogo a data "01/01/0001", quindi allineo su agro-data.inizio/fine

        If oggettoDoveScrivere.Validita_Inizio <AGRODATAINIZIO Then
            oggettoDoveScrivere.Validita_Inizio= AGRODATAINIZIO
        End If

        If oggettoDoveScrivere.Validita_Fine < AGRODATAINIZIO Then
            oggettoDoveScrivere.Validita_Fine = AGRODATAFINE
        End If

    End Sub

End Class
