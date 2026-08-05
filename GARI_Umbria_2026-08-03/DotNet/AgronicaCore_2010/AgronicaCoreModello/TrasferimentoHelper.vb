Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility

Public Class Trasferimento

    Sub New()
        CifreArrotondamento = 6
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
    Public Property ElemCod As Integer?
    Public Property MatCod As Integer?
    Public Property MatDes As String
    Public Property UdmCod As Integer?
    Public Property Qta As Decimal?
    Public Property CalCod As Integer?
    Public Property Lotto As String
    Public Property JollyInt As Integer?
    Public Property UdmCodExtra As Integer?
    Public Property QtaExtra As Decimal?
    Public Property QtaExtraTotale As Decimal?
    Public Property Tara As Decimal?
    Public Property NumContenitori As Decimal?
    Public Property NumImballaggi As Decimal?
    Public Property Distanza_Trasporto_UDM As Integer?
    Public Property Distanza_Trasporto As Decimal?

    '---- START Proprietà Generiche - Jolly ----
    Public Property Flag1 As Boolean

    '---- END   Proprietà Generiche - Jolly ----


    'Contiene le chiavi che si vogliono modificare (nome colonna db), con il loro nuovo valore 
    '  Nome Tabella, Nome Colonna, Vecchia Chiave, Nuova Chiave
    ' (es: "Mov_destinazioni","Id_Destinazione","7896523","12345678")
    ' valore chiave memorizzata come stringa per coprire tutti i casi.
    Public Property ListaChiaviDaModificare As List(Of ChiaviDaModificare)

    Public Property ModuloGias As enum_Omni_Modulo_Generazione
    Public Property Confezionamenti As Contabilita_Riga_Confezionamento

    Public Property MagazzinoScarico As Contabilita_Magazzino
    Public Property MagazzinoCarico As Contabilita_Magazzino

    Public Property MagazzinoScaricoPrecedente As Contabilita_Magazzino
    Public Property MagazzinoCaricoPrecedente As Contabilita_Magazzino

    Public Property KgPrecedenti As Decimal?

    Public Property MovDettaglioRif As Movimento_Dettaglio_Riferimento

End Class

Public Class Trasferimento_Testata
    Public Property Piva As String
    Public Property SaCod As Integer?
    Public Property IdAgenda As Integer?
    Public Property IdMovS As Integer?
    Public Property IdMovC As Integer?
    Public Property Data As Date?
    Public Property DesLib As String
End Class

Public Class Trasferimento_Output
    Public Property Risultato As Boolean
    Public Property MsgError As String
    Public Property Time As DateTime
    Public Property Piva As String
    Public Property DescrizioneAgenda As String
    Public Property SaCodPrincipale As Integer?
    Public Property IdAgenda As Integer?
    Public Property IdMovS As Integer?
    Public Property IdMovC As Integer?
    Public Property IdMovDetS As Integer?
    Public Property IdMovDetC As Integer?
    Public Property TestataSalvata As Boolean
    Public Property Redirect As String

    Public Sub New()
        Risultato = False
        MsgError = ""
        Time = Now
        TestataSalvata = False
        DescrizioneAgenda = ""
        Redirect = ""
    End Sub
End Class

Public Class TrasferimentoHelper

    Private Const DESLIB_DEFAULT_TRASFERIMENTO = "Trasferimento di Magazzino"

    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri)

        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti
    End Sub

