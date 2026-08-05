Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello.OperazioneAgenda_Temp

Public Class Lavorazione

    Sub New()
        CifreArrotondamento = 6
        IdLavCollegata = 0
        IdOrdCollegato = 0
        DataCreazioneMovDett = AgroDataInizializzata
        DataModificaMovDett = AgroDataInizializzata
        duplicaCalCod = False
    End Sub

    Public Structure ChiaviDaModificare
        Dim Tabella As String
        Dim ColonnaChiave As String
        Dim ValoreChiaveVecchia As String
        Dim ValoreChiaveNuova As String
    End Structure

    Public Property CifreArrotondamento As Integer

    Public Property DataMovimento As Date?

    Public Property DataLettura As DateTime?

    Public Property DataModifica As DateTime?

    Public Property UsernameModifica As String

    Public Property Piva As String

    Public Property SaCod As Integer?

    Public Property IdAgenda As Integer?

    Public Property IdMov As Integer?

    Public Property IdMovDet As Integer?

    Public Property IdDestinazione As Integer?

    Public Property TipoDestinazione As Integer?

    Public Property TipoLavorazioneDes As String

    Public Property DescrizioneAggiuntiva As String

    Public Property LineaCod As Integer?

    Public Property PreparazioneCod As Integer?

    Public Property IdTrasformazione As Integer?

    Public Property ElemCod As Integer?

    Public Property MatCod As Integer?

    Public Property MatDes As String

    Public Property UdmCod As Integer?

    Public Property UdmCodExtra As Integer?

    Public Property Qta As Decimal?

    Public Property CalCod As Integer?

    Public Property MateriePrimeCampionature As List(Of Materia_Prima_Campionatura)

    Public Property Lotto As String

    Public Property JollyInt As Integer?

    Public Property QtaExtra As Decimal?

    Public Property QtaExtraTotale As Decimal?

    Public Property Tara As Decimal?

    Public Property NumContenitori As Decimal?

    Public Property NumImballaggi As Decimal?

    Public Property CodMacchinaLav As String

    '-- Per pilotare data creazione/modifica movimenti dettagli
    Public Property DataCreazioneMovDett As DateTime

    Public Property DataModificaMovDett As DateTime

    '-- Per Modifica Cella in Testata e nelle Righe di Uscita
    Public Property New_saCod As Integer?

    '-- Per Modifica Cella nelle Righe di Uscita
    Public Property New_tipoDestinazione As Integer?

    '-- Per Modifica Cella nelle Righe di Uscita
    Public Property New_idDestinazione As Integer?

    '---- START Per Piano Lavoro Etichette ----
    Public Property Descrizione As String

    Public Property Note As String

    '---- END   Per Piano Lavoro Etichette ----

    '---- START Proprietà Generiche - Jolly ----
    Public Property Flag1 As Boolean

    '---- END   Proprietà Generiche - Jolly ----

    'Contiene le chiavi che si vogliono modificare (nome colonna db), con il loro nuovo valore 
    '  Nome Tabella, Nome Colonna, Vecchia Chiave, Nuova Chiave
    ' (es: "Mov_destinazioni","Id_Destinazione","7896523","12345678")
    ' valore chiave memorizzata come stringa per coprire tutti i casi.
    Public Property ListaChiaviDaModificare As List(Of ChiaviDaModificare)

    Public Property Proprieta_Estese As Dictionary(Of String, Object)

    Public Property Linea_Macchina_Lavorazione As Linee_Macchine_Lavorazione

    Public Property Movimenti As List(Of Movimento)

    Public Property Movimenti_Dettagli As List(Of Movimento_Dettaglio)

    'Collegamenti a livello di testata

    Public Property IdLavCollegata As Integer?

    Public Property IdOrdCollegato As Integer?

    'Riferimenti riga agenda collegata

    Public Property pivaRif As String

    Public Property saCodRif As Integer

    Public Property idAgendaRif As Integer

    Public Property idMovRif As Integer

    Public Property idMovDetRif As Integer

    Public Property lavCodRif As Integer

    'Forza creazione nuovo CalCod copiando quello passato

    Public Property duplicaCalCod As Boolean

    Public Function RiferimentiRigaAgendaPresenti() As Boolean

        Dim riferimentiPresenti As Boolean = False

        If Not String.IsNullOrEmpty(pivaRif) AndAlso idAgendaRif > 0 AndAlso idMovDetRif > 0 Then
            riferimentiPresenti = True
        End If

        Return riferimentiPresenti

    End Function

End Class