#Region "Funzioni Pubbliche Inserimento Trasferimento"

    Public Function ScriviTestataTrasferimento(ByVal objTrasferimento As Trasferimento,
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

            flagOggettoCorretto = True '(objLavorazione, enum_TipoOperazioneDB.Scrittura)

            If flagOggettoCorretto Then
                objAgenda = GeneraTestataTrasferimento(CDate(objTrasferimento.DataMovimento),
                                                       objTrasferimento.Piva,
                                                       CInt(objTrasferimento.SaCod),
                                                       objTrasferimento.DescrizioneAggiuntiva)

                If Not IsNothing(objAgenda) Then
                    Dim objAgendaHelper As New Agenda_Operazione_Helper
                    idAgenda = objAgendaHelper.Scrivi(objAgenda, _objParametriServer, flagUsaOraReale:=True)
                End If

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ LavorazioniHelper.ScriviTestataTrasferimento() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return idAgenda

    End Function

    Public Function ScriviDettaglioTrasferimentoScarico(ByVal piva As String,
                                                        ByVal idAgenda As Integer,
                                                        ByVal idMovS As Integer,
                                                        ByVal idMovC As Integer,
                                                        ByVal objTrasferimento As Trasferimento,
                                                        ByVal messaggioDettagliato As Boolean,
                                                        ByRef msgError As String,
                                                        Optional ByVal objTrasferimentoTestata As Trasferimento_Testata = Nothing
                                                        ) As Trasferimento_Output

        Const nomeRoutine = "TrasferimentoHelper.ScriviDettaglioTrasferimentoScarico()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = True
        Dim testataSalvata As Boolean = False
        Dim desLib As String = DESLIB_DEFAULT_TRASFERIMENTO

        Dim objMovDetScarico As Movimento_Dettaglio
        Dim objMovDetCarico As Movimento_Dettaglio
        Dim idMovDetScarico As Integer = 0
        Dim idMovDetCarico As Integer = 0
        Dim flagOggettoCorretto As Boolean

        Try
            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyCaricoScarico(objTrasferimento, enum_TipoOperazioneDB.Scrittura)

            If flagOggettoCorretto Then

                If idAgenda = 0 OrElse idMovS = 0 OrElse idMovC = 0 Then

                    'è la prima riga in assoluto, quindi devo creare anche la testata

                    'If objTrasferimentoTestata Is Nothing Then
                    '    Throw New Exception("objTrasferimento Testata Is Nothing ma idAgenda = 0")
                    'End If

                    'TODO: creo agenda + movimenti

                    'TODO: genero qui le chiavi in modo da averle già in canna senza dover rileggere

                    Dim objAgenda = GeneraTestataTrasferimento(CDate(objTrasferimento.DataMovimento),
                                                               objTrasferimento.Piva,
                                                               CInt(objTrasferimento.SaCod),
                                                               objTrasferimento.DescrizioneAggiuntiva)

                    If IsNothing(objAgenda) Then
                        Throw New Exception("Errore creazione objAgenda")
                    End If

                    Dim objAgendaHelper As New Agenda_Operazione_Helper
                    idAgenda = objAgendaHelper.Scrivi(objAgenda, _objParametriServer, flagUsaOraReale:=True)

                    'Ho scritto, quindi ora so quali sono gli idMov di carico e scarico
                    idMovS = objAgenda.Movimenti.Find(Function(x) x.Cau_Mov = CAU_SCARICO).Id_Mov
                    idMovC = objAgenda.Movimenti.Find(Function(x) x.Cau_Mov = CAU_CARICO).Id_Mov

                    desLib = objAgenda.Des_Lib
                    objTrasferimento.IdAgenda = idAgenda
                    testataSalvata = True

                Else
                    'non è la prima riga, quindi ho già scritto agenda e movimenti (le chiavi le dovrei già avere in canna e mi sono state passate dall'interfaccia)
                    objTrasferimento.IdAgenda = idAgenda
                End If

                'Genero Dettaglio SCARICO
                objMovDetScarico = GeneraDettaglioTrasferimento(CAU_SCARICO,
                                                                objTrasferimento.Piva,
                                                                CInt(objTrasferimento.SaCod),
                                                                idAgenda,
                                                                idMovS,
                                                                objTrasferimento)

                If objMovDetScarico Is Nothing Then
                    Throw New Exception("Non è stato possibile creare l'oggetto Riga Dettaglio Scarico")
                End If

                'Genero Dettaglio CARICO
                objMovDetCarico = GeneraDettaglioTrasferimento(CAU_CARICO,
                                                               objTrasferimento.Piva,
                                                               CInt(objTrasferimento.SaCod),
                                                               idAgenda,
                                                               idMovC,
                                                               objTrasferimento)

                If objMovDetCarico Is Nothing Then
                    Throw New Exception("Non è stato possibile creare l'oggetto Riga Dettaglio Carico")
                End If



                If Not IsNothing(objMovDetScarico) AndAlso Not IsNothing(objMovDetCarico) Then



                    'Dim numConfezioni As Decimal = 0
                    'If objLavorazione.UdmCod = enum_UnitaMisura.Numero AndAlso objLavorazione.Qta > 0 Then
                    '    numConfezioni = objLavorazione.Qta
                    'End If

                    'TODO: fare anche verifica della giacenza prima di scrivere
                    'If Not IsNothing(objTrasferimento.CalCod) AndAlso objTrasferimento.CalCod <> 0 Then
                    'Dim risGiacenzaCheck As String = ""
                    'If _flagSottoscortaConsentito = False Then
                    '    risGiacenzaCheck = ControllaGiacenza(objTrasferimento, objMovDet,
                    '                                         Now, objTrasferimento.CalCod,
                    '                                         numConfezioni,
                    '                                         objTrasferimento.NumContenitori,
                    '                                         objTrasferimento.NumImballaggi,
                    '                                         objTrasferimento.QtaExtraTotale,
                    '                                         (objTrasferimento.QtaExtraTotale + objTrasferimento.Tara),
                    '                                         messaggioDettagliato)
                    'End If

                    'If risGiacenzaCheck <> "" Then
                    '    msgError &= "Impossibile creare: Il prodotto non è più disponibile. " & risGiacenzaCheck
                    '    xRisp = False
                    'End If

                    'End If

                    If xRisp = True Then
                        Dim objMovDetHelper As New Agenda_Movimenti_Dettagli_Helper
                        idMovDetScarico = objMovDetHelper.Scrivi(objMovDetScarico, _objParametriServer)
                        idMovDetCarico = objMovDetHelper.Scrivi(objMovDetCarico, _objParametriServer)

                        If idMovDetScarico <> 0 AndAlso idMovDetCarico <> 0 Then

                            'TODO: che ca**o faccio qui con i sa_cod? in teoria è la stessa agenda, quindi dovrei mettere lo stesso che scrivo in agenda
                            'quindi sempre 0 oppure sempre quello di scarico

                            'devo generarmi la mov_dettagli riferimenti!
                            Dim objRiferimento As New Movimento_Dettaglio_Riferimento With {
                                .Piva = objMovDetScarico.Piva,
                                .Sa_Cod = CInt(objTrasferimento.SaCod),
                                .Id_Agenda = objMovDetScarico.Id_Agenda,
                                .Id_Mov = objMovDetScarico.Id_Mov,
                                .Id_Mov_Det = idMovDetScarico,
                                .Lav_Cod = LAVCOD_TRASFERIMENTO,
                                .Cau_Mov = objMovDetScarico.Cau_Mov,
                                .Piva_Rif = objMovDetCarico.Piva,
                                .Sa_Cod_Rif = CInt(objTrasferimento.SaCod),
                                .Id_Agenda_Rif = objMovDetCarico.Id_Agenda,
                                .Id_Mov_Rif = objMovDetCarico.Id_Mov,
                                .Id_Mov_Det_Rif = idMovDetCarico,
                                .Lav_Cod_Rif = LAVCOD_TRASFERIMENTO,
                                .Cau_Mov_Rif = objMovDetCarico.Cau_Mov,
                                .Qta = CDec(objTrasferimento.Qta),
                                .Data_Creazione = Now,
                                .Username_Creazione = _objParametriServer.UsernameOperazione
                            }
                            Dim objMovRifHelper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                            xRisp = objMovRifHelper.Scrivi(objRiferimento, _objParametriServer)


                            'per evitare problemi che poi non mi trovo il record, movimento sempre anche gli imballaggi,
                            'anche se teoricamente quando cella A e cella B sono sullo stesso centro, fanno riferimento allo stesso magazzino imballi,
                            'quindi mi sarei potuta risparmiare di scaricare gli imballi e ricaricarli sul medesimo magazzino

                            'Mi basta guardare uno dei due elem_cod, tanto sono uguali
                            If UtilityHelper.IsTrasformatoFF(objTrasferimento.ModuloGias, objMovDetScarico.Elem_Cod) Then

                                UtilityHelper.SistemaImballaggiDocumento(objTrasferimento.Piva, idAgenda, idMovS,
                                                                         enum_TipoOperazioneDB.Scrittura, _objParametriServer,
                                                                         objTrasferimento.ModuloGias, objMovDetScarico)

                                UtilityHelper.SistemaImballaggiDocumento(objTrasferimento.Piva, idAgenda, idMovC,
                                                                         enum_TipoOperazioneDB.Scrittura, _objParametriServer,
                                                                         objTrasferimento.ModuloGias, objMovDetCarico)

                            End If


                            If xRisp = True Then
                                'Ho scritto tutto quello che dovevo

                                If testataSalvata = False Then
                                    'Non ho scritto la testata, ma avendo cmq modificato l'operazione devo loggare a parte!!!
                                    LoggaModificaTrasferimento(objTrasferimento, desLib, enum_TipoOperazioneDB.Modifica)
                                End If


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
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return New Trasferimento_Output With {
            .Risultato = xRisp,
            .MsgError = msgError,
            .TestataSalvata = testataSalvata,
            .Piva = piva,
            .IdAgenda = idAgenda,
            .DescrizioneAgenda = desLib,
            .SaCodPrincipale = 0,
            .IdMovS = idMovS,
            .IdMovC = idMovC,
            .IdMovDetS = idMovDetScarico,
            .IdMovDetC = idMovDetCarico
        }

    End Function

    Private Sub LoggaModificaTrasferimento(ByRef objTrasferimento As Trasferimento, ByVal desLib As String, ByVal tipoOperazione As enum_TipoOperazioneDB)

        'Devo anche aggiornare data modifica dell'Agenda e scrivere Agronica_Log_Agenda

        'Di fatto mi serve solo per aggiornare la data di modifica della tabella Agenda
        Dim agendaW As New Agenda_W
        agendaW.ModificaPuntuale(objTrasferimento.Piva, 0, CInt(objTrasferimento.IdAgenda), _objParametriServer)

        If String.IsNullOrEmpty(desLib) Then
            desLib = DESLIB_DEFAULT_TRASFERIMENTO
        End If

        Dim objAgronicaLogAgendaW As New AgronicaLogAgenda_W
        objAgronicaLogAgendaW.Scrivi(CDate(objTrasferimento.DataMovimento),
                                     tipoOperazione,
                                     desLib,
                                     CLng(objTrasferimento.IdAgenda),
                                     objTrasferimento.Piva,
                                     0,
                                     LAVCOD_TRASFERIMENTO,
                                     CInt(enum_Id_Servizio.GiasOnline),
                                     _objParametriServer)
    End Sub

    Public Function AggiornaTrasferimentoScarico(ByVal objTrasferimento As Trasferimento,
                                                 ByVal messaggioDettagliato As Boolean,
                                                 ByRef msgError As String
                                                 ) As Trasferimento_Output

        Const nomeRoutine = "TrasferimentoHelper.AggiornaTrasferimentoScarico()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean
        Dim movDetHelper As New Agenda_Movimenti_Dettagli_Helper

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyCaricoScarico(objTrasferimento, enum_TipoOperazioneDB.Modifica)

            If flagOggettoCorretto Then

                'Elem_Cod è quello del prodotto che stai trattando che in realtà per ora per te è sempre TRASFORMATI_VEGETALI
                'Qui in pratica imposta il CAU_MOV da utilizzare e se ti ritorna CAU_CARICO non devi fare neanche il controllo giacenza

                Dim CheckGiacenze = UtilityHelper.SeCheckGiacenze(CInt(objTrasferimento.ElemCod), _objParametriUtenti)

                Dim risGiacenzaCheck As String = ""

                If CheckGiacenze.eseguiCheckGiacenze Then

                    'TODO: VERY IMPORTANT!!! la verifica di giacenza la devo fare sulla sola differenza, perché il vecchio movimenti qui è ancora conteggiato!!!!

                    'TODO: prima di farlo devo ovviamente verificare la giacenza:
                    If objTrasferimento.QtaExtraTotale < objTrasferimento.KgPrecedenti Then

                        'TODO: se sto diminuendo la qta:
                        '   --> nella cella di scarico posso risparmiarmi il controllo giacenza (se andava bene 100, allora va bene anche 90)
                        '   --> nella cella di carico devo vedere la "giacenza al 31/12/2100 se è sufficiente" ?!?
                        '           (va bene in FF, perché tanto ogni carico è un nuovo Cal_Cod, ma nel resto dell'universo no,
                        '               perché anche se alla fine la giacenza torna potrei violare la giacenza istantanea)

                        'Visto che il vecchio valore è ancora presente sul db, devo verificare solo se c'è giacenza per la differenza
                        '   tra quello che c'era prima ed il nuovo valore
                        Dim qtaDaControllare As Decimal = CDec(objTrasferimento.KgPrecedenti) - CDec(objTrasferimento.QtaExtraTotale)

                        risGiacenzaCheck = ControllaGiacenza(objTrasferimento,
                                                             Nothing,
                                                             objTrasferimento.MagazzinoCarico,
                                                             AGRODATAFINE,
                                                             qtaDaControllare,
                                                             messaggioDettagliato,
                                                             CheckGiacenze.flag_QtaNoZero,
                                                             CheckGiacenze.flag_QtaMaggioreZero)

                    ElseIf objTrasferimento.QtaExtraTotale > objTrasferimento.KgPrecedenti Then

                        'TODO: se sto aumentando la qta:
                        '   --> nella cella di scarico devo vedere la giacenza alla data dell'operazione 
                        '           [ed anche al 31/12/2100 ?!?]
                        '   --> nella cella di carico posso risparmiarmi il controllo giacenza (tanto sto aumentando il quantitativo rispetto a ciò che c'era prima)

                        'Visto che il vecchio valore è ancora presente sul db, devo verificare solo se c'è giacenza per la differenza
                        '   tra il nuovo valore e quello che c'era prima
                        Dim qtaDaControllare As Decimal = CDec(objTrasferimento.QtaExtraTotale) - CDec(objTrasferimento.KgPrecedenti)


                        'Giacenza alla data dell'operazione (avrei avuto quella giacenza se avessi usato fin dall'inizio questa quantità?)
                        risGiacenzaCheck = ControllaGiacenza(objTrasferimento,
                                                             Nothing,
                                                             objTrasferimento.MagazzinoScarico,
                                                             CDate(objTrasferimento.DataMovimento),
                                                             qtaDaControllare,
                                                             messaggioDettagliato,
                                                             CheckGiacenze.flag_QtaNoZero,
                                                             CheckGiacenze.flag_QtaMaggioreZero)

                        'Giacenza al 31/12/2100 (qualcuno successivamente alla mia operazione ha già consumato il prodotto?)
                        risGiacenzaCheck = risGiacenzaCheck & ControllaGiacenza(objTrasferimento,
                                                                                Nothing,
                                                                                objTrasferimento.MagazzinoScarico,
                                                                                AGRODATAFINE,
                                                                                qtaDaControllare,
                                                                                messaggioDettagliato,
                                                                                CheckGiacenze.flag_QtaNoZero,
                                                                                CheckGiacenze.flag_QtaMaggioreZero)

                    Else
                        'è rimasto tutto invariato ==> non faccio verifiche giacenza, ma neanche salvo

                        'Controllo che non sia imputata distanza
                        If objTrasferimento.Distanza_Trasporto = 0 Then
                            Throw New Exception("Quantità invariata. Annullare modifica")
                        End If

                    End If

                        If risGiacenzaCheck <> "" Then
                        msgError &= "Impossibile modificare: Il prodotto andrebbe sotto giacenza; " & risGiacenzaCheck
                        xRisp = False
                    Else
                        xRisp = True
                    End If

                Else
                    xRisp = True
                End If

                If xRisp = True Then

                    xRisp = ModificaQtaScarico(objTrasferimento, CDate(objTrasferimento.DataMovimento))

                    If xRisp = True AndAlso UtilityHelper.IsTrasformatoFF(objTrasferimento.ModuloGias, CInt(objTrasferimento.ElemCod)) Then

                        'TODO: dopo aver scritto i dati aggiornati, faccio di nuovo partire il sistema imballaggi sia su carico che su scarico
                        '      (questo dovrebbe sistemare da solo i relativi carichi/scarichi di imballi)


                        'Piuttosto che leggere sul db (dove mi andrebbe a tirare su anche i suo figli), me ne creo uno al volo con le sole proprietà che mi servono?!?

                        Dim listDettaglioScarico = movDetHelper.Leggi(objTrasferimento.Piva, 0,
                                                                      CInt(objTrasferimento.IdAgenda),
                                                                      CInt(objTrasferimento.IdMov),
                                                                      CInt(objTrasferimento.IdMovDet),
                                                                      _objParametriServer,
                                                                      leggiEsclusivamenteDettaglio:=True)

                        If listDettaglioScarico Is Nothing OrElse listDettaglioScarico.Count <> 1 Then
                            Throw New Exception("Impossibile leggere dettaglio scarico appena scritto.")
                        End If

                        Dim objDettaglioScarico As Movimento_Dettaglio = listDettaglioScarico(0)
                        UtilityHelper.SistemaImballaggiDocumento(objTrasferimento.Piva, CInt(objTrasferimento.IdAgenda), CInt(objTrasferimento.IdMov),
                                                                 enum_TipoOperazioneDB.Modifica, _objParametriServer,
                                                                 objTrasferimento.ModuloGias, objDettaglioScarico)



                        Dim listDettaglioCarico = movDetHelper.Leggi(objTrasferimento.Piva, 0,
                                                                     CInt(objTrasferimento.IdAgenda),
                                                                     objTrasferimento.MovDettaglioRif.Id_Mov_Rif,
                                                                     objTrasferimento.MovDettaglioRif.Id_Mov_Det_Rif,
                                                                     _objParametriServer,
                                                                     leggiEsclusivamenteDettaglio:=True)

                        If listDettaglioCarico Is Nothing OrElse listDettaglioScarico.Count <> 1 Then
                            Throw New Exception("Impossibile leggere dettaglio carico appena scritto.")
                        End If

                        Dim objDettaglioCarico As Movimento_Dettaglio = listDettaglioCarico(0)
                        UtilityHelper.SistemaImballaggiDocumento(objTrasferimento.Piva, CInt(objTrasferimento.IdAgenda), objTrasferimento.MovDettaglioRif.Id_Mov_Rif,
                                                                 enum_TipoOperazioneDB.Modifica, _objParametriServer,
                                                                 objTrasferimento.ModuloGias, objDettaglioCarico)


                        '===========================================================================================================
                        'Gestione ETD in base alla distanza di trasporto
                        '-----------------------------------------------------------------------------------------------------------

                        'Aggiornamento Valori in tabella Mov_Dettaglio_Tecnico_Extra
                        Dim ObjMov_Dettaglio_Tecnico_Extra As New AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_W
                        ObjMov_Dettaglio_Tecnico_Extra.ModificaPuntuale(objTrasferimento.Piva, 0, objDettaglioCarico.Id_Agenda, objDettaglioCarico.Id_Mov, objDettaglioCarico.Id_Mov_Det, 0, _objParametriServer, DistanzaTrasportoUdm:=objTrasferimento.Distanza_Trasporto_UDM, DistanzaTrasporto:=objTrasferimento.Distanza_Trasporto)


                        If CInt(objDettaglioCarico.Cal_Cod) <> 0 Then

                            '1. Lettura dello stato dell'indirizzo del centro aziendale della cella di destinazione
                            Dim ObjCentrixIndirizzi As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
                            Dim codiceStato As String = ""
                            Dim DTIndirizzo As DataTable = ObjCentrixIndirizzi.Leggi_dato_centro(objTrasferimento.Piva, objTrasferimento.MagazzinoCarico.SaCod, "stato <> ''", "", _objParametriServer)

                            If DTIndirizzo.Rows.Count > 0 Then
                                codiceStato = DTIndirizzo(0)("stato")
                            End If


                            '2. Calcolo ETD per la nuova distanza
                            Dim leggiGHG As New AgronicaCoreContabDAL.GHG_Registrazioni_R

                            Dim Standard_Factor As Decimal = leggiGHG.CalcoloStandard_Factor(objTrasferimento.Distanza_Trasporto,
                                                                                             0,
                                                                                             objTrasferimento.Distanza_Trasporto_UDM,
                                                                                             1,
                                                                                             codiceStato,
                                                                                             "",
                                                                                             _objParametriServer)

                            '3. Lettura dei parametri della tabella materie_prime_campionature dello scarico
                            Dim Leggi_MPC As New AgronicaCoreContabDAL.Materie_Prime_Campionature_R
                            Dim DT_MPC As DataTable

                            Dim xFiltroAggiuntivo = "lower(Tipo) In ('oghgforetd', 'okmdistance')"
                            DT_MPC = Leggi_MPC.Leggi(objTrasferimento.CalCod, "", 0, 0, 0, "", True, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, "", _objParametriServer)

                            '4. Modifica Parametri ETD
                            If DT_MPC.Rows.Count > 0 Then

                                Dim Scrivi_MPC As New AgronicaCoreContabDAL.Materie_Prime_Campion_W
                                Dim oghgforetd As Decimal = 0
                                Dim okmdistance As Decimal = 0
                                Dim Val_Cod As Decimal = 0
                                Dim Delta As Decimal = 1

                                For Each dr_MPC In DT_MPC.Rows

                                    Select Case LCase(dr_MPC("Tipo"))

                                        Case "oghgforetd"

                                            oghgforetd = 0
                                            If IsNumeric(dr_MPC("Val_Cod")) Then
                                                oghgforetd = Replace(dr_MPC("Val_Cod"), ".", ",")
                                            End If
                                            'Aggiungo l'etd a quello già presente in scarico
                                            oghgforetd = oghgforetd + Standard_Factor

                                            Val_Cod = oghgforetd

                                        Case "okmdistance"

                                            okmdistance = 0
                                            If IsNumeric(dr_MPC("Val_Cod")) Then
                                                okmdistance = Replace(dr_MPC("Val_Cod"), ".", ",")
                                            End If

                                            If objTrasferimento.Distanza_Trasporto_UDM = enum_UnitaMisura.Miglia Then
                                                'Conversione in KM
                                                Delta = 1.60934
                                            End If

                                            'Aggiungo l'etd a quello già presente in scarico
                                            okmdistance = okmdistance + (objTrasferimento.Distanza_Trasporto * Delta)

                                            Val_Cod = okmdistance

                                        Case Else

                                            '...

                                    End Select

                                    Scrivi_MPC.Modifica(CInt(objDettaglioCarico.Cal_Cod), dr_MPC("Tipo"), dr_MPC("Tipo_Cod"), dr_MPC("Udm_Cod"), _objParametriServer, Val_Cod, Val_Cod_Numerico:=True)

                                Next

                            End If

                        End If

                    End If

                End If




                ''Prima di procedere con al modifica devo verificare se nel frattempo il record della movimenti dettagli è cambiato
                'Dim dataUltimaModifica As DateTime
                'Dim usernameUltimaModifica As String = ""

                'dataUltimaModifica = UtilityHelper.UltimaModificaMovDettaglio(objTrasferimento.Piva,
                '                                                              objTrasferimento.SaCod,
                '                                                              objTrasferimento.IdAgenda,
                '                                                              objTrasferimento.IdMov,
                '                                                              objTrasferimento.IdMovDet,
                '                                                              usernameUltimaModifica,
                '                                                              _objParametriServer)


                'If Not IsNothing(objTrasferimento.DataLettura) AndAlso dataUltimaModifica > objTrasferimento.DataLettura Then
                '    msgError &= UtilityHelper.GetMessaggioDataModifica(dataUltimaModifica, usernameUltimaModifica, _objParametriUtenti)
                '    xRisp = False
                'Else

                'End If

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return New Trasferimento_Output With {
            .Risultato = xRisp,
            .MsgError = msgError
        }
        '.TestataSalvata = testataSalvata,
        '.Piva = piva,
        '.IdAgenda = idAgenda,
        '.DescrizioneAgenda = desLib,
        '.SaCodPrincipale = 0,
        '.IdMovS = idMovS,
        '.IdMovC = idMovC,
        '.IdMovDetS = idMovDetScarico,
        '.IdMovDetC = idMovDetCarico
    End Function

    Public Function AggiornaTrasferimentoCarico(ByVal objTrasferimento As Trasferimento,
                                                ByVal messaggioDettagliato As Boolean,
                                                ByRef msgError As String
                                                ) As Trasferimento_Output

        Const nomeRoutine = "TrasferimentoHelper.AggiornaTrasferimentoCarico()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean

        Dim movDetHelper As New Agenda_Movimenti_Dettagli_Helper
        Dim movDestHelper As New Agenda_Movimenti_Destinazioni_Helper

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyCaricoScarico(objTrasferimento, enum_TipoOperazioneDB.Modifica)

            If flagOggettoCorretto Then

                Dim CheckGiacenze = UtilityHelper.SeCheckGiacenze(CInt(objTrasferimento.ElemCod), _objParametriUtenti)

                Dim risGiacenzaCheck As String = ""

                If CheckGiacenze.eseguiCheckGiacenze Then

                    'TODO: prima di cancellare devo fare un controllo di giacenza sul vecchio magazzino di carico per assicurarmi
                    ' che venendo meno questo carico di prodotto, le eventuali operazioni successive abbiano ancora la giacenza disponibile
                    'controllo giacenza per intera qta movimentata sul vecchio magazzino di carico in data 31/12/2100

                    risGiacenzaCheck = ControllaGiacenza(objTrasferimento,
                                                         Nothing,
                                                         objTrasferimento.MagazzinoCaricoPrecedente,
                                                         AGRODATAFINE,
                                                         CDec(objTrasferimento.QtaExtraTotale),
                                                         messaggioDettagliato,
                                                         CheckGiacenze.flag_QtaNoZero,
                                                         CheckGiacenze.flag_QtaMaggioreZero)

                    If risGiacenzaCheck <> "" Then
                        msgError &= "Impossibile modificare: Il prodotto andrebbe sotto giacenza; " & risGiacenzaCheck
                        xRisp = False
                    Else
                        xRisp = True
                    End If
                Else
                    xRisp = True
                End If

                If xRisp = True Then

                    'xRisp = ModificaQtaScarico(objTrasferimento, objTrasferimento.DataMovimento, messaggioDettagliato, msgError)

                    'TODO: in questo caso modifico il magazzino di carico che è chiave, quindi devo cancellare tutto il carico (dettaglio, destinazione)



                    'TODO: non ho in realtà bisogno di cancellare la mov riferimenti perché qui non specifico il sa_cod, quindi se tengo gli stessi id_mov_det non ci sono problemi

                    'TODO: LEGGO(?) il mov_dettaglio precedente [per avere tutti gli id] poi cancello l'originale, cambio il magazzino e riscrivo

                    'Leggo cosa c'è scritto ora sul db
                    Dim movDetPrecedentiList = movDetHelper.Leggi(objTrasferimento.Piva, 0,
                                                                  CInt(objTrasferimento.IdAgenda),
                                                                  CInt(objTrasferimento.IdMov),
                                                                  CInt(objTrasferimento.IdMovDet),
                                                                  _objParametriServer)

                    If movDetPrecedentiList Is Nothing OrElse movDetPrecedentiList.Count <> 1 Then
                        Throw New Exception("ERRORE: sono in modifica ma non ho trovato il movimento precedente")
                    End If


                    Dim movDetPrecedente As Movimento_Dettaglio = movDetPrecedentiList(0)

                    'TODO: ATTENZIONE: questa cancellazione in realtà elimina solo Dettaglio + Destinazione (non Riferimenti),
                    '   mi va bene così, tanto sulla riferimenti non devo variare nulla

                    Dim msgCancellazione As String = ""
                    Dim modProtetta As Integer = 0
                    Dim r As AgronicaCoreVarieBIZ.RispostaStandard = UtilityHelper.CancellaDocumento(_objParametriServer,
                                                                                                     movDetPrecedente.Piva,
                                                                                                     movDetPrecedente.Id_Agenda,
                                                                                                     movDetPrecedente.Id_Mov,
                                                                                                     movDetPrecedente.Id_Mov_Det,
                                                                                                     LAVCOD_TRASFERIMENTO,
                                                                                                     enum_TipoOperazioneDB.Cancellazione,
                                                                                                     IgnoraAvvisoWarning:=True,
                                                                                                     Bypass_Delete_Exceptions:=True,
                                                                                                     ModuloGiasLicenziato:=objTrasferimento.ModuloGias,
                                                                                                     flagAggiornaConteggi:=False,
                                                                                                     messaggio:=msgCancellazione,
                                                                                                     Modalita_Protetta:=modProtetta,
                                                                                                     objParametriUtenti:=_objParametriUtenti)

                    If r Is Nothing OrElse r.RispostaOK = False Then
                        Throw New Exception("Impossibile sovrascrivere il dettaglio! Errore nella cancellazione")
                    End If


                    'TODO: visto che per il momento posso cambiare solo il magazzino, mi risparmio di rigenerare il nuovo oggetto con i dati di interfaccia,
                    ' che potrebbero essere mancanti, invece vado direttamente a cambiare i dati del magazzino, da quello letto precedentemente.

                    'Dim objDettaglio As Movimento_Dettaglio = GeneraRigaContabilita(objContabDettaglio,
                    '                                                                lavCod, cauMov,
                    '                                                                idMovT, idMovCS,
                    '                                                                dataOp, usernameOperatore,
                    '                                                                movDetPrecedente)

                    Dim objDettaglio As Movimento_Dettaglio = movDetPrecedente.DeepCloneObject()

                    objDettaglio.Sa_Cod = CInt(objTrasferimento.MagazzinoCarico.SaCod)
                    objDettaglio.Username_Modifica = _objParametriServer.UsernameOperazione
                    objDettaglio.Data_Modifica = Now

                    objDettaglio.Movimenti_Destinazioni(0).Sa_Cod = CInt(objTrasferimento.MagazzinoCarico.SaCod)
                    objDettaglio.Movimenti_Destinazioni(0).Tipo = CInt(objTrasferimento.MagazzinoCarico.TipoDestinazione)
                    objDettaglio.Movimenti_Destinazioni(0).Id_Destinazione = CInt(objTrasferimento.MagazzinoCarico.IdDestinazione)
                    objDettaglio.Movimenti_Destinazioni(0).Username_Modifica = _objParametriServer.UsernameOperazione
                    objDettaglio.Movimenti_Destinazioni(0).Data_Modifica = Now

                    'se FF + 210 avevo scritto anche tecnico extra, quindi devo modificare il sa_cod anche di quello
                    If UtilityHelper.IsTrasformatoFF(objTrasferimento.ModuloGias, CInt(objTrasferimento.ElemCod)) Then

                        If objDettaglio.Movimenti_Dettagli_Tecnici_Extra Is Nothing OrElse objDettaglio.Movimenti_Dettagli_Tecnici_Extra.Count <> 1 Then
                            Throw New Exception("Non è presente il Movimento_Dettaglio_Tecnico_Extra ma questo dettaglio è un FF 210")
                        End If

                        objDettaglio.Movimenti_Dettagli_Tecnici_Extra(0).Sa_Cod = CInt(objTrasferimento.MagazzinoCarico.SaCod)
                        objDettaglio.Movimenti_Dettagli_Tecnici_Extra(0).Username_Modifica = _objParametriServer.UsernameOperazione
                        objDettaglio.Movimenti_Dettagli_Tecnici_Extra(0).Data_Modifica = Now

                    End If

                    'Il riferimento è stato tirato su nella lettura, ma me lo scriverebbe in senso inverso e con sa_cod valorizzato
                    '(e visto che tanto non era mai stato eliminato, azzero la lista, in modo che non venga toccato la Mov_Dettagli_Riferimenti)
                    If objDettaglio.Movimenti_Dettagli_Riferimenti IsNot Nothing Then
                        objDettaglio.Movimenti_Dettagli_Riferimenti.Clear()
                    End If



                    If objDettaglio Is Nothing Then
                        Throw New Exception("Non è stato possibile creare l'oggetto Riga Dettaglio")
                    End If

                    Dim objDetHelper As New Agenda_Movimenti_Dettagli_Helper
                    Dim idMovDet = objDetHelper.Scrivi(objDettaglio, _objParametriServer)

                    If idMovDet > 0 Then

                        'Devo anche aggiornare data modifica dell'agenda e scrivere Agronica_Log_Agenda
                        LoggaModificaTrasferimento(objTrasferimento, "", enum_TipoOperazioneDB.Modifica)

                        If UtilityHelper.IsTrasformatoFF(objTrasferimento.ModuloGias, CInt(objTrasferimento.ElemCod)) Then

                            'TODO: se avevo scritto i movimenti imballi, devo andare a cambiare il sa_cod anche su questi
                            '(attenzione che avendo cambiato centro devo anche ricavarmi il nuovo id_destinazione del magazzino imballi)

                            UtilityHelper.SistemaImballaggiDocumento(objTrasferimento.Piva, CInt(objTrasferimento.IdAgenda), CInt(objTrasferimento.IdMov),
                                                                     enum_TipoOperazioneDB.Modifica, _objParametriServer,
                                                                     objTrasferimento.ModuloGias, objDettaglio)

                        End If

                    Else
                        Throw New Exception("ERRORE: non sono riuscito a riscrivere il movimento dettaglio")
                    End If

                End If







                ''Prima di procedere con al modifica devo verificare se nel frattempo il record della movimenti dettagli è cambiato
                'Dim dataUltimaModifica As DateTime
                'Dim usernameUltimaModifica As String = ""

                'dataUltimaModifica = UtilityHelper.UltimaModificaMovDettaglio(objTrasferimento.Piva,
                '                                                              objTrasferimento.SaCod,
                '                                                              objTrasferimento.IdAgenda,
                '                                                              objTrasferimento.IdMov,
                '                                                              objTrasferimento.IdMovDet,
                '                                                              usernameUltimaModifica,
                '                                                              _objParametriServer)


                'If Not IsNothing(objTrasferimento.DataLettura) AndAlso dataUltimaModifica > objTrasferimento.DataLettura Then
                '    msgError &= UtilityHelper.GetMessaggioDataModifica(dataUltimaModifica, usernameUltimaModifica, _objParametriUtenti)
                '    xRisp = False
                'Else

                'End If

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return New Trasferimento_Output With {
            .Risultato = xRisp,
            .MsgError = msgError
        }
        '.TestataSalvata = testataSalvata,
        '.Piva = piva,
        '.IdAgenda = idAgenda,
        '.DescrizioneAgenda = desLib,
        '.SaCodPrincipale = 0,
        '.IdMovS = idMovS,
        '.IdMovC = idMovC,
        '.IdMovDetS = idMovDetScarico,
        '.IdMovDetC = idMovDetCarico
    End Function

    Public Function CancellaRigaTrasferimento(ByVal objTrasferimento As Trasferimento,
                                              ByVal messaggioDettagliato As Boolean,
                                              ByRef msgError As String
                                              ) As Trasferimento_Output

        Const nomeRoutine = "TrasferimentoHelper.CancellaRigaTrasferimento()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim flagOggettoCorretto As Boolean
        Dim objMovDetHelper As New Agenda_Movimenti_Dettagli_Helper

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            'TODO: VERY IMPORTANT!!!! verificare che nessuna della chiavi sia 0, altrimenti rischio di cancellare molto più del dovuto

            flagOggettoCorretto = CheckPropertyCaricoScarico(objTrasferimento, enum_TipoOperazioneDB.Cancellazione)

            If flagOggettoCorretto Then

                Dim CheckGiacenze = UtilityHelper.SeCheckGiacenze(CInt(objTrasferimento.ElemCod), _objParametriUtenti)

                Dim risGiacenzaCheck As String = ""

                If CheckGiacenze.eseguiCheckGiacenze Then

                    'TODO: prima di cancellare devo fare un controllo di giacenza sul magazzino di carico per assicurarmi
                    ' che venendo meno questo carico di prodotto, le eventuali operazioni successive abbiano ancora la giacenza disponibile
                    'controllo giacenza per intera qta movimentata sul magazzino di carico in data 31/12/2100

                    risGiacenzaCheck = ControllaGiacenza(objTrasferimento,
                                                         Nothing,
                                                         objTrasferimento.MagazzinoCarico,
                                                         AGRODATAFINE,
                                                         CDec(objTrasferimento.QtaExtraTotale),
                                                         messaggioDettagliato,
                                                         CheckGiacenze.flag_QtaNoZero,
                                                         CheckGiacenze.flag_QtaMaggioreZero)

                    If risGiacenzaCheck <> "" Then
                        msgError &= "Impossibile cancellare: Il prodotto nel magazzino di destinazione viene già utilizzato. " & risGiacenzaCheck
                        xRisp = False
                    Else
                        xRisp = True
                    End If
                Else
                    xRisp = True
                End If

                If xRisp = True Then

                    'Posso cancellare ==> devo cancellare 2 Dettagli, 2 Destinazioni + riferimenti

                    'TODO: visto che il controllo di giacenza che mi serviva l'ho già fatto qui sopra, posso passare alla cancellazione diretta dei due dettagli,
                    '   senza passare dall'Operazione_Agenda_Utility

                    'Il riferimento verrà cancellato in automatico nel momento in cui si cancella il dettaglio che ho lo stesso Id_Mov_Det (quello di scarico)

                    'Cancello SCARICO
                    xRisp = objMovDetHelper.Cancella(objTrasferimento.Piva,
                                                     0,
                                                     CInt(objTrasferimento.IdAgenda),
                                                     CInt(objTrasferimento.IdMov),
                                                     CInt(objTrasferimento.IdMovDet),
                                                     _objParametriServer)

                    If xRisp = True Then

                        'Cancello CARICO
                        xRisp = objMovDetHelper.Cancella(objTrasferimento.Piva,
                                                         0,
                                                         CInt(objTrasferimento.IdAgenda),
                                                         objTrasferimento.MovDettaglioRif.Id_Mov_Rif,
                                                         objTrasferimento.MovDettaglioRif.Id_Mov_Det_Rif,
                                                         _objParametriServer)
                    End If


                End If

                If xRisp = True Then

                    'Non ho scritto la testata, ma avendo cmq modificato l'operazione devo loggare a parte
                    '(loggo come modifica perché, anche se ho cancellato la riga, considerando l'operazione nella sua interezza si tratta di una modifica)!!!
                    LoggaModificaTrasferimento(objTrasferimento, "", enum_TipoOperazioneDB.Modifica)

                    If UtilityHelper.IsTrasformatoFF(objTrasferimento.ModuloGias, CInt(objTrasferimento.ElemCod)) Then

                        UtilityHelper.SistemaImballaggiDocumento(objTrasferimento.Piva, CInt(objTrasferimento.IdAgenda), CInt(objTrasferimento.IdMov),
                                                                 enum_TipoOperazioneDB.Cancellazione, _objParametriServer,
                                                                 objTrasferimento.ModuloGias, Nothing)

                        UtilityHelper.SistemaImballaggiDocumento(objTrasferimento.Piva, CInt(objTrasferimento.IdAgenda), objTrasferimento.MovDettaglioRif.Id_Mov_Rif,
                                                                 enum_TipoOperazioneDB.Cancellazione, _objParametriServer,
                                                                 objTrasferimento.ModuloGias, Nothing)

                    End If


                    'TODO: se ho cancellato l'ultima riga, devo cancellare anche Agenda + Movimenti ==> lo faccio qui o controllo da interfaccia
                    '   e cambio la funzione chiamata?!?


                End If

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return New Trasferimento_Output With {
            .Risultato = xRisp,
            .MsgError = msgError
        }

    End Function

    Public Function CancellaDocumentoTrasferimento(ByVal piva As String,
                                                   ByVal idAgenda As Integer,
                                                   ByVal messaggioDettagliato As Boolean,
                                                   ByRef msgError As String
                                                   ) As Trasferimento_Output

        Const nomeRoutine = "TrasferimentoHelper.CancellaDocumentoTrasferimento()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim objAgendaHelper As New Agenda_Operazione_Helper

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)


            'Voglio cancellare l'intero documento:
            '- Ciclo per verificare la giacenza (se lo devo fare) per ogni riga di carico
            '- Alla prima riga che mi finirebbe sotto giacenza, interrompo tutto, tanto non posso cancellare
            '- Se tutto va bene, allora procedo con la cancellazione (usando l'AgronicaCoreModello, tanto ho già verificato la giacenza)

            'Questa funzione verrà chiamata dalla pagina di Ricerca dei Trasferimenti, quindi al termine non devo fare nient'altro


            'TODO: per sapere se devo verificare o meno la giacenza ho bisogno dell'Elem_Cod (che potrebbe cambiare di riga in riga)
            ' ==> se vogliamo semplificare, visto che per il momento gestisce solo i Trasformati_Vegetali, potrei verificare fisso in testata il 210,
            '       ma teoricamente dovrei verificarlo su ogni riga in base agli Elem_cod, quindi vuol dire ceh cmq mi dovrei fare il giro di tutti i dettagli a prescindere

            Dim CheckGiacenze = UtilityHelper.SeCheckGiacenze(TRASFORMATI_VEGETALI, _objParametriUtenti)

            Dim risGiacenzaCheck As String = ""

            If CheckGiacenze.eseguiCheckGiacenze Then

                Dim movCarico As Movimento = UtilityHelper.CercaMovimentoSpecifico(CAU_CARICO, piva, idAgenda, _objParametriServer)

                If movCarico Is Nothing Then
                    Throw New Exception("Errore: non è presente il movimento di Carico")
                End If

                If movCarico Is Nothing OrElse movCarico.Movimenti_Dettagli Is Nothing OrElse movCarico.Movimenti_Dettagli.Count = 0 Then
                    Throw New Exception(String.Format("Non sono presenti Movimenti_Dettagli dentro al Movimento [{0}]", movCarico.Id_Mov))
                End If

                For Each movDettaglio In movCarico.Movimenti_Dettagli

                    If movDettaglio.Movimenti_Destinazioni Is Nothing OrElse movDettaglio.Movimenti_Destinazioni.Count <> 1 Then
                        Throw New Exception(String.Format("Non sono presenti Movimenti_Destinazioni dentro al Movimento_Dettaglio [{0}]", movDettaglio.Id_Mov_Det))
                    End If


                    Dim movDest As Movimento_Destinazione = movDettaglio.Movimenti_Destinazioni(0)

                    Dim magazzino As New Contabilita_Magazzino With {
                        .TipoDestinazione = movDest.Tipo,
                        .SaCod = movDest.Sa_Cod,
                        .IdDestinazione = movDest.Id_Destinazione
                    }

                    risGiacenzaCheck = ControllaGiacenza(Nothing,
                                                         movDettaglio,
                                                         magazzino,
                                                         AGRODATAFINE,
                                                         movDettaglio.Qta_Extra_Totale,
                                                         messaggioDettagliato,
                                                         CheckGiacenze.flag_QtaNoZero,
                                                         CheckGiacenze.flag_QtaMaggioreZero)

                    If risGiacenzaCheck <> "" Then
                        msgError &= "Impossibile cancellare: Il prodotto nel magazzino di destinazione viene già utilizzato. " & risGiacenzaCheck
                        xRisp = False
                        Exit For
                    Else
                        xRisp = True
                    End If

                Next

            Else
                'Se non devo verificare la giacenza procedo direttamente alla cancellazione
                xRisp = True
            End If

            If xRisp = True Then

                xRisp = objAgendaHelper.Cancella(piva, 0, idAgenda, False, _objParametriServer)

                'Il log viene già fatto internamente dalla funzione di cancellazione

                'i carichi e gli scarichi relativi agli imballi, visto che sono dentro alla stessa Agenda vengono anch'essi cancellati in automatico

            End If


            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return New Trasferimento_Output With {
            .Risultato = xRisp,
            .MsgError = msgError
        }

    End Function