Public Class LavorazioniHelper

    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri

    Private Const descrizioneLavorazione As String = "Lavorazione"
    Private Const descrizioneOrdineLavorazione As String = "Ordine Lavorazione"

    ''' <summary>
    ''' Costruttore LavorazioniHelper con Flag Sottoscorta ### NON USARE ###
    ''' </summary>
    ''' <param name="objParametriServer">Oggetto parametri server</param>
    ''' <param name="objParametriUtenti">Oggetto parametri utenti</param>
    ''' <param name="flagSottoscortaConsentito">NON USATO: i controlli sono attivati dai relativi parametri di profilazione</param>
    ''' 
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri,
                   ByVal flagSottoscortaConsentito As Boolean)

        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti

    End Sub

    ''' <summary>
    ''' Costruttore LavorazioniHelper
    ''' </summary>
    ''' <param name="objParametriServer">Oggetto parametri server</param>
    ''' <param name="objParametriUtenti">Oggetto parametri utenti</param>
    ''' 
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri)

        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti

    End Sub

#Region "Funzioni Pubbliche Lettura Lavorazioni"

    Public Function LeggiLavorazione(ByVal piva As String,
                                     ByVal saCod As Integer,
                                     ByVal idAgenda As Integer
                                     ) As Lavorazione

        Return LeggiOrdineLavorazioneCore(piva, saCod, idAgenda, LAVCOD_TRASFORMAZIONI)

    End Function

    Public Function LeggiOrdine(ByVal piva As String,
                                ByVal saCod As Integer,
                                ByVal idAgenda As Integer
                                ) As Lavorazione

        Return LeggiOrdineLavorazioneCore(piva, saCod, idAgenda, LAVCOD_TESTATE_ORDINE_LAVORAZIONE)

    End Function


    Public Function LeggiOrdineLavorazione(ByVal piva As String,
                                           ByVal saCod As Integer,
                                           ByVal idAgenda As Integer,
                                           ByVal lavCod As Integer
                                           ) As Lavorazione

        Return LeggiOrdineLavorazioneCore(piva, saCod, idAgenda, lavCod)

    End Function

    Private Function LeggiLavorazioneTestata(ByVal piva As String,
                                             ByVal saCod As Integer,
                                             ByVal idAgenda As Integer
                                             ) As Lavorazione

        'Imposto le opzioni di lettura ai dati minimi necessari

        Dim opzLetturaMovimenti = New Opzioni_Lettura_Movimenti With {
            .LeggiMovimentiDettagli = True,
            .FiltroMovimentiDettagli = enum_FiltroMovimentiDettagli.DatiTestataLavorazione,
            .LeggiMovimentiDettagliTecnici = False,
            .LeggiMovimentiDettagliTecniciExtra = False,
            .LeggiPagamenti = False
        }

        Dim opzLetturaAgenda = New Opzioni_Lettura_Agenda With {
            .LeggiGps = False,
            .LeggiNote = False,
            .LeggiRiferimenti = False,
            .OpzioniLetturaMovimenti = opzLetturaMovimenti
        }

        Return LeggiOrdineLavorazioneCore(piva,
                                          0,
                                          idAgenda,
                                          LAVCOD_TRASFORMAZIONI,
                                          opzioniLetturaAgenda:=opzLetturaAgenda)

    End Function

    Private Function LeggiOrdineLavorazioneCore(ByVal piva As String,
                                                ByVal saCod As Integer,
                                                ByVal idAgenda As Integer,
                                                ByVal lavCod As Integer,
                                                Optional ByVal opzioniLetturaAgenda As Opzioni_Lettura_Agenda = Nothing
                                                ) As Lavorazione

        If IsNothing(opzioniLetturaAgenda) Then
            opzioniLetturaAgenda = New Opzioni_Lettura_Agenda
        End If

        Dim flagConnessione As Boolean = False

        Dim objLavorazione As Lavorazione

        Dim agendaHelper As New Agenda_Operazione_Helper
        Dim objAgenda As Operazione_Agenda
        Dim objMov As Movimento
        Dim objMovLinPro As Movimento
        Dim objMovDet As Movimento_Dettaglio

        Try

            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            objAgenda = agendaHelper.Leggi(piva,
                                           saCod,
                                           idAgenda,
                                           lavCod,
                                           _objParametriServer,
                                           opzioniLetturaAgenda:=opzioniLetturaAgenda)

            If Not IsNothing(objAgenda) Then

                'Ricerco il movimento di carico
                If Not IsNothing(objAgenda.Movimenti) AndAlso objAgenda.Movimenti.Count > 0 Then
                    'TODO: verifica che sia aperta?!? in realtà solo se sono in modifica?!? in lettura non dovrei avere problemi
                    objMov = objAgenda.Movimenti.Find(Function(x) x.Cau_Mov = CAU_CARICO)
                Else
                    Throw New Exception("L'operazione non contiene Movimenti.")
                End If

                'Non ho trovato il movimento di carico
                If IsNothing(objMov) Then
                    Throw New Exception("L'operazione non contiene il Movimento di Carico.")
                End If

                objMovLinPro = objAgenda.Movimenti.Find(Function(x) x.Cau_Mov = CAU_LINEA_PRODUZIONE)
                If IsNothing(objMovLinPro) Then
                    Throw New Exception("L'operazione non contiene il Movimento di Linea Produzione.")
                End If

                'Ricerco il movimento di carico che non movimenta il magazzino
                If Not IsNothing(objMov.Movimenti_Dettagli) AndAlso objMov.Movimenti_Dettagli.Count > 0 Then
                    objMovDet = objMov.Movimenti_Dettagli.Find(Function(x) x.Jolly_Int = MagazzinoNONMovimentato AndAlso x.Extra_Str <> "" AndAlso x.Extra_Int > 0)
                Else
                    Throw New Exception("Il Movimento di Carico non contiene nessun dettaglio.")
                End If

                'Non c'è il record di Lavorazione / Ordine Lavorazione
                If IsNothing(objMovDet) Then
                    Dim messaggioErrore = "Operazione non contenente {0}; impossibile visualizzare."
                    Dim descrOrdLav = OttieniDescrizioneLavCod(lavCod)
                    Throw New Exception(String.Format(messaggioErrore, descrOrdLav))
                End If

                objLavorazione = New Lavorazione With {
                    .DataMovimento = objAgenda.Data,
                    .Piva = objAgenda.Piva,
                    .TipoLavorazioneDes = PreparazioneDesFromPreparazioneCod(objAgenda.Piva, objAgenda.Preparazione_Cod),
                    .DescrizioneAggiuntiva = RicavoDescrizioneAggiuntiva(objAgenda.Des_Lib, objMovDet.Lotto),
                    .LineaCod = objAgenda.Linea_Cod,
                    .PreparazioneCod = objAgenda.Preparazione_Cod,
                    .IdTrasformazione = objAgenda.Id_Trasformazione,
                    .DataLettura = Now,
                    .SaCod = objMovDet.Sa_Cod,
                    .ElemCod = objMovDet.Elem_Cod,
                    .MatCod = objMovDet.Mat_Cod,
                    .MatDes = MatDesFromMatCod(objAgenda.Piva, objMovDet.Elem_Cod, objMovDet.Mat_Cod),
                    .Qta = objMovDet.Qta,
                    .UdmCod = objMovDet.Udm_Cod,
                    .UdmCodExtra = objMovDet.Udm_Cod_Extra,
                    .JollyInt = objMovDet.Jolly_Int,
                    .TipoDestinazione = CInt(objMovDet.Extra_Str),
                    .IdDestinazione = objMovDet.Extra_Int,
                    .Lotto = objMovDet.Lotto,
                    .QtaExtra = objMovDet.Qta_Extra,
                    .QtaExtraTotale = objMovDet.Qta_Extra_Totale,
                    .NumContenitori = objMovDet.Qta_Dettaglio1,
                    .NumImballaggi = objMovDet.Qta_Dettaglio2,
                    .CalCod = objMovDet.Cal_Cod,
                    .CodMacchinaLav = objMovLinPro.Cod_Macchina_Lav
                }

                'Leggo e passo le materie prime campionature
                If Not IsNothing(objMovDet.Materie_Prime_Campionature) AndAlso objMovDet.Materie_Prime_Campionature.Count > 0 Then
                    objLavorazione.MateriePrimeCampionature = New List(Of Materia_Prima_Campionatura)
                    objLavorazione.MateriePrimeCampionature.AddRange(objMovDet.Materie_Prime_Campionature)
                End If

                objLavorazione.Movimenti = objAgenda.Movimenti
                objLavorazione.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)
                For Each m As Movimento In objAgenda.Movimenti
                    objLavorazione.Movimenti_Dettagli.AddRange(m.Movimenti_Dettagli)
                Next

            Else
                Throw New Exception("L'operazione non esiste o non è una Lavorazione.")
            End If

        Catch ex As Exception
            Throw New Exception("[ LavorazioniHelper.LeggiOrdineLavorazione() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objLavorazione

    End Function

    Public Function LeggiScaricoCarico(ByVal piva As String,
                                       ByVal saCod As Integer,
                                       ByVal idAgenda As Integer,
                                       ByVal idMov As Integer,
                                       ByVal idMovDet As Integer
                                       ) As Lavorazione

        Dim flagConnessione As Boolean = False

        Dim objMovDetHelper As New Agenda_Movimenti_Dettagli_Helper
        Dim listMovDet As List(Of Movimento_Dettaglio)
        Dim objLavorazione As Lavorazione = Nothing

        Try

            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            listMovDet = objMovDetHelper.Leggi(piva, saCod, idAgenda, idMov, idMovDet, _objParametriServer)

            If Not IsNothing(listMovDet) Then

                If listMovDet.Count = 0 Then
                    Throw New Exception("La lettura ha restituito zero scarichi")
                ElseIf listMovDet.Count > 1 Then
                    Throw New Exception("La lettura ha restituito più di uno scarico")
                End If

                Dim objMovDet As Movimento_Dettaglio = listMovDet(0)

                objLavorazione = New Lavorazione With {
                    .Piva = objMovDet.Piva,
                    .SaCod = objMovDet.Sa_Cod,
                    .IdMov = objMovDet.Id_Mov,
                    .IdMovDet = objMovDet.Id_Mov_Det,
                    .ElemCod = objMovDet.Elem_Cod,
                    .MatCod = objMovDet.Mat_Cod,
                    .MatDes = MatDesFromMatCod(objMovDet.Piva, objMovDet.Elem_Cod, objMovDet.Mat_Cod),
                    .UdmCod = objMovDet.Udm_Cod,
                    .UdmCodExtra = objMovDet.Udm_Cod_Extra,
                    .Qta = objMovDet.Qta,
                    .DataModifica = objMovDet.Data_Modifica,
                    .UsernameModifica = objMovDet.Username_Modifica,
                    .DataLettura = Now,
                    .DataMovimento = objMovDet.Validita_Inizio,
                    .CalCod = objMovDet.Cal_Cod,
                    .Lotto = objMovDet.Lotto,
                    .JollyInt = objMovDet.Jolly_Int,
                    .QtaExtra = objMovDet.Qta_Extra,
                    .QtaExtraTotale = objMovDet.Qta_Extra_Totale,
                    .Tara = objMovDet.Tara,
                    .NumContenitori = objMovDet.Qta_Dettaglio1,
                    .NumImballaggi = objMovDet.Qta_Dettaglio2
                }

                If Not IsNothing(objMovDet.Movimenti_Destinazioni) Then

                    If objMovDet.Movimenti_Destinazioni.Count = 0 Then
                        Throw New Exception("Non è presente nessun Movimento_Destinazione")
                    ElseIf objMovDet.Movimenti_Destinazioni.Count > 1 Then
                        Throw New NotImplementedException("Sono presenti più Movimenti_Destinazioni")
                    End If

                    Dim objMovDest As Movimento_Destinazione = objMovDet.Movimenti_Destinazioni(0)
                    objLavorazione.IdDestinazione = objMovDest.Id_Destinazione
                    objLavorazione.TipoDestinazione = objMovDest.Tipo

                    'Verifica di consistenza tra i dati di mov det e mov dest
                    If objMovDet.Qta_Dettaglio1 <> objMovDest.Qta_Dest1 Then
                        Throw New Exception("Qta_Dettaglio1 <> Qta_Dest_1")
                    End If

                    If objMovDet.Qta_Dettaglio2 <> objMovDest.Qta_Dest2 Then
                        Throw New Exception("Qta_Dettaglio2 <> Qta_Dest_2")
                    End If

                End If

                If Not IsNothing(objMovDet.Materie_Prime_Campionature) AndAlso objMovDet.Materie_Prime_Campionature.Count > 0 Then
                    objLavorazione.MateriePrimeCampionature = New List(Of Materia_Prima_Campionatura)
                    objLavorazione.MateriePrimeCampionature.AddRange(objMovDet.Materie_Prime_Campionature)
                End If

            End If

        Catch ex As Exception
            Throw New Exception("[ LavorazioniHelper.LeggiScaricoCarico() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objLavorazione

    End Function

#End Region

#Region "Funzioni Pubbliche Inserimento Lavorazioni"
    ''' <summary>
    ''' Scrive un'operazione Agenda con movimento di testata, di scarico e di carico + Mov Dettaglio di Carico (Jolly_Int = 1) e Materie Prime Campionature
    ''' </summary>
    ''' <param name="objLavorazione">Oggetto lavorazione (verrà usata la funzione CheckPropertyTestataLavorazione per controllare la presenza dei campi obbligatori)</param>
    ''' <param name="messaggioDettagliato">True restituisce errore più dettagliato nel controllo giacenze</param>
    ''' <param name="msgError">Controllo giacenza e record nel frattempo modificato generano un messaggio d'errore, non eccezione</param>
    ''' <returns>Id_Agenda scritto, oppure 0 in caso di errore</returns>
    Public Function ScriviTestataLavorazione(ByVal objLavorazione As Lavorazione,
                                             ByVal messaggioDettagliato As Boolean,
                                             ByRef msgError As String,
                                             lavCod As Integer
                                             ) As Integer

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim objAgenda As Operazione_Agenda
        Dim idAgenda As Integer = 0
        Dim flagOggettoCorretto As Boolean

        Try
            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyTestataLavorazione(objLavorazione, enum_TipoOperazioneDB.Scrittura)

            If flagOggettoCorretto Then
                objAgenda = GeneraAperturaAgendaLavorazione(lavCod,
                                                            objLavorazione.DataMovimento,
                                                            objLavorazione.Piva,
                                                            objLavorazione.SaCod,
                                                            objLavorazione.IdDestinazione,
                                                            objLavorazione.TipoDestinazione,
                                                            objLavorazione.TipoLavorazioneDes,
                                                            objLavorazione.DescrizioneAggiuntiva,
                                                            objLavorazione.LineaCod,
                                                            objLavorazione.PreparazioneCod,
                                                            objLavorazione.IdTrasformazione,
                                                            objLavorazione.ElemCod,
                                                            objLavorazione.MatCod,
                                                            objLavorazione.MatDes,
                                                            objLavorazione.UdmCod,
                                                            objLavorazione.UdmCodExtra,
                                                            objLavorazione.Qta,
                                                            objLavorazione.JollyInt,
                                                            objLavorazione.Lotto,
                                                            objLavorazione.QtaExtra,
                                                            objLavorazione.QtaExtraTotale,
                                                            objLavorazione.NumContenitori,
                                                            objLavorazione.NumImballaggi,
                                                            objLavorazione.MateriePrimeCampionature,
                                                            objLavorazione.CodMacchinaLav,
                                                            objLavorazione.IdLavCollegata,
                                                            objLavorazione.IdOrdCollegato)

                If Not IsNothing(objAgenda) Then
                    Dim objAgendaHelper As New Agenda_Operazione_Helper
                    idAgenda = objAgendaHelper.Scrivi(objAgenda, _objParametriServer, flagUsaOraReale:=True, scriviRiferimenti:=True)
                End If

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ LavorazioniHelper.ScriviTestataLavorazione() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return idAgenda

    End Function

    ''' <summary>
    ''' Scrive una testata lavorazione collegata ad un ordine e scrive il dettaglio scarico
    ''' </summary>
    ''' <param name="objLavorazione"></param>
    ''' <param name="messaggioDettagliato"></param>
    ''' <param name="msgError"></param>
    ''' <returns>Ritorna oggetto anonimo con idAgenda e idMovDet creati</returns>
    Public Function ScriviTestataLavorazioneConDettaglioScarico(ByVal objLavorazione As Lavorazione,
                                                                ByVal messaggioDettagliato As Boolean,
                                                                ByRef msgError As String
                                                               ) As Object
        Dim idAgenda As Integer = 0
        Dim idMovDet As Integer = 0
        Const nomeFunzione = "[ LavorazioniHelper.ScriviTestataLavorazioneConDettaglioScarico() ]"

        'Apertura connessione e transazione
        ConnessioniTransazioni.ApriConnessione(True, _objParametriServer)

        Try

            'Creazione testata lavorazione da ordine
            idAgenda = CreazioneTestataLavorazioneDaOrdine(objLavorazione, 0, messaggioDettagliato, msgError)

            'Creazione scarico
            objLavorazione.IdAgenda = idAgenda
            idMovDet = ScriviDettaglioScarico(objLavorazione, messaggioDettagliato, msgError)
            If idMovDet = 0 Then
                Throw New Exception("Riferimento movimento scarico lavorazione non valido")
            End If

            'Commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, _objParametriServer)

        Catch ex As Exception
            'Rollback transazione
            ConnessioniTransazioni.ChiudiTransazione(2, _objParametriServer)
            Throw New Exception(nomeFunzione & " : " & ex.Message)
        Finally
            'Chiusura connessione
            ConnessioniTransazioni.ChiudiConnessione(_objParametriServer)

        End Try

        Return New With
            {
            .idAgenda = idAgenda,
            .idMovDet = idMovDet
            }
    End Function

    ''' <summary>
    ''' Scrive una testata lavorazione collegata ad un ordine e scrive il dettaglio carico
    ''' </summary>
    ''' <param name="objLavorazione"></param>
    ''' <param name="messaggioDettagliato"></param>
    ''' <param name="msgError"></param>
    ''' <returns>Ritorna oggetto anonimo con idAgenda e idMovDet creati</returns>

    Public Function ScriviTestataLavorazioneConDettaglioCarico(ByVal objLavorazione As Lavorazione,
                                                               ByVal messaggioDettagliato As Boolean,
                                                               ByRef msgError As String
                                                              ) As Object

        Dim idAgenda As Integer = 0
        Dim idMovDet As Integer = 0
        Dim idAgendaScaricoColleg As Integer = 0
        Dim idMovDetScaricoColleg As Integer = 0
        Const nomeFunzione = "[ LavorazioniHelper.ScriviTestataLavorazioneConDettaglioCarico() ]"

        'Apertura connessione e transazione
        ConnessioniTransazioni.ApriConnessione(True, _objParametriServer)

        Try

            'Creazione testata lavorazione da ordine
            idAgenda = CreazioneTestataLavorazioneDaOrdine(objLavorazione, 0, messaggioDettagliato, msgError)

            'Creazione carico
            objLavorazione.IdAgenda = idAgenda
            idMovDet = ScriviDettaglioCarico(objLavorazione, messaggioDettagliato, msgError,
                                             idAgendaScaricoColleg:=idAgendaScaricoColleg,
                                             idMovDetScaricoColleg:=idMovDetScaricoColleg,
                                             lavCod:=LAVCOD_TRASFORMAZIONI)
            If idMovDet = 0 Then
                Throw New Exception("Riferimento movimento carico lavorazione non valido")
            End If

            'Commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, _objParametriServer)

        Catch ex As Exception
            'Rollback transazione
            ConnessioniTransazioni.ChiudiTransazione(2, _objParametriServer)
            Throw New Exception(nomeFunzione & " : " & ex.Message)
        Finally
            'Chiusura connessione
            ConnessioniTransazioni.ChiudiConnessione(_objParametriServer)

        End Try

        Return New With
            {
            .idAgenda = idAgenda,
            .idMovDet = idMovDet,
            .idAgendaScaricoColleg = idAgendaScaricoColleg,
            .idMovDetScaricoColleg = idMovDetScaricoColleg
            }
    End Function

    Private Function CreazioneTestataLavorazioneDaOrdine(ByVal objLavorazione As Lavorazione,
                                                         ByVal idLavCollegata As Integer,
                                                         ByVal messaggioDettagliato As Boolean,
                                                         ByRef msgError As String) As Integer
        Dim idAgenda As Integer = 0

        Dim pivaOrdine = objLavorazione.Piva
        Dim idAgendaOrdine = objLavorazione.IdAgenda
        Dim objTestataOrdine = LeggiOrdine(pivaOrdine, 0, idAgendaOrdine)
        If IsNothing(objTestataOrdine) Then
            Throw New Exception("Ordine di lavoro non trovato")
        End If

        Dim MateriePrimeCampionatureLav = New List(Of Materia_Prima_Campionatura)
        For Each mpcOrd In objTestataOrdine.MateriePrimeCampionature
            Dim mpcLav As New Materia_Prima_Campionatura With {.Tipo = mpcOrd.Tipo,
                                                               .Tipo_Cod = mpcOrd.Tipo_Cod,
                                                               .ChkTara_Campionatura = mpcOrd.ChkTara_Campionatura,
                                                               .Tara_Campionatura = mpcOrd.Tara_Campionatura
                                                              }
            MateriePrimeCampionatureLav.Add(mpcLav)
        Next

        Dim objTestataLav As New Lavorazione With {.Piva = objTestataOrdine.Piva,
                                                   .SaCod = objTestataOrdine.SaCod,
                                                   .ElemCod = objTestataOrdine.ElemCod,
                                                   .MatCod = objTestataOrdine.MatCod,
                                                   .IdDestinazione = objTestataOrdine.IdDestinazione,
                                                   .TipoDestinazione = objTestataOrdine.TipoDestinazione,
                                                   .PreparazioneCod = objTestataOrdine.PreparazioneCod,
                                                   .LineaCod = objTestataOrdine.LineaCod,
                                                   .TipoLavorazioneDes = objTestataOrdine.TipoLavorazioneDes,
                                                   .MatDes = objTestataOrdine.MatDes,
                                                   .UdmCod = objTestataOrdine.UdmCod,
                                                   .UdmCodExtra = objTestataOrdine.UdmCodExtra,
                                                   .NumImballaggi = objTestataOrdine.NumImballaggi,
                                                   .NumContenitori = objTestataOrdine.NumContenitori,
                                                   .JollyInt = MagazzinoNONMovimentato,
                                                   .Qta = 0,
                                                   .QtaExtra = 1,
                                                   .QtaExtraTotale = 0,
                                                   .DataMovimento = Date.Today(),
                                                   .Lotto = objTestataOrdine.Lotto,
                                                   .MateriePrimeCampionature = MateriePrimeCampionatureLav,
                                                   .CodMacchinaLav = objTestataOrdine.CodMacchinaLav,
                                                   .IdLavCollegata = idLavCollegata,
                                                   .IdOrdCollegato = idAgendaOrdine}

        idAgenda = ScriviTestataLavorazione(objTestataLav, messaggioDettagliato, msgError, LAVCOD_TRASFORMAZIONI)
        If idAgenda = 0 Then
            Throw New Exception("Riferimento testata lavorazione non valido")
        End If

        Return idAgenda
    End Function

    ''' <summary>
    ''' Scrive uno scarico (Movimento Dettaglio, Mov Destinazione + Eventuale Materie Prime Campionature)
    ''' </summary>
    ''' <param name="objLavorazione">Oggetto lavorazione (verrà usata la funzione CheckPropertyCaricoScarico per controllare la presenza dei campi obbligatori)</param>
    ''' <param name="messaggioDettagliato">True restituisce errore più dettagliato nel controllo giacenze</param>
    ''' <param name="msgError">Controllo giacenza e record nel frattempo modificato generano un messaggio d'errore, non eccezione</param>
    ''' <returns>Id_Mov_Det scritto, oppure 0 in caso di errore</returns>
    Public Function ScriviDettaglioScarico(ByVal objLavorazione As Lavorazione,
                                           ByVal messaggioDettagliato As Boolean,
                                           ByRef msgError As String
                                           ) As Integer

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = True

        Dim objMovDet As Movimento_Dettaglio
        Dim idMovDet As Integer = 0
        Dim flagOggettoCorretto As Boolean

        Try
            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyCaricoScarico(objLavorazione,
                                                             enum_TipoOperazioneDB.Scrittura,
                                                             flagUtilizzaDestinazione:=True)

            If flagOggettoCorretto Then
                objMovDet = GeneraDettaglioCaricoScarico(CAU_SCARICO,
                                                         objLavorazione.Piva,
                                                         objLavorazione.SaCod,
                                                         objLavorazione.IdAgenda,
                                                         objLavorazione.IdDestinazione,
                                                         objLavorazione.TipoDestinazione,
                                                         objLavorazione.TipoLavorazioneDes,
                                                         objLavorazione.ElemCod,
                                                         objLavorazione.MatCod,
                                                         objLavorazione.MatDes,
                                                         objLavorazione.UdmCod,
                                                         objLavorazione.UdmCodExtra,
                                                         objLavorazione.Qta,
                                                         objLavorazione.CalCod,
                                                         objLavorazione.MateriePrimeCampionature,
                                                         objLavorazione.Lotto,
                                                         objLavorazione.JollyInt,
                                                         Agro_Math.ArrotondaVal(objLavorazione.QtaExtra, objLavorazione.CifreArrotondamento),
                                                         objLavorazione.QtaExtraTotale,
                                                         objLavorazione.Tara,
                                                         objLavorazione.NumContenitori,
                                                         objLavorazione.NumImballaggi,
                                                         flagCreaDestinazione:=True,
                                                         objLavorazione.DataCreazioneMovDett,
                                                         objLavorazione.DataModificaMovDett)

                If Not IsNothing(objMovDet) Then

                    'TODO: UDM?
                    Dim numConfezioni As Decimal = 0
                    If objLavorazione.UdmCod = enum_UnitaMisura.Numero AndAlso objLavorazione.Qta > 0 Then
                        numConfezioni = objLavorazione.Qta
                    End If

                    'TODO: fare anche verifica della giacenza prima di scrivere
                    If Not IsNothing(objLavorazione.CalCod) AndAlso objLavorazione.CalCod <> 0 Then

                        Dim risGiacenzaCheck As String = ""
                        Dim CheckGiacenze = UtilityHelper.SeCheckGiacenze(objLavorazione.ElemCod, _objParametriUtenti)

                        If CheckGiacenze.eseguiCheckGiacenze Then
                            risGiacenzaCheck = ControllaGiacenza(objLavorazione, objMovDet,
                                                                 Now, objLavorazione.CalCod,
                                                                 numConfezioni,
                                                                 objLavorazione.NumContenitori,
                                                                 objLavorazione.NumImballaggi,
                                                                 objLavorazione.QtaExtraTotale,
                                                                 (objLavorazione.QtaExtraTotale + objLavorazione.Tara),
                                                                 messaggioDettagliato,
                                                                 CheckGiacenze.flag_QtaNoZero,
                                                                 CheckGiacenze.flag_QtaMaggioreZero)
                        End If

                        If risGiacenzaCheck <> "" Then
                            msgError &= "Impossibile creare: Il prodotto non è più disponibile. " & risGiacenzaCheck
                            xRisp = False
                        End If

                    End If

                    If xRisp = True Then
                        Dim objMovDetHelper As New Agenda_Movimenti_Dettagli_Helper
                        idMovDet = objMovDetHelper.Scrivi(objMovDet, _objParametriServer)
                    End If

                    If idMovDet <> 0 Then
                        'devo andare ad aggiornare i contatori giacenze in materie prime campionature

                        'per farlo devo sapere qual è il cal_cod che è stato assegnato nel caso mi fosse stata passata la lista e non il cal_cod
                        If objLavorazione.CalCod = 0 Then
                            objLavorazione.CalCod = GetCalCod(objLavorazione.Piva, objLavorazione.SaCod, objLavorazione.IdAgenda, 0, idMovDet,
                                                              Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)
                        End If

                        AggiornaGiacenzeCampionature(CAU_SCARICO,
                                                     objLavorazione.CalCod,
                                                     numConfezioni,
                                                     objLavorazione.NumContenitori,
                                                     objLavorazione.NumImballaggi)

                    End If

                End If

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ LavorazioniHelper.ScriviDettaglioScarico() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return idMovDet

    End Function

    ''' <summary>
    ''' Scrive un carico (Movimento Dettaglio, Mov Destinazione + Eventuale Materie Prime Campionature)
    ''' </summary>
    ''' <param name="objLavorazione">Oggetto lavorazione (verrà usata la funzione CheckPropertyCaricoScarico per controllare la presenza dei campi obbligatori)</param>
    ''' <param name="messaggioDettagliato">True restituisce errore più dettagliato nel controllo giacenze</param>
    ''' <param name="msgError">Controllo giacenza e record nel frattempo modificato generano un messaggio d'errore, non eccezione</param>
    ''' <returns>Id_Mov_Det scritto, oppure 0 in caso di errore</returns>
    Public Function ScriviDettaglioCarico(ByVal objLavorazione As Lavorazione,
                                          ByVal messaggioDettagliato As Boolean,
                                          ByRef msgError As String,
                                          Optional ByRef idAgendaScaricoColleg As Integer = 0,
                                          Optional ByRef idMovDetScaricoColleg As Integer = 0,
                                          Optional ByVal lavCod As Integer = LAVCOD_TRASFORMAZIONI
                                          ) As Integer

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim objMovDet As Movimento_Dettaglio
        Dim idMovDet As Integer = 0
        Dim flagOggettoCorretto As Boolean

        idAgendaScaricoColleg = 0
        idMovDetScaricoColleg = 0

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyCaricoScarico(objLavorazione,
                                                             enum_TipoOperazioneDB.Scrittura,
                                                             flagUtilizzaDestinazione:=True)

            If flagOggettoCorretto Then

                If objLavorazione.duplicaCalCod Then
                    If Not IsNothing(objLavorazione.CalCod) AndAlso objLavorazione.CalCod <> 0 Then
                        CaricaMateriePrimeCampionatureDaCalCod(objLavorazione)
                    Else
                        Throw New Exception("CalCod non passato: duplicazione non riuscita")
                    End If
                End If

                objMovDet = GeneraDettaglioCaricoScarico(CAU_CARICO,
                                                         objLavorazione.Piva,
                                                         objLavorazione.SaCod,
                                                         objLavorazione.IdAgenda,
                                                         objLavorazione.IdDestinazione,
                                                         objLavorazione.TipoDestinazione,
                                                         objLavorazione.TipoLavorazioneDes,
                                                         objLavorazione.ElemCod,
                                                         objLavorazione.MatCod,
                                                         objLavorazione.MatDes,
                                                         objLavorazione.UdmCod,
                                                         objLavorazione.UdmCodExtra,
                                                         objLavorazione.Qta,
                                                         objLavorazione.CalCod,
                                                         objLavorazione.MateriePrimeCampionature,
                                                         objLavorazione.Lotto,
                                                         objLavorazione.JollyInt,
                                                         Agro_Math.ArrotondaVal(objLavorazione.QtaExtra, objLavorazione.CifreArrotondamento),
                                                         objLavorazione.QtaExtraTotale,
                                                         objLavorazione.Tara,
                                                         objLavorazione.NumContenitori,
                                                         objLavorazione.NumImballaggi,
                                                         flagCreaDestinazione:=True,
                                                         objLavorazione.DataCreazioneMovDett,
                                                         objLavorazione.DataModificaMovDett)

                If Not IsNothing(objMovDet) Then

                    Dim objMovDetHelper As New Agenda_Movimenti_Dettagli_Helper

                    'Forza uso data modifica passata
                    Dim usaDataModifica As Boolean
                    If objLavorazione.DataModificaMovDett = AgroDataInizializzata Then
                        usaDataModifica = False
                    Else
                        usaDataModifica = True
                    End If

                    'Caricamento riferimenti riga agenda collegata
                    If objLavorazione.RiferimentiRigaAgendaPresenti() Then
                        AggiungiRiferimentiRigaAgendaCollegata(objMovDet, objLavorazione, lavCod)
                    End If

                    'Scrittura carico
                    idMovDet = objMovDetHelper.Scrivi(objMovDet, _objParametriServer, flagUtilizzaDataModifica:=usaDataModifica)

                    If idMovDet <> 0 Then
                        'devo andare ad aggiornare i contatori giacenze in materie prime campionature
                        'per farlo devo sapere qual è il cal_cod che è stato assegnato nel caso mi fosse stata passata la lista e non il cal_cod
                        If objLavorazione.CalCod = 0 Then
                            objLavorazione.CalCod = GetCalCod(objLavorazione.Piva, objLavorazione.SaCod, objLavorazione.IdAgenda, 0, idMovDet,
                                                              Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)
                        End If

                        'TODO: UDM? --> questo metodo di tenere le giacenze non ha più senso
                        Dim numConfezioni As Decimal = 0
                        If objLavorazione.UdmCod = enum_UnitaMisura.Numero AndAlso objLavorazione.Qta > 0 Then
                            numConfezioni = objLavorazione.Qta
                        End If
                        AggiornaGiacenzeCampionature(CAU_CARICO,
                                                     objLavorazione.CalCod,
                                                     numConfezioni,
                                                     objLavorazione.NumContenitori,
                                                     objLavorazione.NumImballaggi)

                        'Gestione inserimento scarico per lavorazione collegata
                        If objLavorazione.JollyInt = MagazzinoMovimentato Then
                            SeGeneraScaricoLavorazioneCollegata(objLavorazione, messaggioDettagliato, _objParametriServer,
                                                                msgError, idAgendaScaricoColleg, idMovDetScaricoColleg)
                        End If

                    End If

                End If

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ LavorazioniHelper.ScriviDettaglioScarico() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return idMovDet

    End Function

    Private Sub CaricaMateriePrimeCampionatureDaCalCod(ByRef objLavorazione As Lavorazione)

        Dim objMateriePrimeCampionature As New Agenda_Materie_Prime_Campionature_Helper
        Dim listaMateriePrimeCampionature As List(Of Materia_Prima_Campionatura)

        listaMateriePrimeCampionature = objMateriePrimeCampionature.Leggi(objLavorazione.CalCod,
                                                                          "",
                                                                          0,
                                                                          0,
                                                                          _objParametriServer)

        If Not IsNothing(listaMateriePrimeCampionature) Then

            For Each mpc In listaMateriePrimeCampionature
                mpc.Progressivo_Origine = mpc.Progressivo
                mpc.Progressivo = 0
            Next

            objLavorazione.MateriePrimeCampionature = listaMateriePrimeCampionature

        End If

        objLavorazione.CalCod = 0

    End Sub

    Private Sub AggiungiRiferimentiRigaAgendaCollegata(objMovDet As Movimento_Dettaglio,
                                                       objLavorazione As Lavorazione,
                                                       lavCod As Integer)

        Dim movDetRif As New Movimento_Dettaglio_Riferimento With
            {
            .Piva_Rif = objLavorazione.pivaRif,
            .Sa_Cod_Rif = objLavorazione.saCodRif,
            .Id_Agenda_Rif = objLavorazione.idAgendaRif,
            .Id_Mov_Rif = objLavorazione.idMovRif,
            .Id_Mov_Det_Rif = objLavorazione.idMovDetRif,
            .Lav_Cod_Rif = objLavorazione.lavCodRif,
            .Cau_Mov_Rif = -1,
            .Qta = 0
            }

        objMovDet.Movimenti_Dettagli_Riferimenti.Add(movDetRif)

        'Se sto inserendo una uscita LAV da ODL, se non presente scrivo il legame di testata
        If lavCod = LAVCOD_TRASFORMAZIONI AndAlso objLavorazione.lavCodRif = LAVCOD_TESTATE_ORDINE_LAVORAZIONE Then
            Dim objMovDetRif As New Mov_Dettagli_Riferimenti_R
            Dim dtMDRifLavOrd As DataTable = objMovDetRif.LeggiPerAgenda(objLavorazione.Piva,
                                                                         0,
                                                                         objLavorazione.IdAgenda,
                                                                         lavCod,
                                                                         -1,
                                                                         "", "",
                                                                         _objParametriServer,
                                                                         leggiRiferimentiInversi:=False,
                                                                         objLavorazione.lavCodRif,
                                                                         -1)
            If dtMDRifLavOrd.Rows.Count = 0 Then
                Dim objMovDetRiferimenti = New Mov_Det_Riferimenti_W
                objMovDetRiferimenti.ScriviPerAgenda(objLavorazione.Piva,
                                                     objLavorazione.IdAgenda,
                                                     lavCod,
                                                     objLavorazione.pivaRif,
                                                     objLavorazione.idAgendaRif,
                                                     objLavorazione.lavCodRif,
                                                     objMovDet.Data,
                                                     _objParametriServer)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Se lavorazione collegata ad altra lavorazione, viene inserito in automatico lo scarico.
    ''' Se lavorazione generata da ordine:
    ''' 1. ricerca ordine collegato;
    ''' 2. generazione nuova testata lavorazione;
    ''' 3. inserimento automatico dello scarico.
    ''' </summary>
    Private Sub SeGeneraScaricoLavorazioneCollegata(ByVal lavCarico As Lavorazione,
                                                    ByVal messaggioDettagliato As Boolean,
                                                    ByVal objParametriServer As AgronicaCoreParametri,
                                                    ByRef msgError As String,
                                                    ByRef idAgendaScaricoColleg As Integer,
                                                    ByRef idMovDetScaricoColleg As Integer)

        'Ricerca lavorazione/ordine collegato

        Dim objLavBiz As New AgronicaCoreContabBIZ.FF_LavorazioneBIZ
        Dim agendaColleg As AgronicaCoreContabBIZ.FF_AgendaCollegBIZ = objLavBiz.Ricerca_LavOrd_Colleg(lavCarico.Piva,
                                                                                                       lavCarico.IdAgenda,
                                                                                                       objParametriServer)

        If agendaColleg.IdAgendaColleg > 0 Then

            Dim idAgendaScarico As Integer = 0

            If agendaColleg.LavCodColleg = LAVCOD_TESTATE_ORDINE_LAVORAZIONE Then
                'Creazione testata lavorazione da ordine
                Dim objLavOrd As New Lavorazione With {.Piva = lavCarico.Piva,
                                                       .IdAgenda = agendaColleg.IdAgendaColleg}
                idAgendaScarico = CreazioneTestataLavorazioneDaOrdine(objLavOrd, lavCarico.IdAgenda, messaggioDettagliato, msgError)
            Else
                idAgendaScarico = agendaColleg.IdAgendaColleg
            End If

            If idAgendaScarico > 0 Then

                '===================================================
                '=== CREAZIONE SCARICO (INGRESSO) DA CARICO (USCITA)
                '===================================================

                'Lettura dati testata lavorazione collegata

                Dim datiLavScarico As New Lavorazione

                datiLavScarico = LeggiLavorazioneTestata(lavCarico.Piva, 0, idAgendaScarico)

                'Caricamento dati da testata lavorazione collegata

                Dim inserimentoLavScarico As New Lavorazione With {
                    .IdAgenda = idAgendaScarico,
                    .Piva = datiLavScarico.Piva,
                    .SaCod = datiLavScarico.SaCod,
                    .JollyInt = MagazzinoMovimentato,
                    .TipoDestinazione = CELLA_FRIGORIFERA,
                    .IdDestinazione = lavCarico.IdDestinazione,
                    .ElemCod = lavCarico.ElemCod,
                    .MatCod = lavCarico.MatCod,
                    .MatDes = lavCarico.MatDes,
                    .TipoLavorazioneDes = datiLavScarico.TipoLavorazioneDes,
                    .CodMacchinaLav = datiLavScarico.CodMacchinaLav
                    }

                'Caricamento dati da lavorazione carico

                inserimentoLavScarico.UdmCod = lavCarico.UdmCod
                inserimentoLavScarico.UdmCodExtra = lavCarico.UdmCodExtra
                inserimentoLavScarico.CalCod = lavCarico.CalCod
                inserimentoLavScarico.Lotto = lavCarico.Lotto
                inserimentoLavScarico.Qta = lavCarico.Qta
                inserimentoLavScarico.QtaExtra = lavCarico.QtaExtra
                inserimentoLavScarico.QtaExtraTotale = lavCarico.QtaExtraTotale
                inserimentoLavScarico.Tara = lavCarico.Tara
                inserimentoLavScarico.NumContenitori = lavCarico.NumContenitori
                inserimentoLavScarico.NumImballaggi = lavCarico.NumImballaggi

                'Inserimento scarico

                Dim idMovDetScarico = ScriviDettaglioScarico(inserimentoLavScarico, messaggioDettagliato, msgError)

                idAgendaScaricoColleg = idAgendaScarico
                idMovDetScaricoColleg = idMovDetScarico

            End If

        End If

    End Sub

#End Region

#Region "Funzioni Pubbliche Modifica Lavorazioni"

    Public Function AggiornaTestataLavorazione(ByVal objLavorazione As Lavorazione,
                                               ByVal messaggioDettagliato As Boolean,
                                               ByRef msgError As String,
                                               lavCod As Integer
                                               ) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean

        Dim agendaHelper As New Agenda_Operazione_Helper
        Dim movDetHelper As New Agenda_Movimenti_Dettagli_Helper

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyTestataLavorazione(objLavorazione, enum_TipoOperazioneDB.Modifica)

            If flagOggettoCorretto Then

                'Prima di procedere con al modifica devo verificare se nel frattempo il record dell'agenda è cambiato
                Dim dataUltimaModifica As DateTime
                Dim usernameUltimaModifica As String = ""
                Dim dataMovimento As Date

                dataUltimaModifica = UtilityHelper.UltimaModificaAgenda(objLavorazione.Piva,
                                                                        objLavorazione.IdAgenda,
                                                                        usernameUltimaModifica,
                                                                        dataMovimento,
                                                                        _objParametriServer)

                If Not IsNothing(objLavorazione.DataLettura) AndAlso dataUltimaModifica > objLavorazione.DataLettura Then
                    msgError &= UtilityHelper.GetMessaggioDataModifica(dataUltimaModifica, usernameUltimaModifica, _objParametriUtenti)
                    xRisp = False
                Else

                    'Tab AGENDA =  KEY + Linea_Cod, Des_Lib (x Descrizione Aggiuntiva)
                    xRisp = agendaHelper.ModificaPuntuale(piva:=objLavorazione.Piva,
                                                          saCod:=0,
                                                          idAgenda:=objLavorazione.IdAgenda,
                                                          objParametri:=_objParametriServer,
                                                          lineaCod:=objLavorazione.LineaCod,
                                                          desLib:=AgronicaCoreContabBIZ.FF_LavorazioneBIZ.CreaDesLibLavorazione(objLavorazione.TipoLavorazioneDes, objLavorazione.MatDes, objLavorazione.Lotto, objLavorazione.DescrizioneAggiuntiva),
                                                          preparazioneCod:=objLavorazione.PreparazioneCod)

                    'Tab MOVIMENTO DETT FITTIZIO =  KEY + Elem_Cod, Mat_Cod, Mov_Det_Des, Qta, Udm_Cod, Extra_Int, Extra_Str, Lotto, Qta_Extra, Qta_Extra_Totale, Qta_Dettaglio1, Qta_Dettaglio2
                    If xRisp = True Then

                        Dim movDetDes As String = Nothing

                        If Not IsNothing(objLavorazione.MatCod) AndAlso objLavorazione.MatCod <> 0 AndAlso
                           Not IsNothing(objLavorazione.ElemCod) AndAlso objLavorazione.ElemCod <> 0 Then

                            Dim preparazioneDes As String = TipoLavorazioneDesFromAgenda(objLavorazione.Piva, objLavorazione.IdAgenda)

                            'Sto modificando il prodotto, quindi devo modificare anche la descrizione
                            Dim matDes As String = MatDesFromMatCod(objLavorazione.Piva, objLavorazione.ElemCod, objLavorazione.MatCod)
                            movDetDes = CreaMovDetDesLavorazioneLavCod(lavCod, matDes, preparazioneDes)

                        End If

                        'HACK: In Extra_Str metto il Tipo_Destinazione e in Extra_Int l'Id_Destinazione
                        'xRisp = movDetHelper.ModificaPuntuale(objLavorazione.Piva,
                        '                                      objLavorazione.SaCod,
                        '                                      objLavorazione.IdAgenda,
                        '                                      objLavorazione.IdMov,
                        '                                      objLavorazione.IdMovDet,
                        '                                      _objParametriServer,
                        '                                      elemCod:=objLavorazione.ElemCod,
                        '                                      matCod:=objLavorazione.MatCod,
                        '                                      movDetDes:=movDetDes,
                        '                                      udmCod:=objLavorazione.UdmCod,
                        '                                      extraInt:=objLavorazione.IdDestinazione,
                        '                                      extraStr:=If(Not IsNothing(objLavorazione.TipoDestinazione), CStr(objLavorazione.TipoDestinazione), Nothing),
                        '                                      qta:=objLavorazione.Qta,
                        '                                      lotto:=objLavorazione.Lotto,
                        '                                      qtaExtra:=objLavorazione.QtaExtra,
                        '                                      qtaDettaglio1:=objLavorazione.NumContenitori,
                        '                                      qtaDettaglio2:=objLavorazione.NumImballaggi
                        '                                      )

                        'Richiamo direttamente il DAL di Scrittura della Movimenti, per poter modificare Cod_Macchina_Lav
                        Dim objMovW As New AgronicaCoreContabDAL.Movimenti_W
                        Dim filtroAggBase = " CAU_MOV = '{0}' "
                        Dim filtroAggFormat = String.Format(filtroAggBase, CAU_LINEA_PRODUZIONE)
                        xRisp = objMovW.ModificaPuntuale(objLavorazione.Piva,
                                                         Sa_Cod:=0,
                                                         objLavorazione.IdAgenda,
                                                         Id_Mov:=0,
                                                         _objParametriServer,
                                                         xFiltroAggiuntivo:=filtroAggFormat,
                                                         Cod_Macchina_Lav:=objLavorazione.CodMacchinaLav)

                        'Richiamo direttamente il DAL di Scrittura della Movimenti_Dettagli, per poter modificare il Sa_Cod se cambia la cella
                        Dim objMovDettW As New AgronicaCoreContabDAL.Movimenti_Dettagli_W

                        xRisp = objMovDettW.ModificaPuntuale(objLavorazione.Piva,
                                                              objLavorazione.SaCod,
                                                              objLavorazione.IdAgenda,
                                                              objLavorazione.IdMov,
                                                              objLavorazione.IdMovDet,
                                                              _objParametriServer,
                                                              Elem_Cod:=objLavorazione.ElemCod,
                                                              Mat_Cod:=objLavorazione.MatCod,
                                                              Mov_Det_Des:=movDetDes,
                                                              Udm_Cod:=objLavorazione.UdmCod,
                                                              Udm_Cod_Extra:=objLavorazione.UdmCodExtra,
                                                              Extra_Int:=objLavorazione.IdDestinazione,
                                                              Extra_Str:=If(Not IsNothing(objLavorazione.TipoDestinazione), CStr(objLavorazione.TipoDestinazione), Nothing),
                                                              Qta:=objLavorazione.Qta,
                                                              Lotto:=objLavorazione.Lotto,
                                                              Qta_Extra:=objLavorazione.QtaExtra,
                                                              Qta_Dettaglio1:=objLavorazione.NumContenitori,
                                                              Qta_Dettaglio2:=objLavorazione.NumImballaggi,
                                                              New_Sa_Cod:=objLavorazione.New_saCod
                                                              )

                    End If

                    'Tab Materie_Prime_Campionature
                    If xRisp = True Then

                        'l'oggetto che mi arriva avrà valorizzato sicuramente il progressivo e il tipo,
                        'poi ci sarà in tipo_cod il nuovo tipo_cod, perché anche se le chiavi sono 4,
                        'in questo contesto, per identificare la riga, mi basteranno progressivo e tipo,
                        'quindi posso inserire negli altri campi chiave i nuovi valori che assumeranno.
                        'Il tipo non cambierà mai e anche il progressivo in questo contesto rimarrà invariato.
                        'Il val_cod che contiene la giacenza qui non mi interessa, perché è il record fittizio
                        'Anche ChkTara e Tara_Campionatura non servono, perché questo record serve solo a impostare un default da scegliere più avanti

                        xRisp = UtilityHelper.AggiornaInserisciMatPrimeCamp(objLavorazione.CalCod,
                                                                            objLavorazione.MateriePrimeCampionature,
                                                                            _objParametriServer)

                    End If

                    'Se la data è diversa devo aggiornare l'agenda e tutti i figli
                    If Not IsNothing(objLavorazione.DataMovimento) AndAlso objLavorazione.DataMovimento <> dataMovimento Then

                        'TODO: aggiorna data movimento, +(flag per aggiornare anche data registrazione), validita_inizio su ogni record figlio
                        xRisp = AggiornaDataMovimento(objLavorazione.Piva, objLavorazione.IdAgenda,
                                                      objLavorazione.DataMovimento)

                    End If

                    'Aggiornamento lavorazione/ordine collegato
                    AggiornaLavorazioneOrdineColleg(objLavorazione, lavCod)
                    SeAggiornaOrdineLavorazione(objLavorazione, lavCod)

                End If

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.AggiornaOrdineLavorazione() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

    Private Sub AggiornaLavorazioneOrdineColleg(objLavorazione As Lavorazione, lavCod As Integer)

        Dim idAgendaRif As Integer

        If lavCod = LAVCOD_TRASFORMAZIONI Then
            idAgendaRif = objLavorazione.IdLavCollegata
        Else
            idAgendaRif = objLavorazione.IdOrdCollegato
        End If

        Dim lavCodRif As Integer = lavCod

        AggiornaAgendaCollegata(idAgendaRif, lavCodRif, objLavorazione, lavCod)

    End Sub

    Private Sub AggiornaAgendaCollegata(idAgendaRif As Integer,
                                        lavCodRif As Integer,
                                        objLavorazione As Lavorazione,
                                        lavCod As Integer)

        Dim objMovDetRif_R = New Mov_Dettagli_Riferimenti_R
        Dim dtMovDetRif = objMovDetRif_R.LeggiPerAgenda(objLavorazione.Piva, 0, objLavorazione.IdAgenda,
                                                        lavCod, -1,
                                                        "", "",
                                                        _objParametriServer,
                                                        leggiRiferimentiInversi:=False,
                                                        Lav_Cod_Rif:=lavCodRif,
                                                        Cau_Mov_Rif:=-1)

        Dim objMovDetRif_W = New Mov_Det_Riferimenti_W

        Dim userCreazione = ""
        Dim dataCreazione = AgroDataInizializzata
        Dim idAgendaRifEsistente = 0

        If dtMovDetRif.Rows.Count > 0 Then
            userCreazione = dtMovDetRif.Rows(0).Item("username_creazione")
            dataCreazione = dtMovDetRif.Rows(0).Item("data_creazione")
            idAgendaRifEsistente = dtMovDetRif.Rows(0).Item("Id_Agenda_Rif")
        End If

        If idAgendaRif <> idAgendaRifEsistente Then

            If idAgendaRifEsistente <> 0 Then
                objMovDetRif_W.Cancella(objLavorazione.Piva, 0, objLavorazione.IdAgenda,
                                        -1, -1, "",
                                        _objParametriServer,
                                        Id_Agenda_Rif:=idAgendaRifEsistente,
                                        Id_Mov_Rif:=-1,
                                        Id_Mov_Det_Rif:=-1)
            End If

            If idAgendaRif <> 0 Then
                objMovDetRif_W.ScriviPerAgenda(objLavorazione.Piva,
                                               objLavorazione.IdAgenda,
                                               lavCod,
                                               objLavorazione.Piva,
                                               idAgendaRif,
                                               lavCodRif,
                                               objLavorazione.DataMovimento,
                                               _objParametriServer,
                                               username_creazione:=userCreazione,
                                               Data_creazione:=dataCreazione)
            End If

        End If

    End Sub

    Private Sub SeAggiornaOrdineLavorazione(objLavorazione As Lavorazione, lavCod As Integer)

        If lavCod = LAVCOD_TRASFORMAZIONI Then

            Dim idAgendaRif As Integer = objLavorazione.IdOrdCollegato

            Dim lavCodRif As Integer = LAVCOD_TESTATE_ORDINE_LAVORAZIONE

            AggiornaAgendaCollegata(idAgendaRif, lavCodRif, objLavorazione, lavCod)

        End If

    End Sub

    Public Function AggiornaScarico(ByVal objLavorazione As Lavorazione,
                                    ByVal messaggioDettagliato As Boolean,
                                    ByRef msgError As String
                                    ) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean
        Dim flagAggiornaGiacenze As Boolean = False
        Dim CheckGiacenze As New UtilityHelperCheckGiacenze


        Dim movDetHelper As New Agenda_Movimenti_Dettagli_Helper
        Dim movDestHelper As New Agenda_Movimenti_Destinazioni_Helper

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyCaricoScarico(objLavorazione,
                                                             enum_TipoOperazioneDB.Modifica,
                                                             flagUtilizzaDestinazione:=True)

            If flagOggettoCorretto Then

                'Prima di procedere con al modifica devo verificare se nel frattempo il record della movimenti dettagli è cambiato
                Dim dataUltimaModifica As DateTime
                Dim usernameUltimaModifica As String = ""

                dataUltimaModifica = UtilityHelper.UltimaModificaMovDettaglio(objLavorazione.Piva,
                                                                              objLavorazione.SaCod,
                                                                              objLavorazione.IdAgenda,
                                                                              objLavorazione.IdMov,
                                                                              objLavorazione.IdMovDet,
                                                                              usernameUltimaModifica,
                                                                              _objParametriServer)


                If Not IsNothing(objLavorazione.DataLettura) AndAlso dataUltimaModifica > objLavorazione.DataLettura Then
                    msgError &= UtilityHelper.GetMessaggioDataModifica(dataUltimaModifica, usernameUltimaModifica, _objParametriUtenti)
                    xRisp = False
                Else

                    'devo segnarmi il cal_cod e la quantità dei vari confezionamenti perché devo aggiornare le giacenze
                    Dim numConfezioniResidue As Decimal = 0
                    Dim numContenitoriResidui As Decimal = 0
                    Dim numImballaggiResidui As Decimal = 0
                    Dim kgNettiResidui As Decimal = 0
                    Dim kgLordiResidui As Decimal = 0
                    Dim calCod As Integer = 0
                    Dim objMovDet As Movimento_Dettaglio = Nothing
                    Dim flagQtaExtraTotZero As Boolean = False

                    ValoriConfezionamenti(objLavorazione, calCod,
                                          numConfezioniResidue, numContenitoriResidui, numImballaggiResidui,
                                          kgNettiResidui, kgLordiResidui,
                                          flagValorizzaSempreResidui:=False,
                                          objMovDet:=objMovDet, flagQtaExtraTotZero:=flagQtaExtraTotZero)

                    If IsNothing(objMovDet) Then
                        Throw New Exception("Non c'è nessuno scarico da modificare con la chiave indicata.")
                    End If

                    If objMovDet.Jolly_Int = MagazzinoMovimentato Then
                        flagAggiornaGiacenze = True
                        CheckGiacenze = UtilityHelper.SeCheckGiacenze(objMovDet.Elem_Cod, _objParametriUtenti)
                    End If

                    Dim risGiacenzaCheck As String = ""

                    If flagAggiornaGiacenze = True AndAlso CheckGiacenze.eseguiCheckGiacenze Then

                        'se sono negativi, vuol dire che ho ridotto il quantitativo di prodotto scaricato, perciò se prima ce l'avevo, ce l'ho anche ora
                        ' e non è necessario verificare la giacenza
                        If numConfezioniResidue > 0 OrElse numContenitoriResidui > 0 OrElse numImballaggiResidui > 0 OrElse
                           (flagQtaExtraTotZero = False AndAlso (kgNettiResidui > 0 OrElse kgLordiResidui > 0)) Then

                            risGiacenzaCheck = ControllaGiacenza(objLavorazione, objMovDet,
                                                                 Now, calCod,
                                                                 Math.Abs(numConfezioniResidue),
                                                                 Math.Abs(numContenitoriResidui),
                                                                 Math.Abs(numImballaggiResidui),
                                                                 Math.Abs(kgNettiResidui),
                                                                 Math.Abs(kgLordiResidui),
                                                                 messaggioDettagliato,
                                                                 CheckGiacenze.flag_QtaNoZero,
                                                                 CheckGiacenze.flag_QtaMaggioreZero)
                        End If
                    End If

                    If risGiacenzaCheck <> "" Then
                        msgError &= "Impossibile modificare: Il prodotto andrebbe sotto giacenza; " & risGiacenzaCheck
                        xRisp = False
                    Else

                        If Not IsNothing(objLavorazione.ListaChiaviDaModificare) AndAlso objLavorazione.ListaChiaviDaModificare.Count > 0 Then
                            'TODO: mi sono arrivate delle chiavi da modificare ==> quando tocco quella tabella devo eliminare e ricreare (conservando le chiavi giuste), anziché update
                            Throw New NotImplementedException("Sto cercando di modificare delle chiavi")
                        Else
                            Dim movDetDes As String = Nothing

                            If Not IsNothing(objLavorazione.MatCod) AndAlso objLavorazione.MatCod <> 0 AndAlso
                               Not IsNothing(objLavorazione.ElemCod) AndAlso objLavorazione.ElemCod <> 0 Then

                                Dim preparazioneDes As String = TipoLavorazioneDesFromAgenda(objLavorazione.Piva, objLavorazione.IdAgenda)
                                'Sto modificando il prodotto, quindi devo modificare anche la descrizione
                                Dim matDes As String = MatDesFromMatCod(objLavorazione.Piva, objLavorazione.ElemCod, objLavorazione.MatCod)
                                movDetDes = CreaMovDetDesLavorazione(CAU_SCARICO, matDes, preparazioneDes)
                            End If

                            xRisp = movDetHelper.ModificaPuntuale(objLavorazione.Piva,
                                                                  objLavorazione.SaCod,
                                                                  objLavorazione.IdAgenda,
                                                                  objLavorazione.IdMov,
                                                                  objLavorazione.IdMovDet,
                                                                  _objParametriServer,
                                                                  elemCod:=objLavorazione.ElemCod,
                                                                  matCod:=objLavorazione.MatCod,
                                                                  movDetDes:=movDetDes,
                                                                  udmCod:=objLavorazione.UdmCod,
                                                                  udmCodExtra:=objLavorazione.UdmCodExtra,
                                                                  qta:=objLavorazione.Qta,
                                                                  calCod:=objLavorazione.CalCod,
                                                                  lotto:=objLavorazione.Lotto,
                                                                  jollyInt:=objLavorazione.JollyInt,
                                                                  qtaExtra:=Agro_Math.ArrotondaVal_Nothing(objLavorazione.QtaExtra, objLavorazione.CifreArrotondamento),
                                                                  qtaExtraTotale:=objLavorazione.QtaExtraTotale,
                                                                  tara:=objLavorazione.Tara,
                                                                  qtaDettaglio1:=objLavorazione.NumContenitori,
                                                                  qtaDettaglio2:=objLavorazione.NumImballaggi
                                                                  )

                            If xRisp = True Then

                                xRisp = movDestHelper.ModificaPuntuale(objLavorazione.Piva,
                                                                       objLavorazione.SaCod,
                                                                       objLavorazione.IdAgenda,
                                                                       objLavorazione.IdMov,
                                                                       objLavorazione.IdMovDet,
                                                                       appezza:=0,
                                                                       idDestinazione:=objLavorazione.IdDestinazione,
                                                                       objParametri:=_objParametriServer,
                                                                       tipoDestinazione:=objLavorazione.TipoDestinazione,
                                                                       qta:=objLavorazione.Qta,
                                                                       qtaDest1:=objLavorazione.NumContenitori,
                                                                       qtaDest2:=objLavorazione.NumImballaggi)

                                'TODO: modifica di materie prime campionature, che fare? mi arriva la lista di quelli da modificare?
                                'in caso di scarico in realtà non dovrei modificare nulla

                            End If

                            If xRisp = True AndAlso flagAggiornaGiacenze = True AndAlso calCod <> 0 Then
                                'devo aggiornare i valori alla giacenza

                                AggiornaGiacenzeCampionature(CAU_SCARICO, calCod,
                                                             numConfezioniResidue,
                                                             numContenitoriResidui,
                                                             numImballaggiResidui)
                            End If

                        End If

                    End If

                End If

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.AggiornaScarico() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Function AggiornaCarico(ByVal objLavorazione As Lavorazione,
                                   ByVal messaggioDettagliato As Boolean,
                                   ByRef msgError As String
                                   ) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean
        Dim flagAggiornaGiacenze As Boolean = False
        Dim CheckGiacenze As New UtilityHelperCheckGiacenze

        Dim movDetHelper As New Agenda_Movimenti_Dettagli_Helper
        Dim movDestHelper As New Agenda_Movimenti_Destinazioni_Helper

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyCaricoScarico(objLavorazione,
                                                             enum_TipoOperazioneDB.Modifica,
                                                             flagUtilizzaDestinazione:=True)

            If flagOggettoCorretto Then

                'Prima di procedere con al modifica devo verificare se nel frattempo il record della movimenti dettagli è cambiato
                Dim dataUltimaModifica As DateTime
                Dim usernameUltimaModifica As String = ""

                dataUltimaModifica = UtilityHelper.UltimaModificaMovDettaglio(objLavorazione.Piva,
                                                                              objLavorazione.SaCod,
                                                                              objLavorazione.IdAgenda,
                                                                              objLavorazione.IdMov,
                                                                              objLavorazione.IdMovDet,
                                                                              usernameUltimaModifica,
                                                                              _objParametriServer)


                If Not IsNothing(objLavorazione.DataLettura) AndAlso dataUltimaModifica > objLavorazione.DataLettura Then
                    msgError &= UtilityHelper.GetMessaggioDataModifica(dataUltimaModifica, usernameUltimaModifica, _objParametriUtenti)
                    xRisp = False
                Else

                    'devo segnarmi il cal_cod e la quantità dei vari confezionamenti perché devo aggiornare le giacenze

                    Dim numConfezioniResidue As Decimal = 0
                    Dim numContenitoriResidui As Decimal = 0
                    Dim numImballaggiResidui As Decimal = 0
                    Dim kgNettiResidui As Decimal = 0
                    Dim kgLordiResidui As Decimal = 0
                    Dim calCod As Integer = 0
                    Dim objMovDet As Movimento_Dettaglio = Nothing
                    Dim flagQtaExtraTotZero As Boolean = False

                    ValoriConfezionamenti(objLavorazione, calCod,
                                          numConfezioniResidue, numContenitoriResidui, numImballaggiResidui,
                                          kgNettiResidui, kgLordiResidui,
                                          flagValorizzaSempreResidui:=False,
                                          objMovDet:=objMovDet, flagQtaExtraTotZero:=flagQtaExtraTotZero)

                    If IsNothing(objMovDet) Then
                        Throw New Exception("Non c'è nessun carico da modificare con la chiave indicata.")
                    End If

                    If objMovDet.Jolly_Int = MagazzinoMovimentato Then
                        flagAggiornaGiacenze = True
                        CheckGiacenze = UtilityHelper.SeCheckGiacenze(objMovDet.Elem_Cod, _objParametriUtenti)
                    End If

                    Dim risGiacenzaCheck As String = ""

                    If flagAggiornaGiacenze = True AndAlso CheckGiacenze.eseguiCheckGiacenze Then

                        If numConfezioniResidue < 0 OrElse numContenitoriResidui < 0 OrElse numImballaggiResidui < 0 OrElse
                           (flagQtaExtraTotZero = False AndAlso (kgNettiResidui < 0 OrElse kgLordiResidui < 0)) Then

                            Dim objMovDettR As New Movimenti_Dettagli_R
                            If objMovDettR.ProdottoPresenteDettagli(objMovDet.Piva, objMovDet.Elem_Cod, 0, objMovDet.Mat_Cod,
                                                                    objMovDet.Lotto, calCod, CAU_SCARICO, _objParametriServer) Then

                                'se i valori residui sono positivi vuol dire che ho aumentato i quantitativi, quindi solo per il controllo
                                'giacenza li passo a zero perché sennò genererebbero giacenza insufficiente
                                risGiacenzaCheck = ControllaGiacenza(objLavorazione, objMovDet,
                                                                     Now, calCod,
                                                                     IIf(numConfezioniResidue > 0, 0, Math.Abs(numConfezioniResidue)),
                                                                     IIf(numContenitoriResidui > 0, 0, Math.Abs(numContenitoriResidui)),
                                                                     IIf(numImballaggiResidui > 0, 0, Math.Abs(numImballaggiResidui)),
                                                                     IIf(kgNettiResidui > 0, 0, Math.Abs(kgNettiResidui)),
                                                                     IIf(kgLordiResidui > 0, 0, Math.Abs(kgLordiResidui)),
                                                                     messaggioDettagliato,
                                                                     CheckGiacenze.flag_QtaNoZero,
                                                                     CheckGiacenze.flag_QtaMaggioreZero)
                            End If

                        End If

                    End If

                    If risGiacenzaCheck <> "" Then
                        msgError &= "Impossibile modificare: Il prodotto è già utilizzato; " & risGiacenzaCheck
                        xRisp = False
                    Else

                        If Not IsNothing(objLavorazione.ListaChiaviDaModificare) AndAlso objLavorazione.ListaChiaviDaModificare.Count > 0 Then

                            'TODO: mi sono arrivate delle chiavi da modificare ==> quando tocco quella tabella devo eliminare e ricreare (conservando le chiavi giuste), anziché update
                            Throw New NotImplementedException("Sto cercando di modificare delle chiavi")
                        Else
                            Dim movDetDes As String = Nothing

                            If Not IsNothing(objLavorazione.MatCod) AndAlso objLavorazione.MatCod <> 0 AndAlso
                               Not IsNothing(objLavorazione.ElemCod) AndAlso objLavorazione.ElemCod <> 0 Then

                                Dim preparazioneDes As String = TipoLavorazioneDesFromAgenda(objLavorazione.Piva, objLavorazione.IdAgenda)
                                'Sto modificando il prodotto, quindi devo modificare anche la descrizione
                                Dim matDes As String = MatDesFromMatCod(objLavorazione.Piva, objLavorazione.ElemCod, objLavorazione.MatCod)
                                movDetDes = CreaMovDetDesLavorazione(CAU_CARICO, matDes, preparazioneDes)
                            End If

                            'xRisp = movDetHelper.ModificaPuntuale(objLavorazione.Piva,
                            '                                      objLavorazione.SaCod,
                            '                                      objLavorazione.IdAgenda,
                            '                                      objLavorazione.IdMov,
                            '                                      objLavorazione.IdMovDet,
                            '                                      _objParametriServer,
                            '                                      elemCod:=objLavorazione.ElemCod,
                            '                                      matCod:=objLavorazione.MatCod,
                            '                                      movDetDes:=movDetDes,
                            '                                      udmCod:=objLavorazione.UdmCod,
                            '                                      qta:=objLavorazione.Qta,
                            '                                      calCod:=objLavorazione.CalCod,
                            '                                      lotto:=objLavorazione.Lotto,
                            '                                      jollyInt:=objLavorazione.JollyInt,
                            '                                      qtaExtra:=Agro_Math.ArrotondaVal_Nothing(objLavorazione.QtaExtra, objLavorazione.CifreArrotondamento),
                            '                                      qtaExtraTotale:=objLavorazione.QtaExtraTotale,
                            '                                      tara:=objLavorazione.Tara,
                            '                                      qtaDettaglio1:=objLavorazione.NumContenitori,
                            '                                      qtaDettaglio2:=objLavorazione.NumImballaggi
                            '                                      )

                            'Richiamo direttamente il DAL di scrittura della Movimenti_Dettagli, per poter modificare il Sa_Cod se cambia la cella
                            Dim objMovDettW As New AgronicaCoreContabDAL.Movimenti_Dettagli_W

                            xRisp = objMovDettW.ModificaPuntuale(objLavorazione.Piva,
                                                                  objLavorazione.SaCod,
                                                                  objLavorazione.IdAgenda,
                                                                  objLavorazione.IdMov,
                                                                  objLavorazione.IdMovDet,
                                                                  _objParametriServer,
                                                                  Elem_Cod:=objLavorazione.ElemCod,
                                                                  Mat_Cod:=objLavorazione.MatCod,
                                                                  Mov_Det_Des:=movDetDes,
                                                                  Udm_Cod:=objLavorazione.UdmCod,
                                                                  Udm_Cod_Extra:=objLavorazione.UdmCodExtra,
                                                                  Qta:=objLavorazione.Qta,
                                                                  Cal_Cod:=objLavorazione.CalCod,
                                                                  Lotto:=objLavorazione.Lotto,
                                                                  Jolly_Int:=objLavorazione.JollyInt,
                                                                  Qta_Extra:=Agro_Math.ArrotondaVal_Nothing(objLavorazione.QtaExtra, objLavorazione.CifreArrotondamento),
                                                                  Qta_Extra_Totale:=objLavorazione.QtaExtraTotale,
                                                                  Tara:=objLavorazione.Tara,
                                                                  Qta_Dettaglio1:=objLavorazione.NumContenitori,
                                                                  Qta_Dettaglio2:=objLavorazione.NumImballaggi,
                                                                  New_Sa_Cod:=objLavorazione.New_saCod
                                                                  )

                            If xRisp = True Then

                                'xRisp = movDestHelper.ModificaPuntuale(objLavorazione.Piva,
                                '                                       objLavorazione.SaCod,
                                '                                       objLavorazione.IdAgenda,
                                '                                       objLavorazione.IdMov,
                                '                                       objLavorazione.IdMovDet,
                                '                                       appezza:=0,
                                '                                       idDestinazione:=objLavorazione.IdDestinazione,
                                '                                       objParametri:=_objParametriServer,
                                '                                       tipoDestinazione:=objLavorazione.TipoDestinazione,
                                '                                       qta:=objLavorazione.Qta,
                                '                                       qtaDest1:=objLavorazione.NumContenitori,
                                '                                       qtaDest2:=objLavorazione.NumImballaggi)

                                'Richiamo direttamente il DAL di scrittura della Mov_Destinazioni, per poter modificare il Sa_Cod se cambia la cella
                                Dim objMovDestW As New AgronicaCoreContabDAL.Mov_Destinazioni_W

                                xRisp = objMovDestW.ModificaPuntuale(objLavorazione.Piva,
                                                                       objLavorazione.SaCod,
                                                                       objLavorazione.IdAgenda,
                                                                       objLavorazione.IdMov,
                                                                       objLavorazione.IdMovDet,
                                                                       Appezza:=0,
                                                                       Id_Destinazione:=objLavorazione.IdDestinazione,
                                                                       objParametri:=_objParametriServer,
                                                                       Tipo_Destinazione:=objLavorazione.New_tipoDestinazione,
                                                                       Qta:=objLavorazione.Qta,
                                                                       Qta_Dest1:=objLavorazione.NumContenitori,
                                                                       Qta_Dest2:=objLavorazione.NumImballaggi,
                                                                       New_Sa_Cod:=objLavorazione.New_saCod,
                                                                       New_Id_Destinazione:=objLavorazione.New_idDestinazione)

                                If xRisp = True Then
                                    'l'oggetto che mi arriva avrà valorizzato sicuramente il progressivo e il tipo,
                                    'poi ci sarà in tipo_cod il nuovo tipo_cod, e (visto che potrei cambiare il tipo di imballo, per es,
                                    'anche chkTara e Tara ), perché anche se le chiavi sono 4, in questo contesto , per identificare la riga,
                                    'mi basteranno progressivo e tipo, quindi posso inserire negli altri campi chiave i nuovi valori che assumeranno.
                                    'Il tipo non cambierà mai e anche il progressivo in questo contesto rimarrà invariato.
                                    'Il val_cod che contiene la giacenza qui non mi interessa, perché mi ero già segnata prima il valore e poi lo aggiorno,
                                    'quindi è inutile impostarlo, qui (anzi è anche rischioso perché poi dovrei sapere che non lo devo andare a riaggiornare)

                                    xRisp = UtilityHelper.AggiornaInserisciMatPrimeCamp(objLavorazione.CalCod,
                                                                                        objLavorazione.MateriePrimeCampionature,
                                                                                        _objParametriServer)

                                End If

                            End If

                            If xRisp = True AndAlso calCod <> 0 Then
                                'devo aggiornare i valori alla giacenza

                                AggiornaGiacenzeCampionature(CAU_CARICO, calCod,
                                                             numConfezioniResidue,
                                                             numContenitoriResidui,
                                                             numImballaggiResidui)
                            End If

                        End If

                    End If

                End If

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.AggiornaCarico() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Function AggiornaDatiCalcolatiCarichi(
        ByVal objLavorazione As Lavorazione,
        ByVal messaggioDettagliato As Boolean,
        ByRef msgError As String
        ) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            'Leggi lavorazione

            Dim lavorazione = LeggiLavorazione(objLavorazione.Piva, 0, objLavorazione.IdAgenda)

            Dim testataCarichiLavorazione = lavorazione.Movimenti.Where(
                Function(mov) mov.Cau_Mov = CAU_CARICO)

            Dim listaCarichiLavorazione = lavorazione.Movimenti_Dettagli.Where(
                Function(movDett) movDett.Id_Mov = testataCarichiLavorazione(0).Id_Mov And
                movDett.Jolly_Int = MagazzinoMovimentato)

            'Aggiornamento carichi

            For Each caricoLavorazione In listaCarichiLavorazione

                Dim lavorazioneAggiorna As New Lavorazione

                lavorazioneAggiorna.Piva = objLavorazione.Piva
                lavorazioneAggiorna.SaCod = objLavorazione.SaCod
                lavorazioneAggiorna.IdAgenda = objLavorazione.IdAgenda

                lavorazioneAggiorna.DataLettura = lavorazione.DataLettura

                lavorazioneAggiorna.IdMov = testataCarichiLavorazione(0).Id_Mov
                lavorazioneAggiorna.IdMovDet = caricoLavorazione.Id_Mov_Det
                lavorazioneAggiorna.IdDestinazione = caricoLavorazione.Movimenti_Destinazioni(0).Id_Destinazione

                If Not IsNothing(objLavorazione.Qta) Then
                    lavorazioneAggiorna.Qta = objLavorazione.Qta
                End If

                If Not IsNothing(objLavorazione.QtaExtraTotale) Then
                    lavorazioneAggiorna.QtaExtraTotale = objLavorazione.QtaExtraTotale
                End If

                lavorazioneAggiorna.CalCod = caricoLavorazione.Cal_Cod

                lavorazioneAggiorna.MateriePrimeCampionature = New List(Of Materia_Prima_Campionatura)

                For Each mpcRicalcolate In objLavorazione.MateriePrimeCampionature

                    Dim mcpAggiorna = mpcRicalcolate

                    lavorazioneAggiorna.MateriePrimeCampionature.Add(mcpAggiorna)

                Next

                xRisp = AggiornaCarico(lavorazioneAggiorna, messaggioDettagliato, msgError)

                If xRisp = False Then
                    Dim messaggio = String.Format("Errore in aggiorna carico (IdMovDet:{0})",
                                                  lavorazioneAggiorna.IdMovDet)
                    Throw New Exception(messaggio)
                End If

            Next

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.AggiornaDatiCalcolatiCarichi() ] : " & ex.Message)

        Finally

            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)

        End Try

        Return xRisp

    End Function

#End Region

#Region "Funzioni Pubbliche Cancellazione Lavorazioni"

    Public Function CancellaScarico(ByVal objLavorazione As Lavorazione,
                                    ByVal messaggioDettagliato As Boolean,
                                    ByRef msgError As String
                                    ) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean
        Dim flagAggiornaGiacenze As Boolean = False

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            'TODO: ci sono dei casi in cui devo impedire l'eliminazione? li devo verificare qui oppure da fuori a seconda delle casistiche?

            flagOggettoCorretto = CheckPropertyCaricoScarico(objLavorazione,
                                                             enum_TipoOperazioneDB.Cancellazione,
                                                             flagUtilizzaDestinazione:=True)

            If flagOggettoCorretto Then

                'devo segnarmi il cal_cod e la quantità dei vari confezionamenti perché se alla fine mi è rimasto il cal_cod devo aggiornare le giacenze
                Dim numConfezioniResidue As Decimal = 0
                Dim numContenitoriResidui As Decimal = 0
                Dim numImballaggiResidui As Decimal = 0
                Dim kgNettiResidui As Decimal = 0
                Dim kgLordiResidui As Decimal = 0
                Dim calCod As Integer = 0
                Dim objMovDet As Movimento_Dettaglio = Nothing
                Dim flagQtaExtraTotZero As Boolean = False

                ValoriConfezionamenti(objLavorazione, calCod,
                                      numConfezioniResidue, numContenitoriResidui, numImballaggiResidui,
                                      kgNettiResidui, kgLordiResidui,
                                      flagValorizzaSempreResidui:=True,
                                      objMovDet:=objMovDet, flagQtaExtraTotZero:=flagQtaExtraTotZero)

                If IsNothing(objMovDet) Then
                    Throw New Exception("Non c'è nessuno scarico da cancellare con la chiave indicata.")
                End If

                If objMovDet.Jolly_Int = MagazzinoMovimentato Then
                    flagAggiornaGiacenze = True
                End If

                Dim objMovDetHelper As New Agenda_Movimenti_Dettagli_Helper
                xRisp = objMovDetHelper.Cancella(objLavorazione.Piva,
                                                 objLavorazione.SaCod,
                                                 objLavorazione.IdAgenda,
                                                 objLavorazione.IdMov,
                                                 objLavorazione.IdMovDet,
                                                 _objParametriServer)

                If xRisp = True AndAlso flagAggiornaGiacenze = True AndAlso calCod <> 0 Then
                    'devo aggiornare i valori alla giacenza (nel caso il cal_cod esista ancora)
                    'ovviamente a segno inverso perché anche se sto facendo lo scarico in realtà li devo sommare, non sottrarre
                    'il segno inverso mi arriva già dal calcolo sopra, quindi qui i valori saranno sempre negativi
                    AggiornaGiacenzeCampionature(CAU_SCARICO, calCod,
                                                 numConfezioniResidue,
                                                 numContenitoriResidui,
                                                 numImballaggiResidui)
                End If

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.CancellaScarico() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Function CancellaCarico(ByVal objLavorazione As Lavorazione,
                                   ByVal messaggioDettagliato As Boolean,
                                   ByRef msgError As String
                                   ) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean
        Dim flagAggiornaGiacenze As Boolean = False
        Dim CheckGiacenze As New UtilityHelperCheckGiacenze

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyCaricoScarico(objLavorazione,
                                                             enum_TipoOperazioneDB.Cancellazione,
                                                             flagUtilizzaDestinazione:=True)

            If flagOggettoCorretto Then

                'devo segnarmi il cal_cod e la quantità dei vari confezionamenti perché se alla fine mi è rimasto il cal_cod devo aggiornare le giacenze
                Dim numConfezioniResidue As Decimal = 0
                Dim numContenitoriResidui As Decimal = 0
                Dim numImballaggiResidui As Decimal = 0
                Dim kgNettiResidui As Decimal = 0
                Dim kgLordiResidui As Decimal = 0
                Dim calCod As Integer = 0
                Dim objMovDet As Movimento_Dettaglio = Nothing
                Dim flagQtaExtraTotZero As Boolean = False

                ValoriConfezionamenti(objLavorazione, calCod,
                                      numConfezioniResidue, numContenitoriResidui, numImballaggiResidui,
                                      kgNettiResidui, kgLordiResidui,
                                      flagValorizzaSempreResidui:=True,
                                      objMovDet:=objMovDet, flagQtaExtraTotZero:=flagQtaExtraTotZero)

                'TODO: ci sono dei casi in cui devo impedire l'eliminazione? li devo verificare qui oppure da fuori a seconda delle casistiche?

                If IsNothing(objMovDet) Then
                    Throw New Exception("Non c'è nessun carico da cancellare con la chiave indicata.")
                End If

                If objMovDet.Jolly_Int = MagazzinoMovimentato Then
                    flagAggiornaGiacenze = True
                    CheckGiacenze = UtilityHelper.SeCheckGiacenze(objMovDet.Elem_Cod, _objParametriUtenti)
                End If

                Dim risGiacenzaCheck As String = ""

                If flagAggiornaGiacenze = True AndAlso CheckGiacenze.eseguiCheckGiacenze Then

                    Dim objMovDettR As New Movimenti_Dettagli_R
                    If objMovDettR.ProdottoPresenteDettagli(objMovDet.Piva, objMovDet.Elem_Cod, 0, objMovDet.Mat_Cod,
                                                            objMovDet.Lotto, calCod, CAU_SCARICO, _objParametriServer) Then

                        risGiacenzaCheck = ControllaGiacenza(objLavorazione, objMovDet,
                                                             Now, calCod,
                                                             Math.Abs(numConfezioniResidue),
                                                             Math.Abs(numContenitoriResidui),
                                                             Math.Abs(numImballaggiResidui),
                                                             Math.Abs(kgNettiResidui),
                                                             Math.Abs(kgLordiResidui),
                                                             messaggioDettagliato,
                                                             CheckGiacenze.flag_QtaNoZero,
                                                             CheckGiacenze.flag_QtaMaggioreZero)
                    End If

                End If

                If risGiacenzaCheck <> "" Then
                    msgError &= "Impossibile cancellare: Il prodotto risultante viene già utilizzato. " & risGiacenzaCheck
                Else
                    xRisp = True
                End If

                If xRisp = True Then
                    Dim objMovDetHelper As New Agenda_Movimenti_Dettagli_Helper
                    xRisp = objMovDetHelper.Cancella(objLavorazione.Piva,
                                                     objLavorazione.SaCod,
                                                     objLavorazione.IdAgenda,
                                                     objLavorazione.IdMov,
                                                     objLavorazione.IdMovDet,
                                                     _objParametriServer)
                End If

                If xRisp = True AndAlso flagAggiornaGiacenze = True AndAlso calCod <> 0 Then
                    'devo aggiornare i valori alla giacenza (nel caso il cal_cod esista ancora)
                    'ovviamente a segno inverso perché anche se sto facendo il carico in realtà li devo sottrarre, non sommare
                    'il segno inverso mi arriva già dal calcolo sopra, quindi qui i valori saranno sempre negativi
                    AggiornaGiacenzeCampionature(CAU_CARICO, calCod,
                                                 numConfezioniResidue,
                                                 numContenitoriResidui,
                                                 numImballaggiResidui)
                End If

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.CancellaCarico() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Function CancellaTotaleLavorazione(ByVal objLavorazione As Lavorazione,
                                              ByVal messaggioDettagliato As Boolean,
                                              ByRef msgError As String
                                              ) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean
        Dim objMovHelper As New Agenda_Movimenti_Helper

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyTestataLavorazione(objLavorazione, enum_TipoOperazioneDB.Cancellazione)

            If flagOggettoCorretto Then

                Dim listMovimenti As List(Of Movimento) = objMovHelper.Leggi(objLavorazione.Piva,
                                                                             0,
                                                                             objLavorazione.IdAgenda,
                                                                             _objParametriServer)

                xRisp = CancellaMovimentiCarico(listMovimenti, messaggioDettagliato, msgError)

                xRisp = CancellaMovimentiScarico(listMovimenti, messaggioDettagliato, msgError)

                xRisp = CancellaAgenda(objLavorazione)

                'TODO: ci sono altri motivi che impediscono la cancellazione?!? (ad es: agenda_riferimenti, documenti collegati,...) e nel caso è meglio verificarli qui o prima?!?

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.CancellaTotaleLavorazione() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

    Private Function CancellaMovimentiCarico(ByVal listMovimenti As List(Of Movimento),
                                             ByVal messaggioDettagliato As Boolean,
                                             ByRef msgError As String
                                             ) As Boolean

        Return CancellaMovimentiPerCausale(CAU_CARICO, listMovimenti, messaggioDettagliato, msgError)

    End Function

    Private Function CancellaMovimentiScarico(ByVal listMovimenti As List(Of Movimento),
                                              ByVal messaggioDettagliato As Boolean,
                                              ByRef msgError As String
                                              ) As Boolean

        Return CancellaMovimentiPerCausale(CAU_SCARICO, listMovimenti, messaggioDettagliato, msgError)

    End Function

    Private Function CancellaMovimentiPerCausale(ByVal cauMov As String,
                                                 ByVal listMovimenti As List(Of Movimento),
                                                 ByVal messaggioDettagliato As Boolean,
                                                 ByRef msgError As String
                                                 ) As Boolean

        Dim xRisp As Boolean = True

        Dim objMovDet = listMovimenti.Find(Function(x) x.Cau_Mov = cauMov)

        If Not IsNothing(objMovDet) AndAlso Not IsNothing(objMovDet.Movimenti_Dettagli) Then

            For Each movDet As Movimento_Dettaglio In objMovDet.Movimenti_Dettagli

                'Lancia eliminazione singolo movimento

                Dim objLav As New Lavorazione With {
                    .Piva = movDet.Piva,
                    .SaCod = movDet.Sa_Cod,
                    .IdAgenda = movDet.Id_Agenda,
                    .IdMov = movDet.Id_Mov,
                    .IdMovDet = movDet.Id_Mov_Det
                }

                Select Case cauMov

                    Case CAU_CARICO
                        xRisp = CancellaCarico(objLav, messaggioDettagliato, msgError)

                    Case CAU_SCARICO
                        xRisp = CancellaScarico(objLav, messaggioDettagliato, msgError)

                    Case Else
                        Dim messaggioEccezione = String.Format("Cancellazione non gestita per causale movimento: {0}",
                                                               cauMov)
                        Throw New Exception(messaggioEccezione)

                End Select

                If xRisp = False Then

                    msgError = String.Format("<b>{0} {1}</b><br/>{2}",
                                             movDet.Mov_Det_Des,
                                             movDet.Lotto,
                                             msgError)

                    'Devo abortire la transazione per riprisintare eventuali movimenti già cancellati
                    Dim messaggioEccezione = String.Format("Errore in fase di cancellazione movimento: {0} {1} (Causale: {2} - IdMovDet: {3})",
                                                           movDet.Mov_Det_Des,
                                                           movDet.Lotto,
                                                           cauMov,
                                                           movDet.Id_Mov_Det)
                    Throw New Exception(messaggioEccezione)

                End If

            Next

        End If

        Return xRisp

    End Function

    Private Function CancellaAgenda(objLavorazione As Lavorazione) As Boolean

        Dim objAgendaHelper As New Agenda_Operazione_Helper

        Return objAgendaHelper.Cancella(objLavorazione.Piva,
                                        0,
                                        objLavorazione.IdAgenda,
                                        cancellaAggancioRicetta:=False,
                                        objParametri:=_objParametriServer)

    End Function

#End Region

#Region "Funzioni Pubbliche Accessorie a Lavorazioni"

    Public Function AggiornaDataMovimento(ByVal piva As String,
                                          ByVal idAgenda As Integer,
                                          ByVal dataMovimento As DateTime
                                          ) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean

        Dim agendaHelper As New Agenda_Operazione_Helper
        Dim movHelper As New Agenda_Movimenti_Helper
        Dim movDetHelper As New Agenda_Movimenti_Dettagli_Helper
        Dim movDestHelper As New Agenda_Movimenti_Destinazioni_Helper

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            '------------------------------------------------
            '----- AGENDA
            '------------------------------------------------
            xRisp = agendaHelper.ModificaPuntuale(piva, 0, idAgenda,
                                                  _objParametriServer,
                                                  validitaInizio:=dataMovimento)

            '------------------------------------------------
            '----- MOVIMENTI
            '------------------------------------------------
            If xRisp = True Then

                xRisp = movHelper.ModificaPuntuale(piva, 0, idAgenda, 0,
                                                   _objParametriServer,
                                                   flagUsaOraReale:=True,
                                                   dataMovimento:=dataMovimento,
                                                   validitaInizio:=dataMovimento,
                                                   ora:=dataMovimento)

                'TODO: in ora serve anche orario?!?, cosa imposto?!?

                If xRisp = True Then
                    'Modifico anche la data registrazione (solo scarico e carico, no intestazione)
                    xRisp = movHelper.ModificaPuntuale(piva, 0, idAgenda, 0,
                                                       _objParametriServer,
                                                       flagUsaOraReale:=True,
                                                       dataRegistrazione:=dataMovimento,
                                                       xFiltroAggiuntivo:=" (Cau_Mov <> '" & CAU_LINEA_PRODUZIONE & "') ")

                End If

            End If

            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI
            '------------------------------------------------
            If xRisp = True Then
                xRisp = movDetHelper.ModificaPuntuale(piva, 0, idAgenda, 0, 0,
                                                      _objParametriServer,
                                                      data:=dataMovimento)
            End If

            '------------------------------------------------
            '----- MOVIMENTI DESTINAZIONI
            '------------------------------------------------
            If xRisp = True Then
                xRisp = movDestHelper.ModificaPuntuale(piva, 0, idAgenda, 0, 0, 0, 0,
                                                       _objParametriServer,
                                                       data:=dataMovimento)
            End If

            '------------------------------------------------
            '----- MATERIE PRIME CAMPIONATURE ???!!!??
            '------------------------------------------------

            'TODO: dovrei pescare i cal_cod dei carichi ed andare ad aggiornare la validita_inizio di tutti quei carichi?!?


            'TODO: implementare modifica anche su mov_det_tecnico e mov_det_tecnico_extra e magari spostare a livello di agenda, non di lavorazione


            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.AggiornaDataMovimento() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Function ChiudiLavorazione(ByVal piva As String, ByVal idAgenda As Integer) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            Dim objMov As Movimento = UtilityHelper.CercaMovimentoSpecificoSenzaDettagli(CAU_LINEA_PRODUZIONE,
                                                                                         piva,
                                                                                         idAgenda,
                                                                                         _objParametriServer)

            If Not IsNothing(objMov) Then

                Dim movHelper As New Agenda_Movimenti_Helper

                xRisp = movHelper.ModificaPuntuale(piva, 0, idAgenda, objMov.Id_Mov, _objParametriServer, extraInt:=1)

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.ChiudiLavorazione() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Function ApriLavorazione(ByVal piva As String, ByVal idAgenda As Integer) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            Dim objMov As Movimento = UtilityHelper.CercaMovimentoSpecificoSenzaDettagli(CAU_LINEA_PRODUZIONE,
                                                                                         piva,
                                                                                         idAgenda,
                                                                                         _objParametriServer)

            If Not IsNothing(objMov) Then

                Dim movHelper As New Agenda_Movimenti_Helper

                xRisp = movHelper.ModificaPuntuale(piva, 0, idAgenda, objMov.Id_Mov, _objParametriServer, extraInt:=0)

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.ApriLavorazione() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

#End Region

#Region "Funzioni Pubbliche Piani Lavoro Etichette"

    Public Function ScriviPianoLavoroEtichette(ByVal objLavorazione As Lavorazione,
                                               ByVal messaggioDettagliato As Boolean,
                                               ByRef msgError As String
                                               ) As Integer

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim objAgenda As Operazione_Agenda
        Dim idAgenda As Integer = 0
        Dim flagOggettoCorretto As Boolean

        Try
            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyTestataEtichetta(objLavorazione, enum_TipoOperazioneDB.Scrittura)

            If flagOggettoCorretto Then

                objAgenda = GeneraPianoLavoroEtichette(Now, objLavorazione.Piva,
                                                       objLavorazione.Descrizione,
                                                       objLavorazione.Note,
                                                       objLavorazione.Lotto)

                If Not IsNothing(objAgenda) Then
                    Dim objAgendaHelper As New Agenda_Operazione_Helper
                    idAgenda = objAgendaHelper.Scrivi(objAgenda, _objParametriServer, flagUsaOraReale:=True)
                End If

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ LavorazioniHelper.ScriviPianoLavoroEtichette() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return idAgenda

    End Function

    Public Function AggiornaPianoLavoroEtichette(ByVal objLavorazione As Lavorazione,
                                                 ByVal messaggioDettagliato As Boolean,
                                                 ByRef msgError As String
                                                 ) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean

        Dim agendaHelper As New Agenda_Operazione_Helper
        Dim movHelper As New Agenda_Movimenti_Helper
        Dim objMovDetW As New Movimenti_Dettagli_W

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyTestataEtichetta(objLavorazione, enum_TipoOperazioneDB.Modifica)

            If flagOggettoCorretto Then

                'Prima di procedere con al modifica devo verificare se nel frattempo il record della movimenti dettagli è cambiato
                Dim dataUltimaModifica As DateTime
                Dim usernameUltimaModifica As String = ""
                Dim dataMovimento As Date

                dataUltimaModifica = UtilityHelper.UltimaModificaAgenda(objLavorazione.Piva,
                                                                        objLavorazione.IdAgenda,
                                                                        usernameUltimaModifica,
                                                                        dataMovimento,
                                                                        _objParametriServer)

                If Not IsNothing(objLavorazione.DataLettura) AndAlso dataUltimaModifica > objLavorazione.DataLettura Then
                    msgError &= UtilityHelper.GetMessaggioDataModifica(dataUltimaModifica, usernameUltimaModifica, _objParametriUtenti)
                    xRisp = False
                Else

                    'Tab AGENDA =  KEY + Des_Lib (x Descrizione)
                    xRisp = agendaHelper.ModificaPuntuale(piva:=objLavorazione.Piva,
                                                          saCod:=0,
                                                          idAgenda:=objLavorazione.IdAgenda,
                                                          objParametri:=_objParametriServer,
                                                          desLib:=objLavorazione.Descrizione
                                                          )

                    'Tab MOVIMENTO CARICO =  KEY + Mov_Desc (x Nota) + Extra_Str (x Lotto Default)
                    If xRisp = True Then

                        Dim idMov As Integer

                        If Not IsNothing(objLavorazione.IdMov) Then
                            idMov = objLavorazione.IdMov
                        Else
                            Dim objMov As Movimento = UtilityHelper.CercaMovimentoSpecificoSenzaDettagli(CAU_CARICO,
                                                                                                         objLavorazione.Piva,
                                                                                                         objLavorazione.IdAgenda,
                                                                                                         _objParametriServer)
                            If Not IsNothing(objMov) Then
                                idMov = objMov.Id_Mov
                            End If
                        End If

                        xRisp = movHelper.ModificaPuntuale(objLavorazione.Piva,
                                                           0,
                                                           objLavorazione.IdAgenda,
                                                           idMov,
                                                           _objParametriServer,
                                                           movDesc:=objLavorazione.Note,
                                                           extraStr:=objLavorazione.Lotto)

                        Dim flagSovrascriviLotto As Boolean = objLavorazione.Flag1

                        'Applico il lotto impostato come default in tutti i dettagli sottostanti
                        If xRisp = True AndAlso Not IsNothing(objLavorazione.Lotto) AndAlso flagSovrascriviLotto = True Then

                            xRisp = objMovDetW.ModificaPuntuale(objLavorazione.Piva,
                                                                Sa_Cod:=0,
                                                                Id_Agenda:=objLavorazione.IdAgenda,
                                                                Id_Mov:=objLavorazione.IdMov,
                                                                Id_Mov_Det:=0,
                                                                objParametri:=_objParametriServer,
                                                                Lotto:=objLavorazione.Lotto)
                        End If

                    End If

                End If

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.AggiornaPianoLavoroEtichette() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Function AggiornaPianoLavoroDettaglio(ByVal objLavorazione As Lavorazione,
                                                 ByVal messaggioDettagliato As Boolean,
                                                 ByRef msgError As String
                                                 ) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean = True

        Dim movDetHelper As New Agenda_Movimenti_Dettagli_Helper

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyCaricoScarico(objLavorazione,
                                                             enum_TipoOperazioneDB.Modifica,
                                                             flagUtilizzaDestinazione:=False)

            If flagOggettoCorretto Then

                'Prima di procedere con la modifica devo verificare se nel frattempo il record della movimenti dettagli è cambiato
                Dim dataUltimaModifica As DateTime
                Dim usernameUltimaModifica As String = ""
                Dim dataMovimento As Date

                dataUltimaModifica = UtilityHelper.UltimaModificaMovDettaglio(objLavorazione.Piva,
                                                                              0,
                                                                              objLavorazione.IdAgenda,
                                                                              objLavorazione.IdMov,
                                                                              objLavorazione.IdMovDet,
                                                                              usernameUltimaModifica,
                                                                              _objParametriServer)

                If Not IsNothing(objLavorazione.DataLettura) AndAlso dataUltimaModifica > objLavorazione.DataLettura Then
                    msgError &= UtilityHelper.GetMessaggioDataModifica(dataUltimaModifica, usernameUltimaModifica, _objParametriUtenti)
                    xRisp = False
                Else

                    Dim movDetDes As String = Nothing

                    If Not IsNothing(objLavorazione.MatCod) AndAlso objLavorazione.MatCod <> 0 AndAlso
                       Not IsNothing(objLavorazione.ElemCod) AndAlso objLavorazione.ElemCod <> 0 Then

                        Dim preparazioneDes As String = TipoLavorazioneDesFromAgenda(objLavorazione.Piva, objLavorazione.IdAgenda)
                        'Sto modificando il prodotto, quindi devo modificare anche la descrizione
                        Dim matDes As String = MatDesFromMatCod(objLavorazione.Piva, objLavorazione.ElemCod, objLavorazione.MatCod)
                        movDetDes = CreaMovDetDesLavorazione(CAU_CARICO, matDes, preparazioneDes)
                    End If

                    xRisp = movDetHelper.ModificaPuntuale(objLavorazione.Piva,
                                                          0,
                                                          objLavorazione.IdAgenda,
                                                          objLavorazione.IdMov,
                                                          objLavorazione.IdMovDet,
                                                          _objParametriServer,
                                                          elemCod:=objLavorazione.ElemCod,
                                                          matCod:=objLavorazione.MatCod,
                                                          movDetDes:=movDetDes,
                                                          udmCod:=objLavorazione.UdmCod,
                                                          udmCodExtra:=objLavorazione.UdmCodExtra,
                                                          qta:=objLavorazione.Qta,
                                                          calCod:=objLavorazione.CalCod,
                                                          lotto:=objLavorazione.Lotto,
                                                          qtaExtra:=Agro_Math.ArrotondaVal_Nothing(objLavorazione.QtaExtra, objLavorazione.CifreArrotondamento),
                                                          qtaExtraTotale:=objLavorazione.QtaExtraTotale,
                                                          tara:=objLavorazione.Tara,
                                                          qtaDettaglio1:=objLavorazione.NumContenitori,
                                                          qtaDettaglio2:=objLavorazione.NumImballaggi)

                    If xRisp = True Then
                        xRisp = UtilityHelper.AggiornaInserisciMatPrimeCamp(objLavorazione.CalCod,
                                                                            objLavorazione.MateriePrimeCampionature,
                                                                            _objParametriServer)
                    End If

                End If

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.AggiornaPianoLavoroDettaglio() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Function ScriviPianoLavoroDettaglio(ByVal objLavorazione As Lavorazione,
                                               ByVal messaggioDettagliato As Boolean,
                                               ByRef msgError As String
                                               ) As Integer

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim objMovDet As Movimento_Dettaglio
        Dim idMovDet As Integer = 0
        Dim flagOggettoCorretto As Boolean

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyCaricoScarico(objLavorazione,
                                                             enum_TipoOperazioneDB.Scrittura,
                                                             flagUtilizzaDestinazione:=False)

            If flagOggettoCorretto Then
                objMovDet = GeneraDettaglioCaricoScarico(CAU_CARICO,
                                                         objLavorazione.Piva,
                                                         0,
                                                         objLavorazione.IdAgenda,
                                                         0,
                                                         0,
                                                         objLavorazione.TipoLavorazioneDes,
                                                         objLavorazione.ElemCod,
                                                         objLavorazione.MatCod,
                                                         objLavorazione.MatDes,
                                                         objLavorazione.UdmCod,
                                                         objLavorazione.UdmCodExtra,
                                                         objLavorazione.Qta,
                                                         objLavorazione.CalCod,
                                                         objLavorazione.MateriePrimeCampionature,
                                                         objLavorazione.Lotto,
                                                         MagazzinoNONMovimentato,
                                                         Agro_Math.ArrotondaVal(objLavorazione.QtaExtra, objLavorazione.CifreArrotondamento),
                                                         objLavorazione.QtaExtraTotale,
                                                         objLavorazione.Tara,
                                                         objLavorazione.NumContenitori,
                                                         objLavorazione.NumImballaggi,
                                                         flagCreaDestinazione:=False,
                                                         objLavorazione.DataCreazioneMovDett,
                                                         objLavorazione.DataModificaMovDett)

                If Not IsNothing(objMovDet) Then
                    Dim objMovDetHelper As New Agenda_Movimenti_Dettagli_Helper
                    idMovDet = objMovDetHelper.Scrivi(objMovDet, _objParametriServer)
                End If

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ LavorazioniHelper.ScriviPianoLavoroDettaglio() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return idMovDet

    End Function

    Public Function CancellaPianoLavoroDettaglio(ByVal objLavorazione As Lavorazione,
                                                 ByVal messaggioDettagliato As Boolean,
                                                 ByRef msgError As String
                                                 ) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyCaricoScarico(objLavorazione,
                                                             enum_TipoOperazioneDB.Cancellazione,
                                                             flagUtilizzaDestinazione:=False)

            If flagOggettoCorretto Then

                Dim objMovDetHelper As New Agenda_Movimenti_Dettagli_Helper
                xRisp = objMovDetHelper.Cancella(objLavorazione.Piva,
                                                 0,
                                                 objLavorazione.IdAgenda,
                                                 objLavorazione.IdMov,
                                                 objLavorazione.IdMovDet,
                                                 _objParametriServer)

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.CancellaPianoLavoroDettaglio() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Function CancellaTotalePianoLavoroEtichette(ByVal objLavorazione As Lavorazione,
                                                       ByVal messaggioDettagliato As Boolean,
                                                       ByRef msgError As String
                                                       ) As Boolean

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean
        Dim objAgendaHelper As New Agenda_Operazione_Helper
        Dim objMovHelper As New Agenda_Movimenti_Helper


        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyTestataEtichetta(objLavorazione, enum_TipoOperazioneDB.Cancellazione)

            If flagOggettoCorretto Then

                Dim listMovimenti As List(Of Movimento) = objMovHelper.Leggi(objLavorazione.Piva,
                                                                             0,
                                                                             objLavorazione.IdAgenda,
                                                                             _objParametriServer)

                'Leggi elenco movimenti di carico
                Dim objMovCarico = listMovimenti.Find(Function(x) x.Cau_Mov = CAU_CARICO)

                If Not IsNothing(objMovCarico) AndAlso Not IsNothing(objMovCarico.Movimenti_Dettagli) AndAlso objMovCarico.Movimenti_Dettagli.Count > 0 Then

                    For Each movDet As Movimento_Dettaglio In objMovCarico.Movimenti_Dettagli

                        Dim objLav As New Lavorazione With {
                            .Piva = movDet.Piva,
                            .SaCod = movDet.Sa_Cod,
                            .IdAgenda = movDet.Id_Agenda,
                            .IdMov = movDet.Id_Mov,
                            .IdMovDet = movDet.Id_Mov_Det
                        }

                        xRisp = CancellaPianoLavoroDettaglio(objLav, messaggioDettagliato, msgError)

                        If xRisp = False Then
                            Exit For
                        End If
                    Next

                Else
                    xRisp = True
                End If

                'Se è andato tutto bene potrei lanciare objAgenda.Elimina per eliminare tutto il resto
                If xRisp = True Then
                    xRisp = objAgendaHelper.Cancella(objLavorazione.Piva,
                                                     0,
                                                     objLavorazione.IdAgenda,
                                                     cancellaAggancioRicetta:=False,
                                                     objParametri:=_objParametriServer)
                End If

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ LavorazioniHelper.CancellaTotalePianoLavoroEtichette() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

#End Region

#Region "Generazione Oggetti"

    Private Function GeneraAperturaAgendaLavorazione(ByVal lavCod As Integer,
                                                     ByVal dataMovimento As Date,
                                                     ByVal pivaInput As String,
                                                     ByVal saCod As Integer,
                                                     ByVal idDestinazione As Integer,
                                                     ByVal tipoDestinazione As Integer,
                                                     ByVal tipoLavorazioneDes As String,
                                                     ByVal descrizioneAggiuntiva As String,
                                                     ByVal lineaCod As Integer,
                                                     ByVal preparazioneCod As Integer,
                                                     ByVal idTrasformazione As Integer,
                                                     ByVal elemCod As Integer,
                                                     ByVal matCod As Integer,
                                                     ByVal matDes As String,
                                                     ByVal udmCod As Integer,
                                                     ByVal udmCodExtra As Integer,
                                                     ByVal qta As Decimal,
                                                     ByVal jollyInt As Integer,
                                                     ByVal lotto As String,
                                                     ByVal qtaExtra As Decimal,
                                                     ByVal qtaExtraTotale As Decimal,
                                                     ByVal numContenitori As Integer,
                                                     ByVal numImballaggi As Integer,
                                                     ByVal listMatPriCampValor As List(Of Materia_Prima_Campionatura),
                                                     ByVal CodMacchinaLav As String,
                                                     ByVal IdLavCollegata As Integer,
                                                     ByVal IdOrdCollegato As Integer
                                                     ) As Operazione_Agenda

        Dim objAgenda As Operazione_Agenda

        Try

            '------------------------------------------------
            '----- AGENDA
            '------------------------------------------------

            objAgenda = New Operazione_Agenda With {
                .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
                .Id_Agenda = 0,
                .Data = dataMovimento,
                .Piva = pivaInput,
                .Sa_Cod = 0,
                .Lav_Cod = lavCod,
                .Linea_Cod = lineaCod,
                .Preparazione_Cod = preparazioneCod,
                .Id_Trasformazione = idTrasformazione,
                .Des_Lib = AgronicaCoreContabBIZ.FF_LavorazioneBIZ.CreaDesLibLavorazione(tipoLavorazioneDes, matDes, lotto, descrizioneAggiuntiva)
            }

            '------------------------------------------------
            '----- MOVIMENTI
            '------------------------------------------------

            'MOVIMENTO DI TESTATA
            Dim objMovimentoT = New Movimento With {
                .Piva = objAgenda.Piva,
                .Sa_Cod = 0,
                .Data = objAgenda.Data,
                .Lav_Cod = lavCod,
                .Cau_Mov = CAU_LINEA_PRODUZIONE,
                .Mov_Desc = CreaMovDescLavorazione(CAU_LINEA_PRODUZIONE, tipoLavorazioneDes),
                .Ora = dataMovimento,
                .Extra_Date = #12/30/1899#,
                .Data_Registrazione = AGRODATAINIZIO,
                .Cod_Macchina_Lav = CodMacchinaLav
            }

            'MOVIMENTO DI SCARICO
            Dim objMovimentoS = New Movimento With {
                .Piva = objAgenda.Piva,
                .Sa_Cod = 0,
                .Data = objAgenda.Data,
                .Lav_Cod = lavCod,
                .Cau_Mov = CAU_SCARICO,
                .Mov_Desc = CreaMovDescLavorazione(CAU_SCARICO, tipoLavorazioneDes),
                .Scadenza_Extra = AGRODATAINIZIO,
                .Scadenza = AGRODATAFINE,
                .Ora = dataMovimento,
                .Extra_Date = #12/30/1899#,
                .Data_Registrazione = dataMovimento
            }

            'MOVIMENTO DI CARICO
            Dim objMovimentoC = New Movimento With {
                .Piva = objAgenda.Piva,
                .Sa_Cod = 0,
                .Data = objAgenda.Data,
                .Lav_Cod = lavCod,
                .Cau_Mov = CAU_CARICO,
                .Mov_Desc = CreaMovDescLavorazione(CAU_CARICO, tipoLavorazioneDes),
                .Scadenza_Extra = AGRODATAINIZIO,
                .Scadenza = AGRODATAFINE,
                .Ora = dataMovimento,
                .Extra_Date = #12/30/1899#,
                .Data_Registrazione = dataMovimento
            }

            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI
            '------------------------------------------------

            Dim objMovDettagli = New Movimento_Dettaglio With {
                .Piva = objAgenda.Piva,
                .Sa_Cod = saCod,
                .Lav_Cod = lavCod,
                .Elem_Cod = elemCod,
                .Pro_Cod = 0,
                .Mat_Cod = matCod,
                .Mov_Det_Des = CreaMovDetDesLavorazioneLavCod(lavCod, matDes, tipoLavorazioneDes),
                .Qta = qta,
                .Udm_Cod = udmCod,
                .Jolly_Int = jollyInt,
                .Extra_Int = idDestinazione,
                .Extra_Str = CStr(tipoDestinazione),
                .Extra_Date = #12/30/1899#,
                .Contabilizzato = NONCONTABILE,
                .Pendente = enum_Pendenza.MovGiustificato,
                .Lotto = lotto,
                .Udm_Cod_Extra = udmCodExtra,
                .Qta_Extra = qtaExtra,
                .Qta_Extra_Totale = qtaExtraTotale,
                .Qta_Dettaglio1 = numContenitori,
                .Qta_Dettaglio2 = numImballaggi,
                .Validita_Inizio = dataMovimento,
                .Validita_Fine = AGRODATAFINE
            }
            'HACK: In Extra_Str metto il Tipo_Destinazione e in Extra_Int l'Id_Destinazione

            '------------------------------------------------
            '----- MATERIE PRIME CAMPIONATURE
            '------------------------------------------------

            'Riempio la lista con i default vuoti
            'Valorizzo tutti i valori di default sempre o solo quando mi è arrivato almeno un elemento, a significare che mi interessa che siano presenti??!
            Dim objMatPriCampHelper As New Agenda_Materie_Prime_Campionature_Helper

            Dim listMateriePrimeCamp As List(Of Materia_Prima_Campionatura)
            listMateriePrimeCamp = objMatPriCampHelper.RiempiListaConDefault(pivaInput, listMatPriCampValor,
                                                                             _objParametriServer)

            'Aggancio la lista sul movimento dettaglio
            If Not IsNothing(listMateriePrimeCamp) AndAlso listMateriePrimeCamp.Count > 0 Then

                'Anche su tutte le Campionature devo impostare Validita_Inizio = Data del movimento che le crea
                For Each matCamp As Materia_Prima_Campionatura In listMateriePrimeCamp
                    matCamp.Validita_Inizio = dataMovimento
                Next

                objMovDettagli.Materie_Prime_Campionature = listMateriePrimeCamp
            End If

            'Aggancio il movimento dettaglio sul movimento
            objMovimentoC.Movimenti_Dettagli = New List(Of Movimento_Dettaglio) From {
                objMovDettagli
            }

            'Aggancio i movimenti sull'agenda
            objAgenda.Movimenti = New List(Of Movimento) From {
                objMovimentoT,
                objMovimentoC,
                objMovimentoS
            }

            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI RIFERIMENTI
            '------------------------------------------------

            'Dim MateriePrimeCampionatureLav = New List(Of Materia_Prima_Campionatura)
            'For Each mpcOrd In objTestataOrdine.MateriePrimeCampionature
            '    Dim mpcLav As New Materia_Prima_Campionatura With {.Tipo = mpcOrd.Tipo,
            '                                                   .Tipo_Cod = mpcOrd.Tipo_Cod,
            '                                                   .ChkTara_Campionatura = mpcOrd.ChkTara_Campionatura,
            '                                                   .Tara_Campionatura = mpcOrd.Tara_Campionatura
            '                                                  }
            '    MateriePrimeCampionatureLav.Add(mpcLav)
            'Next

            If IdLavCollegata <> 0 Or IdOrdCollegato <> 0 Then

                objAgenda.Agenda_Riferimenti = New List(Of Movimento_Dettaglio_Riferimento)

                If IdLavCollegata <> 0 Then
                    Dim objMovDetRifLav = New Movimento_Dettaglio_Riferimento With {
                                .Piva = objAgenda.Piva,
                                .Sa_Cod = 0,
                                .Id_Mov = -1,
                                .Id_Mov_Det = -1,
                                .Lav_Cod = lavCod,
                                .Cau_Mov = -1,
                                .Piva_Rif = objAgenda.Piva,
                                .Sa_Cod_Rif = 0,
                                .Id_Mov_Rif = -1,
                                .Id_Mov_Det_Rif = -1,
                                .Lav_Cod_Rif = LAVCOD_TRASFORMAZIONI,
                                .Cau_Mov_Rif = -1,
                                .Id_Agenda_Rif = IdLavCollegata}
                    'Aggancio il movimento dettaglio riferimento lavorazione sull'agenda
                    objAgenda.Agenda_Riferimenti.Add(objMovDetRifLav)
                End If

                If IdOrdCollegato <> 0 Then
                    Dim objMovDetRifOrd = New Movimento_Dettaglio_Riferimento With {
                                .Piva = objAgenda.Piva,
                                .Sa_Cod = 0,
                                .Id_Mov = -1,
                                .Id_Mov_Det = -1,
                                .Lav_Cod = lavCod,
                                .Cau_Mov = -1,
                                .Piva_Rif = objAgenda.Piva,
                                .Sa_Cod_Rif = 0,
                                .Id_Mov_Rif = -1,
                                .Id_Mov_Det_Rif = -1,
                                .Lav_Cod_Rif = LAVCOD_TESTATE_ORDINE_LAVORAZIONE,
                                .Cau_Mov_Rif = -1,
                                .Id_Agenda_Rif = IdOrdCollegato}
                    'Aggancio il movimento dettaglio riferimento ordine sull'agenda
                    objAgenda.Agenda_Riferimenti.Add(objMovDetRifOrd)
                End If

            End If

        Catch ex As Exception
            Throw New Exception("[ LavorazioniHelper.GeneraAperturaAgendaLavorazione() ] : " & ex.Message)
        End Try

        Return objAgenda

    End Function

    Private Function GeneraDettaglioCaricoScarico(ByVal cauMov As String,
                                                  ByVal pivaInput As String,
                                                  ByVal saCod As Integer,
                                                  ByVal idAgenda As Integer,
                                                  ByVal idDestinazione As Integer,
                                                  ByVal tipoDestinazione As Integer,
                                                  ByVal tipoLavorazioneDes As String,
                                                  ByVal elemCod As Integer,
                                                  ByVal matCod As Integer,
                                                  ByVal matDes As String,
                                                  ByVal udmCod As Integer,
                                                  ByVal udmCodExtra As Integer,
                                                  ByVal qta As Decimal,
                                                  ByVal calCod As Integer,
                                                  ByVal listMatPriCampValor As List(Of Materia_Prima_Campionatura),
                                                  ByVal lotto As String,
                                                  ByVal jollyInt As Integer,
                                                  ByVal qtaExtra As Decimal,
                                                  ByVal qtaExtraTotale As Decimal,
                                                  ByVal tara As Decimal,
                                                  ByVal numContenitori As Integer,
                                                  ByVal numImballaggi As Integer,
                                                  ByVal flagCreaDestinazione As Boolean,
                                                  ByVal dataCreazioneMovDett As DateTime,
                                                  ByVal dataModificaMovDett As DateTime
                                                  ) As Movimento_Dettaglio

        Dim objMovimentoDettaglio As Movimento_Dettaglio = Nothing
        Dim objDestinazione As Movimento_Destinazione

        Try

            Dim objMov As Movimento = UtilityHelper.CercaMovimentoSpecificoSenzaDettagli(cauMov,
                                                                                         pivaInput,
                                                                                         idAgenda,
                                                                                         _objParametriServer)

            If Not IsNothing(objMov) Then

                Dim dataMovimento As Date = objMov.Data

                Dim pendenza As Integer = enum_Pendenza.MovGiustificato
                If flagCreaDestinazione = False Then
                    pendenza = 0
                End If

                'se non mi è arrivato il mat_des, lo vado a cercare
                If matCod <> 0 AndAlso elemCod <> 0 AndAlso matDes = "" Then
                    matDes = MatDesFromMatCod(pivaInput, elemCod, matCod)
                End If

                'creo l'oggetto movimento dettaglio e tutti i suoi figli
                objMovimentoDettaglio = New Movimento_Dettaglio With {
                    .Piva = pivaInput,
                    .Sa_Cod = saCod,
                    .Id_Agenda = idAgenda,
                    .Id_Mov = objMov.Id_Mov,
                    .Id_Mov_Det = 0,
                    .Elem_Cod = elemCod,
                    .Pro_Cod = 0,
                    .Mat_Cod = matCod,
                    .Mov_Det_Des = CreaMovDetDesLavorazione(objMov.Cau_Mov, matDes, tipoLavorazioneDes),
                    .Udm_Cod = udmCod,
                    .Qta = qta,
                    .Contabilizzato = NONCONTABILE,
                    .Pendente = pendenza,
                    .Cal_Cod = calCod,
                    .Extra_Date = #12/30/1899#,
                    .Lotto = lotto,
                    .Jolly_Int = jollyInt,
                    .Udm_Cod_Extra = udmCodExtra,
                    .Qta_Extra = qtaExtra,
                    .Qta_Extra_Totale = qtaExtraTotale,
                    .Tara = tara,
                    .Qta_Dettaglio1 = numContenitori,
                    .Qta_Dettaglio2 = numImballaggi,
                    .Validita_Inizio = dataMovimento,
                    .Validita_Fine = AGRODATAFINE,
                    .Data_Creazione = dataCreazioneMovDett,
                    .Data_Modifica = dataModificaMovDett,
                    .Data = objMov.Data,
                    .Lav_Cod = objMov.Lav_Cod,
                    .Cau_Mov = cauMov
                }
                'TODO: Extra_Str = "1": Gestione Riduzione ad Udm Extra (più eventuali nuovi parametri che non possono essere gestiti altrimenti)???
                '.Extra_Str = "1",
                'TODO: Extra_Int = 1: Gestione Chain (Collegamento tra scarico e relativo carico di residuo)????
                '.Extra_Int = 1,

                If flagCreaDestinazione = True Then
                    'devo creare la destinazione
                    objDestinazione = New Movimento_Destinazione With {
                        .Piva = pivaInput,
                        .Sa_Cod = saCod,
                        .Id_Agenda = idAgenda,
                        .Id_Mov = objMov.Id_Mov,
                        .Id_Mov_Det = objMovimentoDettaglio.Id_Mov_Det,
                        .Id_Destinazione = idDestinazione,
                        .Tipo = tipoDestinazione,
                        .Qta = qta,
                        .Qta_Dest1 = numContenitori,
                        .Qta_Dest2 = numImballaggi,
                        .Data = dataMovimento
                    }

                    objMovimentoDettaglio.Movimenti_Destinazioni = New List(Of Movimento_Destinazione) From {
                        objDestinazione
                    }
                End If

                'Devo anche generare materie_prime_campionature; mi viene già passata la lista da fuori e la devo riempire con i default
                If calCod = 0 Then
                    Dim objMatPriCampHelper As New Agenda_Materie_Prime_Campionature_Helper

                    Dim listMateriePrimeCamp As List(Of Materia_Prima_Campionatura)
                    listMateriePrimeCamp = objMatPriCampHelper.RiempiListaConDefault(pivaInput, listMatPriCampValor,
                                                                                     _objParametriServer)

                    'Aggancio la lista sul movimento dettaglio
                    If Not IsNothing(listMateriePrimeCamp) AndAlso listMateriePrimeCamp.Count > 0 Then

                        'Anche su tutte le Campionature devo impostare Validita_Inizio = Data del movimento che le crea
                        For Each matCamp As Materia_Prima_Campionatura In listMateriePrimeCamp
                            matCamp.Validita_Inizio = dataMovimento
                        Next

                        objMovimentoDettaglio.Materie_Prime_Campionature = listMateriePrimeCamp
                    End If
                End If

            End If

        Catch ex As Exception
            Throw New Exception("[ LavorazioniHelper.GeneraDettaglioCaricoScarico() ] : " & ex.Message)
        End Try

        Return objMovimentoDettaglio

    End Function

    Private Function GeneraPianoLavoroEtichette(ByVal dataMovimento As Date,
                                                ByVal pivaInput As String,
                                                ByVal descrizione As String,
                                                ByVal note As String,
                                                ByVal lottoDefault As String
                                                ) As Operazione_Agenda

        Dim objAgenda As Operazione_Agenda

        Try
            '------------------------------------------------
            '----- AGENDA
            '------------------------------------------------
            objAgenda = New Operazione_Agenda With {
                .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
                .Id_Agenda = 0,
                .Data = AGRODATAINIZIO,
                .Piva = pivaInput,
                .Sa_Cod = 0,
                .Lav_Cod = LAVCOD_MONITORAGGIO_TEMPI_RIENTRO,
                .Des_Lib = descrizione
            }

            '------------------------------------------------
            '----- MOVIMENTI
            '------------------------------------------------

            'MOVIMENTO DI CARICO
            Dim objMovimentoC = New Movimento With {
                .Piva = objAgenda.Piva,
                .Sa_Cod = 0,
                .Data = objAgenda.Data,
                .Lav_Cod = LAVCOD_MONITORAGGIO_TEMPI_RIENTRO,
                .Cau_Mov = CAU_CARICO,
                .Mov_Desc = note,
                .Scadenza_Extra = AGRODATAINIZIO,
                .Scadenza = AGRODATAFINE,
                .Ora = dataMovimento,
                .Extra_Date = #12/30/1899#,
                .Data_Registrazione = dataMovimento,
                .Extra_Str = lottoDefault
            }

            'Aggancio i movimenti sull'agenda
            objAgenda.Movimenti = New List(Of Movimento) From {
                objMovimentoC
            }

        Catch ex As Exception
            Throw New Exception("[ LavorazioniHelper.GeneraPianoLavoroEtichette() ] : " & ex.Message)
        End Try

        Return objAgenda

    End Function

#End Region

#Region "Verifiche preliminari funzioni"

    Private Function CheckPropertyTestataLavorazione(ByRef objLavorazione As Lavorazione,
                                                     ByVal tipoOperazione As enum_TipoOperazioneDB
                                                     ) As Boolean

        Const nomeRoutine = "LavorazioniHelper.CheckPropertyTestataLavorazione()"
        Dim stringaErrore As String = ""

        Try

            UtilityHelper.ControllaStringEmpty(objLavorazione, Nothing, stringaErrore, "Piva")

            If tipoOperazione = enum_TipoOperazioneDB.Modifica OrElse
               tipoOperazione = enum_TipoOperazioneDB.Cancellazione Then
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "IdAgenda")
            End If

            If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "DataMovimento")
                UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "SaCod")
                UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "IdDestinazione")
                UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "TipoDestinazione")
                UtilityHelper.ControllaPresenza(objLavorazione, "", stringaErrore, "TipoLavorazioneDes")
                UtilityHelper.ControllaPresenza(objLavorazione, "", stringaErrore, "DescrizioneAggiuntiva")
                UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "LineaCod")
                UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "PreparazioneCod")
                UtilityHelper.ControllaPresenza(objLavorazione, 0, stringaErrore, "IdTrasformazione")
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "ElemCod")
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "MatCod")
                UtilityHelper.ControllaPresenza(objLavorazione, "", stringaErrore, "MatDes")
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "UdmCod")
                UtilityHelper.ControllaPresenza(objLavorazione, CInt(enum_UnitaMisura.KG), stringaErrore, "UdmCodExtra")
                UtilityHelper.ControllaPresenza(objLavorazione, 0, stringaErrore, "Qta")
                UtilityHelper.ControllaPresenza(objLavorazione, MagazzinoMovimentato, stringaErrore, "JollyInt")
                UtilityHelper.ControllaPresenza(objLavorazione, "", stringaErrore, "Lotto")
                UtilityHelper.ControllaPresenza(objLavorazione, 1, stringaErrore, "QtaExtra")
                UtilityHelper.ControllaPresenza(objLavorazione, 0, stringaErrore, "QtaExtraTotale")
                UtilityHelper.ControllaPresenza(objLavorazione, 0, stringaErrore, "NumContenitori")
                UtilityHelper.ControllaPresenza(objLavorazione, 0, stringaErrore, "NumImballaggi")
                'la lista di materie prime campionature se non ce l'ho va bene che rimanga nothing
            End If

            If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                'Se non mi viene passato non devo usare nessun valore di default perché in modifica se non ce l'ho non lo tocco

                'di fatto l'essenziale è che mi arrivino le chiavi e la data della lettura precedente

                UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "DataLettura")
                'UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "SaCod")
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "IdMov")
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "IdMovDet")
                UtilityHelper.ControllaPresenza(objLavorazione, 0, stringaErrore, "CalCod")
                'UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "IdDestinazione")
            End If

            If Not String.IsNullOrEmpty(stringaErrore) Then
                Throw New Exception(stringaErrore)
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return True

    End Function

    Private Function CheckPropertyCaricoScarico(ByRef objLavorazione As Lavorazione,
                                                ByVal tipoOperazione As enum_TipoOperazioneDB,
                                                ByVal flagUtilizzaDestinazione As Boolean
                                                ) As Boolean

        Const nomeRoutine = "LavorazioniHelper.CheckPropertyCaricoScarico()"
        Dim stringaErrore As String = ""

        Try

            UtilityHelper.ControllaStringEmpty(objLavorazione, Nothing, stringaErrore, "Piva")
            UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "IdAgenda")

            If tipoOperazione = enum_TipoOperazioneDB.Cancellazione OrElse
               tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "IdMov")
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "IdMovDet")

                If flagUtilizzaDestinazione = True Then
                    UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "SaCod")
                End If

            End If

            If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "DataLettura")

                If flagUtilizzaDestinazione = True Then
                    UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "IdDestinazione")
                End If
            End If

            If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then

                If flagUtilizzaDestinazione = True Then
                    UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "SaCod")
                    UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "IdDestinazione")
                    UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "TipoDestinazione")
                End If

                UtilityHelper.ControllaPresenza(objLavorazione, "", stringaErrore, "TipoLavorazioneDes")
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "ElemCod")
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "MatCod")
                UtilityHelper.ControllaPresenza(objLavorazione, "", stringaErrore, "MatDes")
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "UdmCod")
                UtilityHelper.ControllaPresenza(objLavorazione, CInt(enum_UnitaMisura.KG), stringaErrore, "UdmCodExtra")
                UtilityHelper.ControllaPresenza(objLavorazione, Decimal.Parse(0.0), stringaErrore, "Qta")
                UtilityHelper.ControllaPresenza(objLavorazione, 0, stringaErrore, "CalCod")
                UtilityHelper.ControllaPresenza(objLavorazione, "", stringaErrore, "Lotto")
                UtilityHelper.ControllaPresenza(objLavorazione, MagazzinoMovimentato, stringaErrore, "JollyInt")
                UtilityHelper.ControllaPresenza(objLavorazione, Decimal.Parse(1.0), stringaErrore, "QtaExtra")
                UtilityHelper.ControllaPresenza(objLavorazione, Decimal.Parse(0.0), stringaErrore, "QtaExtraTotale")
                UtilityHelper.ControllaPresenza(objLavorazione, Decimal.Parse(0.0), stringaErrore, "Tara")
                UtilityHelper.ControllaPresenza(objLavorazione, Decimal.Parse(0.0), stringaErrore, "NumContenitori")
                UtilityHelper.ControllaPresenza(objLavorazione, Decimal.Parse(0.0), stringaErrore, "NumImballaggi")
            End If



            If Not String.IsNullOrEmpty(stringaErrore) Then
                Throw New Exception(stringaErrore)
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return True

    End Function

    Private Function CheckPropertyTestataEtichetta(ByRef objLavorazione As Lavorazione,
                                                   ByVal tipoOperazione As enum_TipoOperazioneDB
                                                   ) As Boolean

        Const nomeRoutine = "LavorazioniHelper.CheckPropertyTestataEtichetta()"
        Dim stringaErrore As String = ""

        Try

            UtilityHelper.ControllaStringEmpty(objLavorazione, Nothing, stringaErrore, "Piva")

            If tipoOperazione = enum_TipoOperazioneDB.Modifica OrElse
               tipoOperazione = enum_TipoOperazioneDB.Cancellazione Then
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "IdAgenda")
            End If

            If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                UtilityHelper.ControllaNumZero(objLavorazione, Nothing, stringaErrore, "IdMov")
                UtilityHelper.ControllaPresenza(objLavorazione, Nothing, stringaErrore, "DataLettura")
            End If

            If tipoOperazione <> enum_TipoOperazioneDB.Cancellazione Then
                UtilityHelper.ControllaStringEmpty(objLavorazione, Nothing, stringaErrore, "Descrizione")
            End If

            If Not String.IsNullOrEmpty(stringaErrore) Then
                Throw New Exception(stringaErrore)
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return True

    End Function

#End Region

#Region "Giacenze"

    Private Sub AggiornaGiacenzeCampionature(ByVal cauMov As String,
                                             ByVal calCod As Integer,
                                             ByVal numConfezioni As Decimal,
                                             ByVal numContenitori As Decimal,
                                             ByVal numImballaggi As Decimal)

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Try
            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)
            'per evitare di fare query inutili, li aggiorno solo se sono <> 0, perché altrimenti la cosa sarebbe cmq ininfluente

            'aggiorno contenitori ("oconfezione")
            If numConfezioni <> 0 Then
                AggiornaGiacenzaCampionatura(cauMov, "oconfezione", calCod, numConfezioni)
            End If

            'aggiorno contenitori ("ocontenitore")
            If numContenitori <> 0 Then
                AggiornaGiacenzaCampionatura(cauMov, "ocontenitore", calCod, numContenitori)
            End If

            'aggiorno imballaggi ("oimballaggio")
            If numImballaggi <> 0 Then
                AggiornaGiacenzaCampionatura(cauMov, "oimballaggio", calCod, numImballaggi)
            End If


            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ LavorazioniHelper.AggiornaGiacenzeCampionature() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

    End Sub

    Private Sub AggiornaGiacenzaCampionatura(ByVal cauMov As String,
                                             ByVal tipo As String,
                                             ByVal calCod As Integer,
                                             ByVal numConfezionamento As Decimal)

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim objMatPrimeCampHelper As New Agenda_Materie_Prime_Campionature_Helper

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            Dim listCamp = objMatPrimeCampHelper.Leggi(calCod, tipo, 0, 0, _objParametriServer)

            If Not IsNothing(listCamp) AndAlso listCamp.Count > 0 Then
                'modifico
                objMatPrimeCampHelper.Modifica(listCamp(0).Progressivo,
                                               listCamp(0).Tipo,
                                               listCamp(0).Tipo_Cod,
                                               listCamp(0).Udm_Cod,
                                               _objParametriServer,
                                               valCod:=CStr(If(cauMov = CAU_SCARICO,
                                                               CDec(listCamp(0).Val_Cod) - numConfezionamento,
                                                               CDec(listCamp(0).Val_Cod) + numConfezionamento)))
                'se cauMov è scarico, allora sottraggo, altrimenti sommo
            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ LavorazioniHelper.AggiornaGiacenzaCampionatura() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

    End Sub

    Private Function ControllaGiacenza(ByVal objLavorazione As Lavorazione,
                                       ByVal objMovDet As Movimento_Dettaglio,
                                       ByVal data As Date,
                                       ByVal calCod As Integer,
                                       ByVal numConfezioniResidue As Decimal,
                                       ByVal numContenitoriResidui As Decimal,
                                       ByVal numImballaggiResidui As Decimal,
                                       ByVal kgNettiResidui As Decimal,
                                       ByVal kgLordiResidui As Decimal,
                                       ByVal messaggioDettagliato As Boolean,
                                       ByVal Flag_QtaNoZero As Boolean,
                                       ByVal Flag_QtaMaggioreZero As Boolean
                                       ) As String

        Dim risGiacenzaCheck As String
        Dim objMagazzinoBiz As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ

        Try
            Dim idDestinazione As Integer = 0
            If Not IsNothing(objLavorazione.IdDestinazione) Then
                idDestinazione = objLavorazione.IdDestinazione
            ElseIf Not IsNothing(objMovDet) AndAlso Not IsNothing(objMovDet.Movimenti_Destinazioni) AndAlso objMovDet.Movimenti_Destinazioni.Count = 1 Then
                idDestinazione = objMovDet.Movimenti_Destinazioni(0).Id_Destinazione
            End If

            Dim elemCod As Integer = 0
            If Not IsNothing(objLavorazione.ElemCod) Then
                elemCod = objLavorazione.ElemCod
            ElseIf Not IsNothing(objMovDet) AndAlso Not IsNothing(objMovDet.Elem_Cod) Then
                elemCod = objMovDet.Elem_Cod
            End If

            Dim matCod As Integer = 0
            If Not IsNothing(objLavorazione.MatCod) Then
                matCod = objLavorazione.MatCod
            ElseIf Not IsNothing(objMovDet) AndAlso Not IsNothing(objMovDet.Mat_Cod) Then
                matCod = objMovDet.Mat_Cod
            End If

            Dim lotto As String = ""
            If Not IsNothing(objLavorazione.Lotto) Then
                lotto = objLavorazione.Lotto
            ElseIf Not IsNothing(objMovDet) AndAlso Not IsNothing(objMovDet.Lotto) Then
                lotto = objMovDet.Lotto
            End If

            risGiacenzaCheck = objMagazzinoBiz.Controllo_Giacenza(objLavorazione.Piva,
                                                                  Sa_Cod:=objLavorazione.SaCod,
                                                                  strFabbricato_Cod:=idDestinazione,
                                                                  elemCod:=elemCod,
                                                                  nomeProdotto:="",
                                                                  CodiceProdotto:=matCod,
                                                                  Lotto:=lotto,
                                                                  Cal_Cod:=calCod,
                                                                  data:=data,
                                                                  cifreArrotondamentoPesi:=3,
                                                                  messaggioDettagliato:=messaggioDettagliato,
                                                                  checkPeso:=True,
                                                                  imballiDaScaricare:=numImballaggiResidui,
                                                                  contenitoriDaScaricare:=numContenitoriResidui,
                                                                  confezioniDaScaricare:=numConfezioniResidue,
                                                                  kgLordiDaScaricare:=kgLordiResidui,
                                                                  kgNettiDaScaricare:=kgNettiResidui,
                                                                  objParametriServer:=_objParametriServer,
                                                                  objParametriUtenti:=_objParametriUtenti,
                                                                  Flag_QtaNoZero:=Flag_QtaNoZero,
                                                                  Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero)

        Catch ex As Exception
            Throw New Exception("[ LavorazioniHelper.ControllaGiacenza() ] : " & ex.Message)
        End Try

        Return risGiacenzaCheck

    End Function

#End Region

#Region "Creazione Descrizioni e Messaggi"

    'Private Shared Function CreaDesLibLavorazione(ByVal tipoLavorazioneDes As String,
    '                                              ByVal matDes As String,
    '                                              ByVal lotto As String,
    '                                              ByVal descrizioneAggiuntiva As String) As String
    '    Dim stb As New StringBuilder

    '    If Not IsNothing(tipoLavorazioneDes) AndAlso Not String.IsNullOrEmpty(tipoLavorazioneDes) Then
    '        stb.Append(tipoLavorazioneDes)
    '    End If

    '    If Not IsNothing(matDes) AndAlso Not String.IsNullOrEmpty(matDes) Then
    '        stb.Append(If(stb.Length > 0, " ", "") & matDes)
    '    End If

    '    If Not IsNothing(lotto) AndAlso Not String.IsNullOrEmpty(lotto) Then
    '        stb.Append(If(stb.Length > 0, " ", "") & lotto)
    '    End If

    '    If Not IsNothing(descrizioneAggiuntiva) AndAlso Not String.IsNullOrEmpty(descrizioneAggiuntiva) Then
    '        stb.Append(If(stb.Length > 0, " ", "") & descrizioneAggiuntiva)
    '    End If

    '    Return stb.ToString

    'End Function

    Private Shared Function CreaMovDescLavorazione(ByVal cauMov As String, ByVal tipoLavorazioneDes As String) As String
        Select Case cauMov
            Case CAU_SCARICO
                Return "Scarico di Componenti da " & tipoLavorazioneDes
            Case CAU_CARICO
                Return "Carico di Preparati da " & tipoLavorazioneDes
            Case Else
                Return ""
        End Select
    End Function

    Private Shared Function CreaMovDetDesLavorazioneLavCod(ByVal lavCod As Integer,
                                                           ByVal matDes As String,
                                                           ByVal tipoLavorazioneDes As String) As String

        Dim descrOrdLav = OttieniDescrizioneLavCod(lavCod)

        Dim movMatDes = String.Format("{0} {1}",
                                      OttieniDescrizioneLavCod(lavCod),
                                      CreaMovDetDesLavorazione(CAU_CARICO, matDes, tipoLavorazioneDes)
                                      )

        Return movMatDes

    End Function

    Private Shared Function OttieniDescrizioneLavCod(lavCod As Integer) As String

        If lavCod = LAVCOD_TRASFORMAZIONI Then
            Return descrizioneLavorazione
        Else
            Return descrizioneOrdineLavorazione
        End If

    End Function

    Private Shared Function CreaMovDetDesLavorazione(ByVal cauMov As String,
                                                     ByVal matDes As String,
                                                     ByVal tipoLavorazioneDes As String) As String
        Select Case cauMov
            Case CAU_SCARICO
                Return "Scarico di " & matDes & " da " & tipoLavorazioneDes
            Case CAU_CARICO
                Return matDes
            Case Else
                Return ""
        End Select
    End Function

    Private Function MatDesFromMatCod(ByVal piva As String, ByVal elemCod As Integer, ByVal matCod As Integer) As String
        Dim matPrimeR As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Return matPrimeR.MatDes_from_MatCod(piva, elemCod, matCod, "", Nothing, "", _objParametriServer)
    End Function

    Private Function PreparazioneDesFromPreparazioneCod(ByVal piva As String, ByVal preparazioneCod As Integer) As String
        Dim lineePrepR As New Linee_Preparazioni_R
        Return lineePrepR.PreparazioneDes_From_PreparazioneCod(piva, preparazioneCod, _objParametriServer)
    End Function

    Private Function TipoLavorazioneDesFromAgenda(ByVal piva As String, ByVal idAgenda As Integer) As String
        Dim lineaCod As Integer = 0
        Dim preparazioneCod As Integer = 0

        Dim agendaR As New Agenda_R
        agendaR.LineaCodPreparazioneCod_From_IdAgenda(piva, idAgenda, lineaCod, preparazioneCod, _objParametriServer)

        Dim lineePreparazioniR As New Linee_Preparazioni_R

        Return lineePreparazioniR.PreparazioneDes_From_PreparazioneCod(piva, preparazioneCod, _objParametriServer)
    End Function

    Private Shared Function RicavoDescrizioneAggiuntiva(ByVal desLib As String, ByVal lotto As String) As String
        Dim descrizioneAggiuntiva As String = ""

        If desLib <> "" AndAlso lotto <> "" Then
            Dim tempS = desLib.Split(lotto)
            descrizioneAggiuntiva = Trim(tempS(1))
        End If

        Return descrizioneAggiuntiva
    End Function

#End Region

#Region "Utility Varie"

    Private Function GetCalCod(ByVal pivaInput As String,
                               ByVal saCod As Integer,
                               ByVal idAgenda As Integer,
                               ByVal idMov As Integer,
                               ByVal idMovDet As Integer,
                               ByRef numConfezioni As Decimal,
                               ByRef numContenitori As Decimal,
                               ByRef numImballaggi As Decimal,
                               ByRef kgNetti As Decimal,
                               ByRef kgLordi As Decimal,
                               ByRef objMovDet As Movimento_Dettaglio,
                               ByRef flagQtaExtraTotZero As Boolean?
                               ) As Integer

        Dim objMovDetHelper As New Agenda_Movimenti_Dettagli_Helper
        Dim calCod As Integer = 0

        Try
            ' Giulia: 7/11/2017: Il saCod non lo posso filtrare, perché il filtro viene applicato sulla movimenti, mentre andrebbe sulla movimenti_dettagli
            '   se dovesse capitare che vengono fuori più di una riga, andrà rivista la query (con Selezione_JoinCompleta) per filtrare il sa_cod sui dettagli
            '   e gli altri si dovranno adattare
            Dim listMovDet = objMovDetHelper.Leggi(pivaInput, 0, idAgenda, idMov, idMovDet, _objParametriServer)

            If Not IsNothing(listMovDet) AndAlso listMovDet.Count > 1 Then

                Throw New Exception("La query ha restituito " & listMovDet.Count & " risultati. Filtrare per Sa_Cod sulla Movimenti_Dettagli.")

            ElseIf Not IsNothing(listMovDet) AndAlso listMovDet.Count = 1 Then

                calCod = listMovDet(0).Cal_Cod

                If listMovDet(0).Udm_Cod = enum_UnitaMisura.Numero Then
                    numConfezioni = listMovDet(0).Qta
                Else
                    numConfezioni = 0
                End If
                numContenitori = listMovDet(0).Qta_Dettaglio1
                numImballaggi = listMovDet(0).Qta_Dettaglio2

                kgNetti = listMovDet(0).Qta_Extra_Totale
                kgLordi = listMovDet(0).Qta_Extra_Totale + listMovDet(0).Tara

                If listMovDet(0).Qta_Extra_Totale = 0 Then
                    flagQtaExtraTotZero = True
                End If

                objMovDet = listMovDet(0)
            End If

        Catch ex As Exception
            Throw New Exception("[ LavorazioniHelper.GetCalCod() ] : " & ex.Message)
        End Try

        Return calCod

    End Function

    Private Sub ValoriConfezionamenti(ByVal objLavorazione As Lavorazione,
                                      ByRef calCod As Integer,
                                      ByRef numConfezioniResidue As Decimal,
                                      ByRef numContenitoriResidui As Decimal,
                                      ByRef numImballaggiResidui As Decimal,
                                      ByRef kgNettiResidui As Decimal,
                                      ByRef kgLordiResidui As Decimal,
                                      ByVal flagValorizzaSempreResidui As Boolean,
                                      ByRef objMovDet As Movimento_Dettaglio,
                                      ByRef flagQtaExtraTotZero As Boolean?)

        Dim numConfezioniPrec As Decimal = 0
        Dim numContenitoriPrec As Decimal = 0
        Dim numImballaggiPrec As Decimal = 0
        Dim kgNettiPrec As Decimal = 0
        Dim kgLordiPrec As Decimal = 0

        Try

            If calCod = 0 Then
                calCod = GetCalCod(objLavorazione.Piva, objLavorazione.SaCod,
                                   objLavorazione.IdAgenda,
                                   objLavorazione.IdMov, objLavorazione.IdMovDet,
                                   numConfezioniPrec,
                                   numContenitoriPrec,
                                   numImballaggiPrec,
                                   kgNettiPrec, kgLordiPrec,
                                   objMovDet, flagQtaExtraTotZero)
            End If

            'per trovare il valore che dovrò SOMMARE alla giacenza (perché sono nel regno dei carichi)
            'devo fare [0 - (valore del movimento precedente) + (nuovo valore)]

            'se flagValorizzaSempreResidui, allora sto per esempio cancellando , quindi devo valorizzare le qta,
            'anche se le rispettive proprietà di objLavorazione non sono state valorizzate

            'TODO: UDM?
            'devo capire se ora sono usate le confezioni
            If Not IsNothing(objLavorazione.Qta) AndAlso Not IsNothing(objLavorazione.UdmCod) AndAlso
               objLavorazione.UdmCod = enum_UnitaMisura.Numero Then
                numConfezioniResidue = 0 - numConfezioniPrec + objLavorazione.Qta
            ElseIf flagValorizzaSempreResidui = True Then
                numConfezioniResidue = 0 - numConfezioniPrec
            End If

            If Not IsNothing(objLavorazione.NumContenitori) Then
                numContenitoriResidui = 0 - numContenitoriPrec + objLavorazione.NumContenitori
            ElseIf flagValorizzaSempreResidui = True Then
                numContenitoriResidui = 0 - numContenitoriPrec
            End If

            If Not IsNothing(objLavorazione.NumImballaggi) Then
                numImballaggiResidui = 0 - numImballaggiPrec + objLavorazione.NumImballaggi
            ElseIf flagValorizzaSempreResidui = True Then
                numImballaggiResidui = 0 - numImballaggiPrec
            End If

            Dim nettoLavorazione As Decimal = 0
            If Not IsNothing(objLavorazione.QtaExtraTotale) Then
                nettoLavorazione = objLavorazione.QtaExtraTotale
            End If

            Dim lordoLavorazione As Decimal = nettoLavorazione
            If Not IsNothing(objLavorazione.Tara) Then
                lordoLavorazione = nettoLavorazione + objLavorazione.Tara
            End If

            kgNettiResidui = 0 - kgNettiPrec + nettoLavorazione
            kgLordiResidui = 0 - kgLordiPrec + lordoLavorazione

        Catch ex As Exception
            Throw New Exception("[ LavorazioniHelper.ValoriConfezionamenti() ] : " & ex.Message)
        End Try

    End Sub

#End Region

End Class