#End Region

#Region "Generazione Oggetti"

    Private Function GeneraTestataTrasferimento(ByVal dataMovimento As Date,
                                                ByVal piva As String,
                                                ByVal saCod As Integer,
                                                ByVal descrizioneAggiuntiva As String
                                                ) As Operazione_Agenda

        Const nomeRoutine = "TrasferimentoHelper.GeneraTestataTrasferimento()"
        Dim objAgenda As Operazione_Agenda
        Dim objSequenze As New Agro_Sequenze

        Try

            'TODO: sa_cod che cavolo metto?

            '------------------------------------------------
            '----- AGENDA
            '------------------------------------------------
            objAgenda = New Operazione_Agenda With {
                .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
                .Data = dataMovimento,
                .Piva = piva,
                .Sa_Cod = 0,
                .Lav_Cod = LAVCOD_TRASFERIMENTO,
                .Des_Lib = DESLIB_DEFAULT_TRASFERIMENTO
            }
            Dim idAgenda As Integer = objSequenze.NuovoId_Tabella("Agenda", objAgenda.BaseCode, objAgenda.TopCode, _objParametriServer)
            objAgenda.Id_Agenda = idAgenda
            '.Des_Lib = CreaDesLibLavorazione(tipoLavorazioneDes, matDes, lotto, descrizioneAggiuntiva)

            '------------------------------------------------
            '----- MOVIMENTI
            '------------------------------------------------
            'MOVIMENTO DI SCARICO
            Dim objMovimentoS = New Movimento With {
                .Piva = objAgenda.Piva,
                .Sa_Cod = 0,
                .Id_Agenda = objAgenda.Id_Agenda,
                .Data = dataMovimento,
                .Lav_Cod = LAVCOD_TRASFERIMENTO,
                .Cau_Mov = CAU_SCARICO,
                .Mov_Desc = "Scarico da Trasferimento di Magazzino",
                .Scadenza_Extra = AGRODATAINIZIO,
                .Scadenza = AGRODATAFINE,
                .Ora = dataMovimento,
                .Extra_Date = #12/30/1899#,
                .Data_Registrazione = dataMovimento
            }
            Dim idMovS As Integer = objSequenze.NuovoId_Tabella("Movimenti", objMovimentoS.BaseCode, objMovimentoS.TopCode, _objParametriServer)
            objMovimentoS.Id_Mov = idMovS

            'MOVIMENTO DI CARICO
            Dim objMovimentoC = New Movimento With {
                .Piva = objAgenda.Piva,
                .Sa_Cod = 0,
                .Id_Agenda = objAgenda.Id_Agenda,
                .Data = dataMovimento,
                .Lav_Cod = LAVCOD_TRASFERIMENTO,
                .Cau_Mov = CAU_CARICO,
                .Mov_Desc = "Carico da Trasferimento di Magazzino",
                .Scadenza_Extra = AGRODATAINIZIO,
                .Scadenza = AGRODATAFINE,
                .Ora = dataMovimento,
                .Extra_Date = #12/30/1899#,
                .Data_Registrazione = dataMovimento
            }
            Dim idMovC As Integer = objSequenze.NuovoId_Tabella("Movimenti", objMovimentoC.BaseCode, objMovimentoC.TopCode, _objParametriServer)
            objMovimentoC.Id_Mov = idMovC

            'Aggancio i movimenti sull'agenda
            objAgenda.Movimenti = New List(Of Movimento) From {
                objMovimentoC,
                objMovimentoS
            }

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return objAgenda

    End Function

    Private Function GeneraDettaglioTrasferimento(ByVal cauMov As String,
                                                  ByVal piva As String,
                                                  ByVal saCod As Integer,
                                                  ByVal idAgenda As Integer,
                                                  ByVal idMov As Integer,
                                                  ByVal objTrasferimento As Trasferimento
                                                  ) As Movimento_Dettaglio

        Const nomeRoutine = "TrasferimentoHelper.GeneraDettaglioTrasferimento()"
        Dim objMovDettaglio As Movimento_Dettaglio = Nothing
        Dim objDestinazione As Movimento_Destinazione = Nothing
        Dim Progressivo As Integer = 0
        Try

            'TODO: dò per scontato che qualcuno abbia già scritto agenda + mov carico + mov scarico (poi lo dovrò integrare qui dentro)

            If idMov <> 0 Then

                'TODO: SCARICO!!!
                'TODO: creo Mov_Dettaglio, Mov_Dettaglio_Tecnico_Extra e Mov_Destinazione


                'TODO: CARICO!!!
                'TODO: creo Mov_Dettaglio, Mov_Dettaglio_Tecnico_Extra e Mov_Destinazione

                'TODO: devo per forza prima scrivere scarico e carico, perché devo sapere quali sono i loro id_mov_det

                'TODO: ora che so gli id_mov_det posso scrivere la mov_dettagli_riferimenti che me li collega


                Dim magazzino As New Contabilita_Magazzino

                Select Case cauMov
                    Case CAU_SCARICO
                        magazzino = objTrasferimento.MagazzinoScarico
                    Case CAU_CARICO
                        magazzino = objTrasferimento.MagazzinoCarico
                    Case Else
                        Throw New Exception(String.Format("Il Cau_Mov [{0}] non è gestito correttamente per ricavare il magazzino.", cauMov))
                End Select

                If magazzino Is Nothing OrElse magazzino.IdDestinazione Is Nothing OrElse magazzino.IdDestinazione = 0 Then
                    Throw New Exception("Magazzino non valorizzato")
                End If




                'se non mi è arrivato il mat_des, lo vado a cercare
                Dim matDes As String = ""
                If objTrasferimento.MatCod <> 0 AndAlso objTrasferimento.ElemCod <> 0 AndAlso objTrasferimento.MatDes = "" Then
                    matDes = MatDesFromMatCod(piva, CInt(objTrasferimento.ElemCod), CInt(objTrasferimento.MatCod))
                Else
                    matDes = objTrasferimento.MatDes
                End If

                '===========================================================================================================
                'Gestione ETD in base alla distanza di trasporto
                '-----------------------------------------------------------------------------------------------------------
                Progressivo = CInt(objTrasferimento.CalCod) 'Inizializzazione

                If cauMov = CAU_CARICO And objTrasferimento.CalCod <> 0 Then

                    '1. Ricavo un nuovo cal_cod
                    Dim objSequenze As Object = New AgronicaCoreDataProvider.Agro_Sequenze

                    Progressivo = objSequenze.NuovoId_Tabella("Materie_Prime_Campionature",
                                                              CInt(0),
                                                              CInt(2000000000),
                                                              _objParametriServer)

                    Progressivo = Progressivo * (-1)

                    '2. Lettura dello stato dell'indirizzo del centro aziendale della cella di destinazione
                    Dim ObjCentrixIndirizzi As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
                    Dim codiceStato As String = ""
                    Dim DTIndirizzo As DataTable = ObjCentrixIndirizzi.Leggi_dato_centro(piva, magazzino.SaCod, "stato <> ''", "", _objParametriServer)

                    If DTIndirizzo.Rows.Count > 0 Then
                        codiceStato = DTIndirizzo(0)("stato")
                    End If


                    '3. Calcolo ETD per la nuova distanza
                    Dim leggiGHG As New AgronicaCoreContabDAL.GHG_Registrazioni_R

                    Dim Standard_Factor As Decimal = leggiGHG.CalcoloStandard_Factor(objTrasferimento.Distanza_Trasporto,
                                                                                     0,
                                                                                     objTrasferimento.Distanza_Trasporto_UDM,
                                                                                     1,
                                                                                     codiceStato,
                                                                                     "",
                                                                                     _objParametriServer)

                    '4. Lettura dei parametri della tabella materie_prime_campionature
                    Dim Leggi_MPC As New AgronicaCoreContabDAL.Materie_Prime_Campionature_R
                    Dim DT_MPC As DataTable

                    DT_MPC = Leggi_MPC.Leggi(objTrasferimento.CalCod, "", 0, 0, 0, "", True, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", _objParametriServer)

                    '5. Riscrittura ed Aggiunta dei nuovi valori ETD a quelli già presente
                    If DT_MPC.Rows.Count > 0 Then

                        Dim Scrivi_MPC As New AgronicaCoreContabDAL.Materie_Prime_Campion_W
                        Dim oghgforetd As Decimal = 0
                        Dim okmdistance As Decimal = 0
                        Dim Val_Cod As String
                        Dim Delta As Decimal = 1

                        For Each dr_MPC In DT_MPC.Rows

                            Select Case LCase(dr_MPC("Tipo"))

                                Case "oghgforetd"

                                    oghgforetd = 0
                                    If IsNumeric(dr_MPC("Val_Cod")) Then
                                        oghgforetd = Replace(dr_MPC("Val_Cod"), ".", ",")
                                    End If
                                    'Aggiungo l'etd a quello già presente in scarico
                                    oghgforetd = oghgforetd + Standard_Factor

                                    Val_Cod = oghgforetd

                                Case "okmdistance"

                                    okmdistance = 0
                                    If IsNumeric(dr_MPC("Val_Cod")) Then
                                        okmdistance = Replace(dr_MPC("Val_Cod"), ".", ",")
                                    End If

                                    If objTrasferimento.Distanza_Trasporto_UDM = enum_UnitaMisura.Miglia Then
                                        'Conversione in KM
                                        Delta = 1.60934
                                    End If

                                    'Aggiungo l'etd a quello già presente in scarico
                                    okmdistance = okmdistance + (objTrasferimento.Distanza_Trasporto * Delta)

                                    Val_Cod = okmdistance

                                Case Else

                                    Val_Cod = dr_MPC("Val_Cod")

                            End Select

                            If IsNumeric(Val_Cod) Then
                                Val_Cod = Replace(Val_Cod, ",", ".")
                            End If

                            Scrivi_MPC.Scrivi(Progressivo, dr_MPC("Tipo"), dr_MPC("Tipo_Cod"), dr_MPC("Udm_Cod"), Val_Cod,
                                             dr_MPC("Descrizione"), dr_MPC("Peso_Campione"), dr_MPC("Progressivo_Origine"), dr_MPC("Piva_SuperUser_Origine"),
                                             dr_MPC("Validita_Inizio"), dr_MPC("Validita_Fine"), _objParametriServer, dr_MPC("ChkStima"), dr_MPC("ChkTara_Campionatura"),
                                             dr_MPC("Tara_Campionatura"), dr_MPC("Data_Creazione"), dr_MPC("Data_Modifica"), _objParametriServer.UtenteUsername, _objParametriServer.UtenteUsername)

                        Next



                    End If

                End If




                'creo l'oggetto movimento dettaglio e tutti i suoi figli
                objMovDettaglio = New Movimento_Dettaglio With {
                    .Piva = piva,
                    .Sa_Cod = If(magazzino.SaCod, 0),
                    .Cau_Mov = cauMov,
                    .Id_Agenda = idAgenda,
                    .Id_Mov = idMov,
                    .Id_Mov_Det = 0,
                    .Elem_Cod = CInt(objTrasferimento.ElemCod),
                    .Pro_Cod = 0,
                    .Mat_Cod = CInt(objTrasferimento.MatCod),
                    .Mov_Det_Des = If(cauMov = CAU_SCARICO, "Scarico ", "Carico ") & matDes,
                    .Udm_Cod = CInt(objTrasferimento.UdmCod),
                    .Qta = CDec(objTrasferimento.Qta),
                    .Contabilizzato = NONCONTABILE,
                    .Pendente = enum_Pendenza.Trasferimento,
                    .Cal_Cod = Progressivo,
                    .Extra_Date = #12/30/1899#,
                    .Lotto = objTrasferimento.Lotto,
                    .Jolly_Int = CInt(objTrasferimento.JollyInt),
                    .Udm_Cod_Extra = If(objTrasferimento.UdmCodExtra, enum_UnitaMisura.KG),
                    .Qta_Extra = Agro_Math.ArrotondaVal(CDec(objTrasferimento.QtaExtra), objTrasferimento.CifreArrotondamento),
                    .Qta_Extra_Totale = CDec(objTrasferimento.QtaExtraTotale),
                    .Tara = Agro_Math.ArrotondaVal_3(CDec(objTrasferimento.Tara)),
                    .Qta_Dettaglio1 = CDec(objTrasferimento.NumContenitori),
                    .Qta_Dettaglio2 = CDec(objTrasferimento.NumImballaggi),
                    .Validita_Inizio = CDate(objTrasferimento.DataMovimento),
                    .Validita_Fine = AGRODATAFINE
                }
                'TODO: Extra_Str = "1": Gestione Riduzione ad Udm Extra (più eventuali nuovi parametri che non possono essere gestiti altrimenti)???
                '.Extra_Str = "1",
                'TODO: Extra_Int = 1: Gestione Chain (Collegamento tra scarico e relativo carico di residuo)????
                '.Extra_Int = 1,

                'devo creare la destinazione
                objDestinazione = New Movimento_Destinazione With {
                .Piva = piva,
                .Sa_Cod = If(magazzino.SaCod, 0),
                .Id_Agenda = idAgenda,
                .Id_Mov = idMov,
                .Id_Mov_Det = objMovDettaglio.Id_Mov_Det,
                .Id_Destinazione = If(magazzino.IdDestinazione, 0),
                .Tipo = If(magazzino.TipoDestinazione, 0),
                .Qta = CDec(objTrasferimento.Qta),
                .Qta_Dest1 = CDec(objTrasferimento.NumContenitori),
                .Qta_Dest2 = CDec(objTrasferimento.NumImballaggi),
                .Data = CDate(objTrasferimento.DataMovimento)
            }

                objMovDettaglio.Movimenti_Destinazioni = New List(Of Movimento_Destinazione) From {
                objDestinazione
            }


                'Devo scrivere anche Mov_dettaglio_tecnico_extra che contiene i mat_cod degli imballi
                If UtilityHelper.IsTrasformatoFF(objTrasferimento.ModuloGias, CInt(objTrasferimento.ElemCod)) Then

                    Dim dettaglioConfezionamento As New Contabilita_Riga_Confezionamento
                    Dim matCodContenitore As Integer = 0
                    Dim matCodImballo As Integer = 0

                    If objTrasferimento.Confezionamenti IsNot Nothing Then
                        dettaglioConfezionamento = objTrasferimento.Confezionamenti
                        matCodContenitore = UtilityHelper.GetMatCodContenitore(dettaglioConfezionamento, piva, _objParametriServer)
                        matCodImballo = UtilityHelper.GetMatCodImballo(dettaglioConfezionamento, piva, _objParametriServer)
                    End If

                    Dim newIdRegDettaglio As Integer = 0
                    'If Not movDetPrecedente Is Nothing AndAlso
                    '   Not movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra Is Nothing AndAlso
                    '   movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra.Count = 1 Then
                    '    newIdRegDettaglio = movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra(0).Id_Reg_Dettaglio
                    'End If

                    Dim objMovTecExtraD As New Movimento_Dettaglio_Tecnico_Extra With {
                    .Piva = objMovDettaglio.Piva,
                    .Sa_Cod = objMovDettaglio.Sa_Cod,
                    .Id_Agenda = objMovDettaglio.Id_Agenda,
                    .Id_Mov = objMovDettaglio.Id_Mov,
                    .Id_Mov_Det = objMovDettaglio.Id_Mov_Det,
                    .Id_Reg_Dettaglio = newIdRegDettaglio,
                    .Validita_Inizio = objMovDettaglio.Data,
                    .Unita_Trasporto = CInt(objTrasferimento.ModuloGias),
                    .Num_Contenitori = CInt(If(objTrasferimento.NumImballaggi, 0)),
                    .Imballaggio_Cod = matCodImballo,
                    .Marche_Contenitori = If(dettaglioConfezionamento.FF_imballaggio_Descrizione, ""),
                    .Num_Colli = CInt(If(objTrasferimento.NumContenitori, 0)),
                    .Contenitore_Cod = matCodContenitore,
                    .Des_Contenitori = If(dettaglioConfezionamento.FF_contenitore_Descrizione, ""),
                    .DistanzaTrasportoUdm = objTrasferimento.Distanza_Trasporto_UDM,
                    .DistanzaTrasporto = objTrasferimento.Distanza_Trasporto
                }

                    'If Not movDetPrecedente Is Nothing AndAlso
                    '   Not movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra Is Nothing AndAlso
                    '   movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra.Count > 0 Then
                    '    objMovTecExtraD.Data_Creazione = movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra(0).Data_Creazione
                    '    objMovTecExtraD.Username_Creazione = movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra(0).Username_Creazione
                    'Else
                    '    objMovTecExtraD.Username_Creazione = usernameOperatore
                    'End If

                    'Aggancio il movimento dettaglio tecnico extra sul movimento dettaglio
                    objMovDettaglio.Movimenti_Dettagli_Tecnici_Extra = New List(Of Movimento_Dettaglio_Tecnico_Extra) From {
                    objMovTecExtraD
                }

                End If


            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return objMovDettaglio

    End Function

    Private Function ModificaQtaScarico(ByVal objTrasferimento As Trasferimento,
                                        ByVal dataOp As Date
                                        ) As Boolean

        Const nomeRoutine = "TrasferimentoHelper.ModificaQtaScarico()"
        Dim xRisp As Boolean = False

        Dim movDetHelper As New Agenda_Movimenti_Dettagli_Helper
        Dim movDestHelper As New Agenda_Movimenti_Destinazioni_Helper
        Dim movTecnicoExtraHelper As New AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_W
        Dim movDetRifW As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W

        Try

            'TODO: in scarico posso modificare solo la qta, quindi vado in update puntuale solo di quelle

            'Modifica puntuale Mov dettaglio scarico
            xRisp = movDetHelper.ModificaPuntuale(objTrasferimento.Piva,
                                                  CInt(objTrasferimento.MagazzinoScarico.SaCod),
                                                  CInt(objTrasferimento.IdAgenda),
                                                  CInt(objTrasferimento.IdMov),
                                                  CInt(objTrasferimento.IdMovDet),
                                                  _objParametriServer,
                                                  qta:=objTrasferimento.Qta,
                                                  qtaExtra:=Agro_Math.ArrotondaVal_Nothing(objTrasferimento.QtaExtra, objTrasferimento.CifreArrotondamento),
                                                  qtaExtraTotale:=objTrasferimento.QtaExtraTotale,
                                                  tara:=objTrasferimento.Tara,
                                                  qtaDettaglio1:=objTrasferimento.NumContenitori,
                                                  qtaDettaglio2:=objTrasferimento.NumImballaggi)

            If xRisp = True Then

                'modifica puntuale Mov destinazione scarico
                xRisp = movDestHelper.ModificaPuntuale(objTrasferimento.Piva,
                                                       CInt(objTrasferimento.MagazzinoScarico.SaCod),
                                                       CInt(objTrasferimento.IdAgenda),
                                                       CInt(objTrasferimento.IdMov),
                                                       CInt(objTrasferimento.IdMovDet),
                                                       appezza:=0,
                                                       tipoDestinazione:=objTrasferimento.MagazzinoScarico.TipoDestinazione,
                                                       idDestinazione:=CInt(objTrasferimento.MagazzinoScarico.IdDestinazione),
                                                       objParametri:=_objParametriServer,
                                                       qta:=objTrasferimento.Qta,
                                                       qtaDest1:=objTrasferimento.NumContenitori,
                                                       qtaDest2:=objTrasferimento.NumImballaggi)

                If xRisp = True AndAlso UtilityHelper.IsTrasformatoFF(objTrasferimento.ModuloGias, CInt(objTrasferimento.ElemCod)) Then

                    xRisp = movTecnicoExtraHelper.ModificaPuntuale(objTrasferimento.Piva,
                                                                   CInt(objTrasferimento.MagazzinoScarico.SaCod),
                                                                   CInt(objTrasferimento.IdAgenda),
                                                                   CInt(objTrasferimento.IdMov),
                                                                   CInt(objTrasferimento.IdMovDet),
                                                                   0,
                                                                   _objParametriServer,
                                                                   Num_Contenitori:=CType(objTrasferimento.NumImballaggi, Integer?),
                                                                   Num_Colli:=CType(objTrasferimento.NumContenitori, Integer?))

                End If

            End If

            If xRisp = True Then

                'Modifica puntuale Mov dettaglio Carico
                xRisp = movDetHelper.ModificaPuntuale(objTrasferimento.Piva,
                                                      CInt(objTrasferimento.MagazzinoCarico.SaCod),
                                                      CInt(objTrasferimento.IdAgenda),
                                                      objTrasferimento.MovDettaglioRif.Id_Mov_Rif,
                                                      objTrasferimento.MovDettaglioRif.Id_Mov_Det_Rif,
                                                      _objParametriServer,
                                                      qta:=objTrasferimento.Qta,
                                                      qtaExtra:=Agro_Math.ArrotondaVal_Nothing(objTrasferimento.QtaExtra, objTrasferimento.CifreArrotondamento),
                                                      qtaExtraTotale:=objTrasferimento.QtaExtraTotale,
                                                      tara:=objTrasferimento.Tara,
                                                      qtaDettaglio1:=objTrasferimento.NumContenitori,
                                                      qtaDettaglio2:=objTrasferimento.NumImballaggi)

                If xRisp = True Then

                    'Modifica puntuale Mov destinazione Carico
                    xRisp = movDestHelper.ModificaPuntuale(objTrasferimento.Piva,
                                                           CInt(objTrasferimento.MagazzinoCarico.SaCod),
                                                           CInt(objTrasferimento.IdAgenda),
                                                           objTrasferimento.MovDettaglioRif.Id_Mov_Rif,
                                                           objTrasferimento.MovDettaglioRif.Id_Mov_Det_Rif,
                                                           appezza:=0,
                                                           tipoDestinazione:=objTrasferimento.MagazzinoCarico.TipoDestinazione,
                                                           idDestinazione:=CInt(objTrasferimento.MagazzinoCarico.IdDestinazione),
                                                           objParametri:=_objParametriServer,
                                                           qta:=objTrasferimento.Qta,
                                                           qtaDest1:=objTrasferimento.NumContenitori,
                                                           qtaDest2:=objTrasferimento.NumImballaggi)

                    If xRisp = True AndAlso UtilityHelper.IsTrasformatoFF(objTrasferimento.ModuloGias, CInt(objTrasferimento.ElemCod)) Then

                        xRisp = movTecnicoExtraHelper.ModificaPuntuale(objTrasferimento.Piva,
                                                                       CInt(objTrasferimento.MagazzinoCarico.SaCod),
                                                                       CInt(objTrasferimento.IdAgenda),
                                                                       objTrasferimento.MovDettaglioRif.Id_Mov_Rif,
                                                                       objTrasferimento.MovDettaglioRif.Id_Mov_Det_Rif,
                                                                       0,
                                                                       _objParametriServer,
                                                                       Num_Contenitori:=CType(objTrasferimento.NumImballaggi, Integer?),
                                                                       Num_Colli:=CType(objTrasferimento.NumContenitori, Integer?))

                    End If


                End If
            End If

            If xRisp = True Then
                'modifica puntuale qta di mov dettagli riferimenti
                xRisp = movDetRifW.ModificaPuntuale(objTrasferimento.Piva, 0, CInt(objTrasferimento.IdAgenda),
                                                    CInt(objTrasferimento.IdMov), CInt(objTrasferimento.IdMovDet),
                                                    CInt(objTrasferimento.IdAgenda),
                                                    objTrasferimento.MovDettaglioRif.Id_Mov_Rif,
                                                    objTrasferimento.MovDettaglioRif.Id_Mov_Det_Rif,
                                                    _objParametriServer,
                                                    Qta:=objTrasferimento.Qta)
            End If

            If xRisp = True Then
                'Devo anche aggiornare data modifica dell'agenda e scrivere Agronica_Log_Agenda
                LoggaModificaTrasferimento(objTrasferimento, "", enum_TipoOperazioneDB.Modifica)
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return xRisp

    End Function

#End Region

#Region "Verifiche preliminari funzioni"

    Private Function CheckPropertyCaricoScarico(ByRef objTrasferimento As Trasferimento,
                                                ByVal tipoOperazione As enum_TipoOperazioneDB
                                                ) As Boolean

        Const nomeRoutine = "TrasferimentoHelper.CheckPropertyCaricoScarico()"
        Dim stringaErrore As String = ""

        Try

            UtilityHelper.ControllaStringEmpty(objTrasferimento, Nothing, stringaErrore, "Piva")

            If tipoOperazione = enum_TipoOperazioneDB.Cancellazione OrElse
               tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                UtilityHelper.ControllaNumZero(objTrasferimento, Nothing, stringaErrore, "IdAgenda")
                UtilityHelper.ControllaNumZero(objTrasferimento, Nothing, stringaErrore, "IdMov")
                UtilityHelper.ControllaNumZero(objTrasferimento, Nothing, stringaErrore, "IdMovDet")

                UtilityHelper.ControllaPresenza(objTrasferimento, 0, stringaErrore, "SaCod")

                'Verifico i riferimenti
                UtilityHelper.ControllaPresenza(objTrasferimento, Nothing, stringaErrore, "MovDettaglioRif")
                UtilityHelper.ControllaNumZero(objTrasferimento.MovDettaglioRif, Nothing, stringaErrore, "Id_Mov_Rif")
                UtilityHelper.ControllaNumZero(objTrasferimento.MovDettaglioRif, Nothing, stringaErrore, "Id_Mov_Det_Rif")

            End If

            If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                UtilityHelper.ControllaPresenza(objTrasferimento, Nothing, stringaErrore, "DataLettura")

                UtilityHelper.ControllaPresenza(objTrasferimento, Nothing, stringaErrore, "IdDestinazione")
                UtilityHelper.ControllaPresenza(objTrasferimento, 0D, stringaErrore, "KgPrecedenti")
            End If

            If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then

                UtilityHelper.ControllaPresenza(objTrasferimento, 0, stringaErrore, "SaCod")
                'UtilityHelper.ControllaPresenza(objTrasferimento, Nothing, stringaErrore, "IdDestinazione")
                'UtilityHelper.ControllaPresenza(objTrasferimento, Nothing, stringaErrore, "TipoDestinazione")

                UtilityHelper.ControllaPresenza(objTrasferimento, "", stringaErrore, "TipoLavorazioneDes")

                UtilityHelper.ControllaNumZero(objTrasferimento, Nothing, stringaErrore, "ElemCod")
                UtilityHelper.ControllaNumZero(objTrasferimento, Nothing, stringaErrore, "MatCod")
                UtilityHelper.ControllaPresenza(objTrasferimento, "", stringaErrore, "MatDes")
                UtilityHelper.ControllaNumZero(objTrasferimento, Nothing, stringaErrore, "UdmCod")
                UtilityHelper.ControllaPresenza(objTrasferimento, 0D, stringaErrore, "Qta")
                UtilityHelper.ControllaPresenza(objTrasferimento, 0, stringaErrore, "CalCod")
                UtilityHelper.ControllaPresenza(objTrasferimento, "", stringaErrore, "Lotto")
                UtilityHelper.ControllaPresenza(objTrasferimento, MagazzinoMovimentato, stringaErrore, "JollyInt")
                UtilityHelper.ControllaPresenza(objTrasferimento, 1D, stringaErrore, "QtaExtra")
                UtilityHelper.ControllaPresenza(objTrasferimento, 0D, stringaErrore, "QtaExtraTotale")
                UtilityHelper.ControllaPresenza(objTrasferimento, 0D, stringaErrore, "Tara")
                UtilityHelper.ControllaPresenza(objTrasferimento, 0D, stringaErrore, "NumContenitori")
                UtilityHelper.ControllaPresenza(objTrasferimento, 0D, stringaErrore, "NumImballaggi")
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

    Private Function MatDesFromMatCod(ByVal piva As String, ByVal elemCod As Integer, ByVal matCod As Integer) As String
        Dim matPrimeR As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Return matPrimeR.MatDes_from_MatCod(piva, elemCod, matCod, "", Nothing, "", _objParametriServer)
    End Function

    Private Function ControllaGiacenza(ByVal objTrasferimento As Trasferimento,
                                       ByVal objMovDet As Movimento_Dettaglio,
                                       ByVal magazzino As Contabilita_Magazzino,
                                       ByVal dataControlloGiacenza As Date,
                                       ByVal qtaKgDaControllare As Decimal,
                                       ByVal messaggioDettagliato As Boolean,
                                       ByVal Flag_QtaNoZero As Boolean,
                                       ByVal Flag_QtaMaggioreZero As Boolean
                                       ) As String

        Const nomeRoutine = "TrasferimentoHelper.ControllaGiacenza()"
        Dim risGiacenzaCheck As String = ""
        Dim objMagazzinoBiz As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ

        Try

            Dim piva As String = ""
            If Not IsNothing(objTrasferimento) AndAlso Not IsNothing(objTrasferimento.Piva) Then
                piva = objTrasferimento.Piva
            ElseIf Not IsNothing(objMovDet) AndAlso Not IsNothing(objMovDet.Piva) Then
                piva = objMovDet.Piva
            End If

            Dim elemCod As Integer = 0
            If Not IsNothing(objTrasferimento) AndAlso Not IsNothing(objTrasferimento.ElemCod) Then
                elemCod = CInt(objTrasferimento.ElemCod)
            ElseIf Not IsNothing(objMovDet) AndAlso Not IsNothing(objMovDet.Elem_Cod) Then
                elemCod = objMovDet.Elem_Cod
            End If

            Dim matCod As Integer = 0
            If Not IsNothing(objTrasferimento) AndAlso Not IsNothing(objTrasferimento.MatCod) Then
                matCod = CInt(objTrasferimento.MatCod)
            ElseIf Not IsNothing(objMovDet) AndAlso Not IsNothing(objMovDet.Mat_Cod) Then
                matCod = objMovDet.Mat_Cod
            End If

            Dim lotto As String = ""
            If Not IsNothing(objTrasferimento) AndAlso Not IsNothing(objTrasferimento.Lotto) Then
                lotto = objTrasferimento.Lotto
            ElseIf Not IsNothing(objMovDet) AndAlso Not IsNothing(objMovDet.Lotto) Then
                lotto = objMovDet.Lotto
            End If

            Dim calCod As Integer = 0
            If Not IsNothing(objTrasferimento) AndAlso Not IsNothing(objTrasferimento.CalCod) Then
                calCod = CInt(objTrasferimento.CalCod)
            ElseIf Not IsNothing(objMovDet) AndAlso Not IsNothing(objMovDet.Cal_Cod) Then
                calCod = objMovDet.Cal_Cod
            End If


            risGiacenzaCheck = objMagazzinoBiz.Controllo_Giacenza(piva,
                                                                  Sa_Cod:=CStr(magazzino.SaCod),
                                                                  strFabbricato_Cod:=CStr(magazzino.IdDestinazione),
                                                                  elemCod:=elemCod,
                                                                  nomeProdotto:="",
                                                                  CodiceProdotto:=CStr(matCod),
                                                                  Lotto:=lotto,
                                                                  Cal_Cod:=calCod,
                                                                  data:=dataControlloGiacenza,
                                                                  cifreArrotondamentoPesi:=3,
                                                                  messaggioDettagliato:=messaggioDettagliato,
                                                                  checkPeso:=True,
                                                                  imballiDaScaricare:=0,
                                                                  contenitoriDaScaricare:=0,
                                                                  confezioniDaScaricare:=0,
                                                                  kgLordiDaScaricare:=0,
                                                                  kgNettiDaScaricare:=qtaKgDaControllare,
                                                                  objParametriServer:=_objParametriServer,
                                                                  objParametriUtenti:=_objParametriUtenti,
                                                                  Flag_QtaNoZero:=Flag_QtaNoZero,
                                                                  Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero)

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return risGiacenzaCheck

    End Function

End Class
