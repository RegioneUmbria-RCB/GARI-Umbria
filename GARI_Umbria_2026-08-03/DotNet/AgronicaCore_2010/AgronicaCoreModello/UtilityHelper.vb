Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModello.My.Resources
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class UtilityHelper

#Region "Data Ultima Modifica"

    Public Shared Function UltimaModificaMovDettaglio(ByVal piva As String,
                                                      ByVal saCod As Integer,
                                                      ByVal idAgenda As Integer,
                                                      ByVal idMov As Integer,
                                                      ByVal idMovDet As Integer,
                                                      ByRef usernameModifica As String,
                                                      ByRef objParametriServer As AgronicaCoreParametri
                                                      ) As DateTime
        Dim movDetR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim dtResult = movDetR.Leggi_UltimaModifica(piva, saCod, idAgenda, idMov, idMovDet, "", usernameModifica, objParametriServer)

        Return dtResult.AddMilliseconds(-dtResult.Millisecond) 'Azzero i millisecondi

    End Function

    Public Shared Function UltimaModificaAgenda(ByVal piva As String,
                                                ByVal idAgenda As Integer,
                                                ByRef usernameModifica As String,
                                                ByRef dataMovimento As Date,
                                                ByRef objParametriServer As AgronicaCoreParametri
                                                ) As DateTime
        Dim agendaR As New AgronicaCoreContabDAL.Agenda_R
        Dim dtResult = agendaR.Leggi_UltimaModifica_DataMovimento(piva, idAgenda, "", usernameModifica, dataMovimento, objParametriServer)

        Return dtResult.AddMilliseconds(-dtResult.Millisecond) 'Azzero i millisecondi

    End Function

    Public Shared Function GetMessaggioDataModifica(ByVal dataUltimaModifica As DateTime,
                                                    ByVal usernameUltimaModifica As String,
                                                    ByRef objParametriUtenti As AgronicaCoreParametri
                                                    ) As String

        Dim nomeUtente As String = GetNomeUtenteFromCF(usernameUltimaModifica, objParametriUtenti)

        Return String.Format("L'operazione che si sta cercando di modificare è già stata modificata da {0} in data {1}.",
                             nomeUtente, dataUltimaModifica.ToString())

    End Function

#End Region

#Region "Verifica presenza/correttezza singole Proprietà"

    ''' <summary>
    ''' Verifica presenza della proprietà dell'oggetto (se assente può impostare un valore default)
    ''' </summary>
    ''' <param name="obj">Oggetto da analizzare</param>
    ''' <param name="defaultValue">Valore da impostare se non presente (Nothing = non imposta default)</param>
    ''' <param name="stringaErrore">Stringa dove viene concatenato l'errore</param>
    ''' <param name="prop">Proprietà da controllare</param>
    Public Shared Sub ControllaPresenza(Of T)(ByRef obj As T, ByVal defaultValue As Object, ByRef stringaErrore As String, ByVal prop As String)
        Dim val = obj.GetType.GetProperty(prop).GetValue(obj, Nothing)
        If IsNothing(val) Then
            If Not IsNothing(defaultValue) Then
                obj.GetType.GetProperty(prop).SetValue(obj, defaultValue, Nothing)
            Else
                Dim objStr As String = obj.GetType.Name
                stringaErrore &= String.Format("La proprietà {0} dell'oggetto {1} è obbligatoria. ", prop, objStr) & vbCrLf
            End If
        End If
    End Sub

    Public Shared Sub ControllaStringEmpty(Of T)(ByRef obj As T, ByVal defaultValue As String, ByRef stringaErrore As String, ByVal prop As String)
        Dim stringa = obj.GetType.GetProperty(prop).GetValue(obj, Nothing)
        If IsNothing(stringa) OrElse String.IsNullOrEmpty(stringa) Then
            If Not IsNothing(defaultValue) Then
                obj.GetType.GetProperty(prop).SetValue(obj, defaultValue, Nothing)
            Else
                Dim objStr As String = obj.GetType.Name
                stringaErrore &= String.Format("La proprietà {0} dell'oggetto {1} è obbligatoria e non può essere vuota. ", prop, objStr) & vbCrLf
            End If
        End If
    End Sub

    Public Shared Sub ControllaNumZero(Of T)(ByRef obj As T, ByVal defaultValue As Decimal?, ByRef stringaErrore As String, ByVal prop As String)
        Dim num = obj.GetType.GetProperty(prop).GetValue(obj, Nothing)
        If IsNothing(num) OrElse num = 0 Then
            If Not IsNothing(defaultValue) Then
                obj.GetType.GetProperty(prop).SetValue(obj, defaultValue, Nothing)
            Else
                Dim objStr As String = obj.GetType.Name
                stringaErrore &= String.Format("La proprietà {0} dell'oggetto {1} è obbligatoria e non può essere zero. ", prop, objStr) & vbCrLf
            End If
        End If
    End Sub

#End Region

#Region "Numero documento"

    Public Shared Function ControllaEsistenzaNumDoc(ByVal lavCod As Integer,
                                                    ByVal piva As String,
                                                    ByVal idAgenda As Integer,
                                                    ByVal docNumeroSin As String,
                                                    ByRef docNumero As Integer,
                                                    ByVal docNumeroDes As String,
                                                    ByVal dataDoc As Date,
                                                    ByVal dataRegistrazione As Date,
                                                    ByVal sezionaleCod As Integer,
                                                    ByVal codRisUm As Integer,
                                                    ByRef objParametriServer As AgronicaCoreParametri,
                                                    ByRef objParametriUtenti As AgronicaCoreParametri,
                                                    ByRef msgRisp As String
                                                    ) As Boolean

        Dim strFiltro As String = ""
        Dim errore As Boolean = False
        Dim flagForzaRicalcolo As Boolean = False
        Dim flagPermettiRicalcolo As Boolean = False
        Dim bOk As Boolean = False

        'TODO: sistema funzione!!!

        'TODO: FormOrdine, FormAccettazione_Fruttagel (sembra non facesse alcun controllo?!?)

        'TODO: nei documenti emessi (+ accettazione anche se carico) facciamo il calcolo automatico e bloccato del numero documento (no sequenza_progressivi)
        'TODO: creare tabella che tenga traccia dei buchi numerazione (ID AutoInc?!?, Piva, Lav_Cod, Data_Doc, Doc_Numero_Sin, Doc_Numero, Doc_Numero_Des, colonne STD)
        'TODO: come parte dell'algoritmo che stacca il nuovo Doc_Numero, devo prima guardare la tabella Buchi_Numerazione, poi se non trovo nulla di utilizzabile,
        ' procedo con ultimo doc + 1

        'TODO: se il documento prevede la sequenza_progressivi, devo scriverlo subito

        'TODO: come mi comporto con i Lunghezza_Sin, Lunghezza_Centro, Lunghezza_Des e CarattereFormattazione di chi usa Sequenza_Progressivi?!?

        Select Case lavCod

            Case LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE

                'TODO: questo sarebbe il caso Accettazione OF!!!
                'Gli altri casi che si possono verificare in FormAccettazione_OF non venivano controllati

                strFiltro = "Agenda.Id_Agenda <> " & idAgenda &
                            " And Agenda.Lav_Cod = " & lavCod &
                            " And Year(Data_Movimento) = " & Year(dataDoc) &
                            " And Doc_Numero = " & CLng(docNumero) &
                            " And Upper(Doc_Numero_Sin) = '" & Agro_SQL_SaveText(Trim(UCase(docNumeroSin))) &
                            "' And Upper(Doc_Numero_Des) = '" & Agro_SQL_SaveText(Trim(UCase(docNumeroDes))) & "'"
                            '" And Year(Data_Registrazione) = " & Year(dataRegistrazione) &

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_CONFERIMENTO

                'TODO: questo sembra che in realtà non venisse mai richiamato?!?

                strFiltro = "Agenda.Id_Agenda <> " & idAgenda &
                            " And Agenda.Lav_Cod In (1025,1050) And Year(Data_Movimento) = " & Year(dataDoc) &
                            " And Doc_Numero = " & CLng(docNumero) &
                            " And Upper(Doc_Numero_Sin) = '" & Agro_SQL_SaveText(Trim(UCase(docNumeroSin))) &
                            "' And Upper(Doc_Numero_Des) = '" & Agro_SQL_SaveText(Trim(UCase(docNumeroDes))) &
                            "' And Sezionale_Cod = " & sezionaleCod &
                            " And Movimenti.Cod_RisUm = " & codRisUm
                            '" And Agenda.Lav_Cod In (1025,1050) And Year(Data_Registrazione) = " & Year(dataRegistrazione) &

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_CONFERIMENTO_DIVERSI, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                strFiltro = "Agenda.Id_Agenda <> " & idAgenda &
                            " And Agenda.Lav_Cod In (1031,1052,1069) And Year(Data_Movimento) = " & Year(dataDoc) &
                            " And Doc_Numero = " & CLng(docNumero) &
                            " And Upper(Doc_Numero_Sin) = '" & Agro_SQL_SaveText(Trim(UCase(docNumeroSin))) &
                            "' And Upper(Doc_Numero_Des) = '" & Agro_SQL_SaveText(Trim(UCase(docNumeroDes))) &
                            "' And Sezionale_Cod = " & sezionaleCod

                flagPermettiRicalcolo = True

            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_FATTURA_PROFESSIONISTI

                strFiltro = "Agenda.Id_Agenda <> " & idAgenda &
                            " And Agenda.Lav_Cod = " & lavCod &
                            " And Year(Data_Movimento) = " & Year(dataDoc) &
                            " And Doc_Numero = " & CLng(docNumero) &
                            " And Upper(Doc_Numero_Sin) = '" & Agro_SQL_SaveText(Trim(UCase(docNumeroSin))) &
                            "' And Upper(Doc_Numero_Des) = '" & Agro_SQL_SaveText(Trim(UCase(docNumeroDes))) &
                            "' And Sezionale_Cod = " & sezionaleCod &
                            " And Movimenti.Cod_RisUm = " & codRisUm

            Case LAVCOD_FATTURA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA,
                LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                LAVCOD_FATTURA_LIQ_CONF_RICEVUTA, LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA,
                LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_DOCO_EMESSO, LAVCOD_DOCO_RICEVUTO,
                LAVCOD_DAA_EMESSO, LAVCOD_MVV_EMESSO, LAVCOD_MVV_RICEVUTO,
                LAVCOD_FATTURA_PROFORMA, LAVCOD_CONTRATTO_AFFITTO

                'tutti gli altri doc gestiti da formFattura (esclusi quelli del punto sopra)
                'TODO: da verificare perché sembra che in questi casi faccia sempre il ricalcolo, anche se alcuni sono doc ricevuti?!?

                strFiltro = "Agenda.Id_Agenda <> " & idAgenda &
                            " And Agenda.Lav_Cod = " & lavCod &
                            " And Year(Data_Movimento) = " & Year(dataDoc) &
                            " And Doc_Numero = " & CInt(docNumero) &
                            " And Upper(Doc_Numero_Sin) = '" & Agro_SQL_SaveText(Trim(UCase(docNumeroSin))) &
                            "' And Upper(Doc_Numero_Des) = '" & Agro_SQL_SaveText(Trim(UCase(docNumeroDes))) &
                            "' And Sezionale_Cod = " & sezionaleCod

                flagPermettiRicalcolo = True
                flagForzaRicalcolo = True

            Case Else

                'Non devo fare controlli?!?
                Return errore
        End Select

        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
        Dim dt As DataTable = objMovimenti.Leggi(piva, 0, 0, 0, 0, CAU_REGISTRAZIONI,
                                                 enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                 strFiltro, "", objParametriServer)

        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

            errore = True

            Dim nomeUtente As String = GetNomeUtenteFromCF(dt.Rows(0).Item("Username_Modifica"), objParametriUtenti)

            Dim msg As String = String.Format("Attenzione. Il documento {0} è già presente in archivio dell'azienda {1}. " & Chr(13) &
                                              "Il documento è stato inserito in data {2} dall'utente {3}. ",
                                              Trim(docNumeroSin) & docNumero & Trim(docNumeroDes), dt.Rows(0).Item("rag_soc"),
                                              CDate(dt.Rows(0).Item("Data_Movimento")).ToShortDateString(), nomeUtente)

            'If flagPermettiRicalcolo = True Then
            'If flagForzaRicalcolo = False AndAlso bNUMERO_DOC_DUPLICATI Then
            '    If cCar.SMS(GiasSoftware,
            '                msg & Chr(13) & Chr(13) & "Si desidera continuare?",
            '                vbYesNo) = vbYes Then
            '        'Di fatto sto permettendo di avere 2 documenti emessi con stesso numero
            '        bOk = True
            '    End If
            'End If

            'If Not bOk Then

            '    Call CmdNumero_Click

            '    cCar.SMS(GiasSoftware,
            '             "Attenzione. Il documento verrà salvato con numero " & Trim(docNumeroSin) & docNumeroNEW & Trim(docNumeroDes) &
            '             " poiché la numerazione impostata non è più disponibile." & Chr(13),
            '             vbInformation)

            'msgRisp = "Attenzione. Il documento verrà salvato con numero " & Trim(docNumeroSin) & "docNumeroNEW" & Trim(docNumeroDes) &
            '         " poiché la numerazione impostata non è più disponibile." & Chr(13)
            '    errore = False
            '    Exit Function

            'End If
            'Else

            'If cCar.SMS("Gias Lan",
            '            msg & Chr(13) & Chr(13) & "Si desidera salvare comunque il documento?.",
            '            vbYesNo) = vbNo Then

            msgRisp = msg '& Chr(13) & Chr(13) & "Si desidera salvare comunque il documento?."
            'errore = False
            'Exit Function

            'End If

            'End If

        End If

        Return errore

    End Function

    Public Shared Function FormattaNumeroDoc(ByVal docNumeroSin As String,
                                             ByVal docNumero As Integer,
                                             ByVal docNumeroLunghezza As Integer?,
                                             ByVal docNumeroCarattereFormattazione As String,
                                             ByVal docNumeroDes As String
                                             ) As String

        Dim lunghezzaNumero As Integer = If(docNumeroLunghezza, 0)
        Dim carattereFormattazione As String = If(docNumeroCarattereFormattazione, "")
        Dim numPadded As String = ""
        If lunghezzaNumero > 0 Then
            numPadded = (CStr(docNumero)).PadLeft(lunghezzaNumero, carattereFormattazione)
        Else
            numPadded = CStr(docNumero)
        End If

        Return docNumeroSin & numPadded & docNumeroDes

    End Function

    Public Shared Function VerificaCoerenzaDateNumDoc(ByVal newNumDoc As Integer,
                                                      ByVal docNumeroSin As String,
                                                      ByVal docNumeroDes As String,
                                                      ByVal docNumeroVisualizzato As String,
                                                      ByVal piva As String,
                                                      ByVal lavCod As Integer,
                                                      ByVal dataMovimento As Date,
                                                      ByRef objParametriServer As AgronicaCoreParametri
                                                      ) As String

        Const nomeRoutine = "UtilityHelper.VerificaCoerenzaDateNumDoc()"
        Dim obj As New AgronicaCoreContabDAL.Contabilita_R
        Dim msgError As String = ""

        Try

            'In caso di ordini, il controllo non è richiesto
            If IsOrdine(lavCod) = False Then

                Dim thisNumDoc As String = docNumeroVisualizzato

                If String.IsNullOrEmpty(thisNumDoc) Then
                    thisNumDoc = docNumeroSin & newNumDoc & docNumeroDes
                End If

                'Precedente
                Dim dtPrecedente As DataTable = obj.DocumentoPrecedenteSuccessivo(enum_TipoDocumento_Numerazione.Precedente,
                                                                                  newNumDoc,
                                                                                  piva,
                                                                                  lavCod,
                                                                                  Year(dataMovimento),
                                                                                  docNumeroSin,
                                                                                  docNumeroDes,
                                                                                  0,
                                                                                  "", "", objParametriServer)

                If dtPrecedente Is Nothing Then
                    Throw New Exception(AgronicaCoreModelloRes.ImpossibileRicavareDocPrecedente)
                ElseIf dtPrecedente.Rows.Count = 0 Then
                    'Non ho documenti precedenti in quell'anno, tutto OK ==> Prosegui
                Else
                    'Ho 1 record, devo verificare se va bene la data
                    Dim numDocPrec As Integer = CInt(dtPrecedente.Rows(0).Item("Doc_Numero"))
                    Dim numDocShowPrec As String = dtPrecedente.Rows(0).Item("Doc_Numero_Visualizzato")
                    Dim dataDocPrec As Date = CDate(dtPrecedente.Rows(0).Item("Data_Movimento"))

                    If String.IsNullOrEmpty(numDocShowPrec) Then
                        numDocShowPrec = docNumeroSin & numDocPrec & docNumeroDes
                    End If

                    If dataMovimento < dataDocPrec Then
                        msgError = String.Format(AgronicaCoreModelloRes.NumDataDocIncoerenteConPrecedente,
                                                 numDocShowPrec, dataDocPrec.ToShortDateString(),
                                                 thisNumDoc, dataMovimento.ToShortDateString(), "<br>")
                    Else
                        'Coerenza tra numeri e date, tutto OK ==> Prosegui
                    End If
                End If

                'Successivo
                Dim dtSuccessivo As DataTable = obj.DocumentoPrecedenteSuccessivo(enum_TipoDocumento_Numerazione.Successivo,
                                                                                  newNumDoc,
                                                                                  piva,
                                                                                  lavCod,
                                                                                  Year(dataMovimento),
                                                                                  docNumeroSin,
                                                                                  docNumeroDes,
                                                                                  0,
                                                                                  "", "", objParametriServer)

                If dtSuccessivo Is Nothing Then
                    Throw New Exception(AgronicaCoreModelloRes.ImpossibileRicavareDocSuccessivo)
                ElseIf dtSuccessivo.Rows.Count = 0 Then
                    'Non ho documenti successivi in quell'anno, tutto OK ==> Prosegui
                Else
                    'Ho 1 record, devo verificare se va bene la data
                    Dim numDocSucc As Integer = CInt(dtSuccessivo.Rows(0).Item("Doc_Numero"))
                    Dim numDocShowSucc As String = dtSuccessivo.Rows(0).Item("Doc_Numero_Visualizzato")
                    Dim dataDocSucc As Date = CDate(dtSuccessivo.Rows(0).Item("Data_Movimento"))

                    If String.IsNullOrEmpty(numDocShowSucc) Then
                        numDocShowSucc = docNumeroSin & numDocSucc & docNumeroDes
                    End If

                    If dataDocSucc < dataMovimento Then
                        msgError = String.Format(AgronicaCoreModelloRes.NumDataDocIncoerenteConSuccessivo,
                                                 numDocShowSucc, dataDocSucc.ToShortDateString(),
                                                 thisNumDoc, dataMovimento.ToShortDateString(), "<br>")
                    Else
                        'Coerenza tra numeri e date, tutto OK ==> Prosegui
                    End If
                End If
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return msgError

    End Function

    Public Shared Function AlgoritmoAssegnazioneNumDoc(ByRef newNumDoc As Integer,
                                                       ByRef newNumDocVisualizzato As String,
                                                       ByVal lavCod As Integer,
                                                       ByVal piva As String,
                                                       ByVal dataMovimento As Date,
                                                       ByVal docNumeroSin As String,
                                                       ByVal originalDocNumero As Integer,
                                                       ByVal docNumeroDes As String,
                                                       ByVal docNumeroLock As Boolean,
                                                       ByVal docNumeroVisualizzato As String,
                                                       ByRef objParametriServer As AgronicaCoreParametri,
                                                       Optional ByVal docNumeroLunghezza As Integer = 0,
                                                       Optional ByVal carattereFormattazione As String = ""
                                                       ) As String

        Const nomeRoutine = "UtilityHelper.AlgoritmoAssegnazioneNumDoc()"
        Dim msgErroreGestito As String = ""

        Try

            'Solo nel caso abbia aperto un documento vecchio, che non aveva salvato il Doc_Numero_Visualizzato, lo aggiorno
            If String.IsNullOrEmpty(docNumeroVisualizzato) AndAlso originalDocNumero <> 0 Then
                docNumeroVisualizzato = FormattaNumeroDoc(docNumeroSin,
                                                          originalDocNumero, 0, "",
                                                          docNumeroDes)
            End If

            Dim isDocEmesso As Boolean = IsNumeroDocumentoEmesso(lavCod)


            'Se mi è arrivato un numero doc = 0, vuol dire che lo devo generare io ora lato server
            '(dopo averlo generato devo cmq verificare se è sensato con la coerenza delle date)


            If originalDocNumero = 0 Then

                'Devo generare io in automatico il numero

                If isDocEmesso = False Then
                    Throw New GiasException(AgronicaCoreModelloRes.DocRicevutoMancanzaNumero)
                End If

                Dim objContabR As New AgronicaCoreContabDAL.Contabilita_R
                Dim lastNumDoc As Integer = 0
                lastNumDoc = objContabR.LastNumDocumento(piva, lavCod,
                                                         Year(dataMovimento),
                                                         docNumeroSin, docNumeroDes,
                                                         0, "", "",
                                                         objParametriServer)

                newNumDoc = lastNumDoc + 1
                newNumDocVisualizzato = FormattaNumeroDoc(docNumeroSin,
                                                          newNumDoc, docNumeroLunghezza, carattereFormattazione,
                                                          docNumeroDes)

                'Devo verificare se c'è coerenza tra il documento che voglio inserire e le date dei documenti con quel numero -1 e +1
                msgErroreGestito = VerificaCoerenzaDateNumDoc(newNumDoc,
                                                                  docNumeroSin, docNumeroDes,
                                                                  newNumDocVisualizzato,
                                                                  piva, lavCod, dataMovimento,
                                                                  objParametriServer)

                'caso particolare per Accettazione Fruttagel per mantenere la compatibilità col LAN finché lavorano con ambiente misto!!!
                If objParametriServer.PivaSuperUser = "01271980391" Then

                    Dim objSeqProgrW As New AgronicaCoreDataProvider.Sequenza_Progressivi_W

                    If lavCod = LAVCOD_ACCETTAZIONE_DIVERSI Then

                        objSeqProgrW.Modifica_Base(piva, Year(dataMovimento),
                                                   enum_SequenzaProgressiviTipi.BolleAccettDaDiversi,
                                                   docNumeroSin, docNumeroDes, 0,
                                                   newNumDoc,
                                                   "", objParametriServer)

                    ElseIf lavCod = LAVCOD_BOLLA_EMESSA Then

                        objSeqProgrW.Modifica_Base(piva, Year(dataMovimento),
                                                   enum_SequenzaProgressiviTipi.DDTEmessi,
                                                   docNumeroSin, docNumeroDes, 0,
                                                   newNumDoc,
                                                   "", objParametriServer)

                    End If

                End If

                'TODO: qui andrà integrato tutto il meccanismo per recuperare eventuali buchi di numerazione!

            ElseIf isDocEmesso = True AndAlso
                   originalDocNumero > 0 AndAlso
                   docNumeroLock = False Then

                'L'utente ha sbloccato il numero e mi ha scritto lui un numero

                'La verifica se il documento esiste già è stato fatto sopra

                newNumDoc = originalDocNumero
                newNumDocVisualizzato = docNumeroVisualizzato

                'Devo verificare se c'è coerenza tra il documento che voglio inserire e le date dei documenti con quel numero -1 e +1
                msgErroreGestito = VerificaCoerenzaDateNumDoc(newNumDoc,
                                                                  docNumeroSin, docNumeroDes,
                                                                  newNumDocVisualizzato,
                                                                  piva, lavCod, dataMovimento,
                                                                  objParametriServer)

            Else

                'C'è un numero assegnato, ma il numero è lockato ==> non è stato modificato dall'ultimo salvataggio
                'Nel momento in cui uno sblocca il campo numero, poi non lo può rilockare finché non ha salvato
                '==> non faccio nessun controllo e ripasso indietro ciò che mi è arrivato

                newNumDoc = originalDocNumero
                newNumDocVisualizzato = docNumeroVisualizzato

            End If

        Catch ex As GiasException
            msgErroreGestito = ex.Message
        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return msgErroreGestito

    End Function

#End Region

#Region "Aggiornamento Materie Prime Campionature"

    Public Shared Function AggiornaInserisciMatPrimeCamp(ByVal calCod As Integer,
                                                         ByVal listMatPrimeCampionature As List(Of Materia_Prima_Campionatura),
                                                         ByRef objParametriServer As AgronicaCoreParametri
                                                         ) As Boolean

        Const nomeRoutine = "UtilityHelper.AggiornaInserisciMatPrimeCamp()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False

        Dim matPrimCampHelper As New Agenda_Materie_Prime_Campionature_Helper

        Try

            Utility.VerificaApriTransazione(objParametriServer, flagConnessione, flagTransazione)

            If Not IsNothing(listMatPrimeCampionature) AndAlso listMatPrimeCampionature.Count > 0 Then

                For Each matPrimCamp In listMatPrimeCampionature

                    'Prima devo verificare se esiste, oppure se questa specifica riga la devo inserire, perché è un parametro che prima non esisteva
                    Dim listMat = matPrimCampHelper.Leggi(progressivo:=calCod,
                                                          tipo:=matPrimCamp.Tipo,
                                                          tipoCod:=0,
                                                          udmCod:=0,
                                                          objParametri:=objParametriServer)

                    If IsNothing(listMat) OrElse listMat.Count = 0 Then

                        'Si tratta di un tipo parametro che all'atto della creazione non era presente, quindi questa specifica riga va in insert
                        'Devo impostare il progressivo come il cal_cod, per fare in modo che sia uguale agli altri
                        matPrimCamp.Progressivo = calCod
                        xRisp = matPrimCampHelper.Scrivi(matPrimCamp, objParametriServer)

                    Else

                        xRisp = matPrimCampHelper.CancellaRiscriviMod(progressivo:=calCod,
                                                                      tipo:=matPrimCamp.Tipo,
                                                                      tipoCod:=listMat(0).Tipo_Cod,
                                                                      udmCod:=matPrimCamp.Udm_Cod,
                                                                      objParametri:=objParametriServer,
                                                                      flagVerificaPreventiva:=True,
                                                                      listMatPrime:=listMat,
                                                                      tipoCodNew:=matPrimCamp.Tipo_Cod,
                                                                      valCod:=matPrimCamp.Val_Cod,
                                                                      chkTaraCampionatura:=matPrimCamp.ChkTara_Campionatura,
                                                                      taraCampionatura:=matPrimCamp.Tara_Campionatura)
                    End If

                    If xRisp = False Then
                        Exit For
                    End If
                Next
            Else
                xRisp = True
            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

#End Region

#Region "Utility Varie"

    Public Shared Function CercaMovimentoSpecifico(ByVal cauMov As String,
                                                   ByVal piva As String,
                                                   ByVal idAgenda As Integer,
                                                   ByRef objParametriServer As AgronicaCoreParametri,
                                                   Optional ByVal opzioniLetturaMovimenti As Opzioni_Lettura_Movimenti = Nothing
                                                   ) As Movimento

        If IsNothing(opzioniLetturaMovimenti) Then
            opzioniLetturaMovimenti = New Opzioni_Lettura_Movimenti
        End If

        Const nomeRoutine = "UtilityHelper.CercaMovimentoSpecifico()"
        Dim objMovimentiHelper As New Agenda_Movimenti_Helper
        Dim mov As Movimento = Nothing

        Try

            Dim listMovimenti As List(Of Movimento) = objMovimentiHelper.Leggi(piva,
                                                                               0,
                                                                               idAgenda,
                                                                               objParametriServer,
                                                                               opzioniLetturaMovimenti:=opzioniLetturaMovimenti)

            If Not listMovimenti Is Nothing AndAlso listMovimenti.Count > 0 Then
                mov = listMovimenti.Find(Function(x) x.Cau_Mov = cauMov)
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return mov

    End Function

    Public Shared Function CercaMovimentoSpecificoSenzaDettagli(ByVal cauMov As String,
                                                                ByVal piva As String,
                                                                ByVal idAgenda As Integer,
                                                                ByRef objParametriServer As AgronicaCoreParametri
                                                                ) As Movimento

        Dim opzioniLetturaMovimenti As New Opzioni_Lettura_Movimenti With {
            .LeggiMovimentiDettagli = False,
            .LeggiMovimentiDettagliTecnici = False,
            .LeggiMovimentiDettagliTecniciExtra = False,
            .LeggiPagamenti = False
        }

        Return CercaMovimentoSpecifico(cauMov,
                                       piva,
                                       idAgenda,
                                       objParametriServer,
                                       opzioniLetturaMovimenti:=opzioniLetturaMovimenti)

    End Function

    Public Shared Function GetNomeUtenteFromCF(ByVal usernameModifica As String,
                                               ByRef objParametriUtenti As AgronicaCoreParametri
                                               ) As String
        Dim nomeUtente As String = ""

        Dim objUtentiDettagliR As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim username As String = objUtentiDettagliR.Username_From_CodFisc(usernameModifica, objParametriUtenti)

        If username <> "" Then
            nomeUtente = objUtentiDettagliR.NomeCognome_From_Username(username, objParametriUtenti)
        End If

        Return nomeUtente

    End Function

    Public Shared Function GetDesLib(piva As String, idAgenda As Integer, lavCod As Integer, ByRef objParametri As AgronicaCoreParametri) As String
        Dim desLib As String = ""

        Dim objAgendaR As New AgronicaCoreContabDAL.Agenda_R
        Dim dt As DataTable = objAgendaR.Leggi(piva, 0, idAgenda, lavCod,
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               "", "", objParametri)
        If dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then
            desLib = dt.Rows(0).Item("Des_Lib")
        End If

        Return desLib
    End Function

    Public Shared Function IsTrasformatoFF(ByVal moduloGias As enum_Omni_Modulo_Generazione, ByVal elemCod As Integer) As Boolean
        If (moduloGias = enum_Omni_Modulo_Generazione.FreshFood OrElse
            moduloGias = enum_Omni_Modulo_Generazione.Tabacco) AndAlso
           elemCod = TRASFORMATI_VEGETALI Then
            'Gestione Dedicata ai trasformati del Gias FF e Tabacco
            Return True
        Else
            If moduloGias = enum_Omni_Modulo_Generazione.Zoo AndAlso
                elemCod = TRASFORMATI_ANIMALI Then
                'Gestione Dedicata ai trasformati animali del Gias Zoo
                Return True
            Else
                Return False
            End If
        End If
    End Function

    Public Shared Function IsAccettazione(ByVal lavCod As Integer) As Boolean
        Select Case lavCod
            Case LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE
                Return True
            Case LAVCOD_DISTINTA_CARICO, LAVCOD_AUTO_DDT_EMESSO
                'TODO: Questi come li trattiamo?!?
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Shared Function IsNumeroDocumentoEmesso(ByVal lavCod As Integer) As Boolean
        Select Case lavCod
            Case LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_ORDINE_ACQUISTO
                Return True
            Case LAVCOD_DISTINTA_CARICO, LAVCOD_AUTO_DDT_EMESSO
                'TODO: Questi come li trattiamo?!?
                Return True
            Case LAVCOD_ORDINE_VENDITA, LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_FATTURA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Shared Function IsMovimentoMagazzino(ByVal lavCod As Integer) As Boolean
        Select Case lavCod
            Case LAVCOD_CARICO, LAVCOD_SCARICO
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Shared Function IsContrattoAffitto(ByVal lavCod As Integer) As Boolean

        Return lavCod = LAVCOD_CONTRATTO_AFFITTO

    End Function

    Public Shared Function IsFattura(ByVal lavCod As Integer) As Boolean
        Select Case lavCod
            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Shared Function IsNotaAccredito(ByVal lavCod As Integer) As Boolean
        Select Case lavCod
            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Shared Function IsOrdine(ByVal lavCod As Integer) As Boolean
        Select Case lavCod
            Case LAVCOD_ORDINE_VENDITA, LAVCOD_ORDINE_ACQUISTO
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Shared Function GetMatCodConfezione(ByRef dettaglioConfezionamento As Contabilita_Riga_Confezionamento) As Integer

        Dim matCodConfezione As Integer = 0

        If Not dettaglioConfezionamento Is Nothing Then
            If Not dettaglioConfezionamento.FF_confezione_Codice_Generazione_Link Is Nothing AndAlso
               dettaglioConfezionamento.FF_confezione_Codice_Generazione_Link <> 0 Then
                matCodConfezione = dettaglioConfezionamento.FF_confezione_Codice_Generazione_Link
            ElseIf Not dettaglioConfezionamento.FF_confezione_Mat_Cod_Generazione_Link Is Nothing AndAlso
                   dettaglioConfezionamento.FF_confezione_Mat_Cod_Generazione_Link <> 0 Then
                matCodConfezione = dettaglioConfezionamento.FF_confezione_Mat_Cod_Generazione_Link
            End If

            dettaglioConfezionamento.FF_confezione_Mat_Cod = matCodConfezione
        End If

        Return matCodConfezione

    End Function

    Public Shared Function GetMatCodContenitore(ByRef dettaglioConfezionamento As Contabilita_Riga_Confezionamento, ByVal piva As String, ByRef objParametri_Server As AgronicaCoreParametri) As Integer

        Dim matCodContenitore As Integer = 0

        If Not dettaglioConfezionamento Is Nothing Then

            If Not dettaglioConfezionamento.FF_contenitore_Codice_Generazione_Link Is Nothing AndAlso
               dettaglioConfezionamento.FF_contenitore_Codice_Generazione_Link <> 0 Then

                'Cerco il prodotto effettivo di anagrafica collegato al Codice_Generazione
                Dim handleOGenerazioni_Anagrafe_Log As New OGenerazioni_Anagrafe_Log_R()

                Dim dtOGenAnagrafeLog = handleOGenerazioni_Anagrafe_Log.LeggiOmniLog_Prodotto(piva, enum_Omni_Modulo_Generazione.FreshFood,
                    enum_Omni_Tipo_Generazione.BeniConfezionamento,
                    dettaglioConfezionamento.FF_contenitore_Codice_Generazione_Link, 0, 0, 0, "", objParametri_Server)

                If dtOGenAnagrafeLog.Rows.Count > 0 Then
                    matCodContenitore = dtOGenAnagrafeLog.Rows(0)("Mat_Cod")

                    dettaglioConfezionamento.FF_contenitore_Codice_Generazione_Link = 0
                    dettaglioConfezionamento.FF_contenitore_Mat_Cod_Generazione_Link = matCodContenitore

                Else
                    matCodContenitore = dettaglioConfezionamento.FF_contenitore_Codice_Generazione_Link
                End If

            ElseIf Not dettaglioConfezionamento.FF_contenitore_Mat_Cod_Generazione_Link Is Nothing AndAlso
                   dettaglioConfezionamento.FF_contenitore_Mat_Cod_Generazione_Link <> 0 Then

                matCodContenitore = dettaglioConfezionamento.FF_contenitore_Mat_Cod_Generazione_Link

            End If

            dettaglioConfezionamento.FF_contenitore_Mat_Cod = matCodContenitore
        End If

        Return matCodContenitore

    End Function

    Public Shared Function GetMatCodImballo(ByRef dettaglioConfezionamento As Contabilita_Riga_Confezionamento, ByVal piva As String, ByRef objParametri_Server As AgronicaCoreParametri) As Integer

        Dim matCodImballo As Integer = 0

        If Not dettaglioConfezionamento Is Nothing Then

            If Not dettaglioConfezionamento.FF_imballaggio_Codice_Generazione_Link Is Nothing AndAlso
               dettaglioConfezionamento.FF_imballaggio_Codice_Generazione_Link <> 0 Then

                'Cerco il prodotto effettivo di anagrafica collegato al Codice_Generazione
                Dim handleOGenerazioni_Anagrafe_Log As New OGenerazioni_Anagrafe_Log_R()

                Dim dtOGenAnagrafeLog = handleOGenerazioni_Anagrafe_Log.LeggiOmniLog_Prodotto(piva, enum_Omni_Modulo_Generazione.FreshFood,
                    enum_Omni_Tipo_Generazione.BeniConfezionamento,
                    dettaglioConfezionamento.FF_imballaggio_Codice_Generazione_Link, 0, 0, 0, "", objParametri_Server)

                If dtOGenAnagrafeLog.Rows.Count > 0 Then

                    matCodImballo = dtOGenAnagrafeLog.Rows(0)("Mat_Cod")

                    dettaglioConfezionamento.FF_imballaggio_Codice_Generazione_Link = 0
                    dettaglioConfezionamento.FF_imballaggio_Mat_Cod_Generazione_Link = matCodImballo

                Else
                    matCodImballo = dettaglioConfezionamento.FF_imballaggio_Codice_Generazione_Link
                End If


            ElseIf Not dettaglioConfezionamento.FF_imballaggio_Mat_Cod_Generazione_Link Is Nothing AndAlso
                   dettaglioConfezionamento.FF_imballaggio_Mat_Cod_Generazione_Link <> 0 Then

                matCodImballo = dettaglioConfezionamento.FF_imballaggio_Mat_Cod_Generazione_Link

            End If

            dettaglioConfezionamento.FF_imballaggio_Mat_Cod = matCodImballo
        End If

        Return matCodImballo

    End Function

#End Region

#Region "Cancellazione Documento / Riga"

    Public Shared Function CancellaDocumento(ByRef objParametriServer As AgronicaCoreParametri,
                                             ByVal piva As String,
                                             ByVal Id_Agenda As Integer,
                                             ByVal Id_Mov As Integer,
                                             ByVal Id_Mov_Det As Integer,
                                             ByVal Lav_Cod As Integer,
                                             ByVal Tipo_Operazione As enum_TipoOperazioneDB,
                                             ByVal IgnoraAvvisoWarning As Boolean,
                                             ByVal Bypass_Delete_Exceptions As Boolean,
                                             ByVal ModuloGiasLicenziato As enum_Omni_Modulo_Generazione,
                                             ByVal flagAggiornaConteggi As Boolean,
                                             ByRef messaggio As String,
                                             ByRef Modalita_Protetta As Integer,
                                             Optional ByVal bForza_No_Sessione As Boolean = False,
                                             Optional ByVal Preserva_Costi As Boolean = False,
                                             Optional ByRef objParametriUtenti As AgronicaCoreParametri = Nothing,
                                             Optional ByVal Preserva_GHG As Boolean = False,
                                             Optional ByVal idServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline,
                                             Optional ByVal origine As enum_SistemiEsterni = enum_SistemiEsterni.gias,
                                             Optional ByVal DataMovimento As Date? = Nothing
                                             ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim res As Boolean = False
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim conteggiTotali As Contabilita_Totali_Testata = Nothing

        Dim objParametriAgenda As ParametriAgenda_Temp.ParametriAgenda
        Dim bkParametriAgendaDataSessione As Date?

        Try

            Utility.VerificaApriTransazione(objParametriServer, flagConnessione, flagTransazione)

            Select Case bForza_No_Sessione

                Case True

                    objParametriAgenda = New ParametriAgenda_Temp.ParametriAgenda(Not bForza_No_Sessione) With {
                        .Piva = piva,
                        .Id_Agenda = Id_Agenda,
                        .Lav_Cod = Lav_Cod,
                        .Tipo_Operazione = CStr(Tipo_Operazione)
                    }

                Case False

                    objParametriAgenda = New ParametriAgenda_Temp.ParametriAgenda() With {
                        .Piva = piva,
                        .Id_Agenda = Id_Agenda,
                        .Lav_Cod = Lav_Cod,
                        .Tipo_Operazione = CStr(Tipo_Operazione)
                    }

            End Select

            If DataMovimento IsNot Nothing AndAlso DataMovimento.HasValue Then

                If Not bForza_No_Sessione Then
                    bkParametriAgendaDataSessione = objParametriAgenda.Data
                End If

                objParametriAgenda.Data = DataMovimento.Value
            End If

            Select Case Tipo_Operazione

                Case enum_TipoOperazioneDB.Cancellazione

                    If Lav_Cod = LAVCOD_CONTRATTO_AFFITTO AndAlso Not Bypass_Delete_Exceptions Then
                        ControllaPresenzaRiferimentiCatastali(piva, Id_Agenda, objParametriServer)
                    End If

                    Dim docData As Date = Now
                    Dim docDesLib As String = ""
                    Dim erroreInCancellazione As Boolean = False

                    res = Operazione_Agenda_Utility.GestisciCancellazione(objParametriAgenda, objParametriServer,
                                                                          messaggio, IgnoraAvvisoWarning,
                                                                          Id_Mov:=Id_Mov,
                                                                          Id_Mov_Det:=Id_Mov_Det,
                                                                          Modalita_Protetta:=Modalita_Protetta,
                                                                          Bypass_Delete_Exceptions:=Bypass_Delete_Exceptions,
                                                                          Preserva_Costi:=Preserva_Costi,
                                                                          Preserva_GHG:=Preserva_GHG,
                                                                          objParametri_Utenti:=objParametriUtenti, docData:=docData, docDesLib:=docDesLib,
                                                                          idServizio:=idServizio, origine:=origine, erroreInCancellazione:=erroreInCancellazione)

                    r.RispostaOK = res

                    If res = True Then

                        If Id_Mov_Det <> 0 Then
                            messaggio = "Riga cancellata correttamente."

                            If Bypass_Delete_Exceptions = False Then
                                'Loggo come modifica del documento
                                Dim objAgronicaLogAgendaW As New AgronicaCoreContabDAL.AgronicaLogAgenda_W
                                objAgronicaLogAgendaW.Scrivi(docData,
                                                             enum_TipoOperazioneDB.Modifica,
                                                             docDesLib,
                                                             Id_Agenda,
                                                             piva,
                                                             0,
                                                             Lav_Cod,
                                                             CInt(idServizio),
                                                             objParametriServer,
                                                             Origine:=CInt(origine))
                            End If


                            'Solo quando lo sto facendo da interfaccia e ho volutamente cancellato!!!
                            If flagAggiornaConteggi = True Then

                                'Devo aggiornare anche i campi di testata ed eliminare eventuali righe di imballaggi movimentate da questo dettaglio

                                If ModuloGiasLicenziato = enum_Omni_Modulo_Generazione.FreshFood OrElse
                                   ModuloGiasLicenziato = enum_Omni_Modulo_Generazione.Tabacco OrElse
                                   ModuloGiasLicenziato = enum_Omni_Modulo_Generazione.Zoo Then

                                    SistemaImballaggiDocumento(objParametriAgenda.Piva, objParametriAgenda.Id_Agenda,
                                                               Id_Mov, enum_TipoOperazioneDB.Cancellazione,
                                                               objParametriServer)

                                End If

                                'TODO: aggiorno colli testata + peso lordo testata (+ eventualmente provvigione, castelletto, ecc...)
                                conteggiTotali = AggiornaConteggiTotali(objParametriAgenda.Piva, objParametriAgenda.Lav_Cod,
                                                                        objParametriAgenda.Id_Agenda,
                                                                        objParametriServer, 0)

                            End If

                            If IsAccettazione(Lav_Cod) AndAlso Bypass_Delete_Exceptions = False Then
                                'Se ho correttamente cancellato una singola riga di un conferimento, richiamo la gestione degli scarichi da raccolte
                                Dim helperDettaglio As New ContabilitaHelper_Dettaglio(objParametriServer, objParametriUtenti)
                                Dim contabOutputRaccolte = helperDettaglio.ScriviScaricoRaccolteAziendaAgricola(Tipo_Operazione, piva, Id_Agenda, Id_Mov, Id_Mov_Det, 0)

                                If contabOutputRaccolte.Risultato = False Then
                                    res = False
                                    r.RispostaOK = False
                                    r.Errore = contabOutputRaccolte.MsgError
                                End If
                            End If

                        Else
                            messaggio = "Documento cancellato correttamente."
                            'La cancellazione del documento è loggata dalla classe Operazione_Agenda
                        End If

                    ElseIf IgnoraAvvisoWarning OrElse erroreInCancellazione Then
                        'Sono in cancellazione effettiva
                        r.Errore = messaggio
                        r.RispostaConferma = False
                    Else
                        'Rimando al chiamante il messaggio di warning in r.RispostaStringa
                        r.Errore = ""
                        r.RispostaConferma = True
                    End If

                Case Else

                    Dim matrice_delete(,) As String
                    res = ControllaOperazione(matrice_delete, objParametriAgenda, objParametriServer, messaggio,
                                              IgnoraAvvisoWarning, "",
                                              Id_Mov, Id_Mov_Det, Modalita_Protetta, objParametri_Utenti:=objParametriUtenti)

                    r.RispostaOK = res

                    If res = True Then

                        'Modifica accettata senza problemi
                        messaggio = ""

                    Else

                        r.Errore = ""
                        r.RispostaConferma = True

                    End If

            End Select

            'Concatenazione Risposta+ModalitaProtetta

            Dim objRisp As Object = New With {
                .RispostaStringa = messaggio,
                .ModalitaProtetta = Modalita_Protetta,
                .ConteggiTotali = conteggiTotali,
                .Time = Now
            }

            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            r.RispostaStringa = JsonConvert.SerializeObject(objRisp, settingLoc)

            If res = True Then
                Utility.VerificaChiudiTransazione(objParametriServer, flagTransazione)
            Else
                Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            End If

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            r.RispostaOK = False
            r.Errore = ex.Message
            r.RispostaStringa = ""
            r.RispostaConferma = False

        Finally

            Utility.VerificaChiudiConnessione(objParametriServer, flagConnessione)

            If Not bForza_No_Sessione Then

                If bkParametriAgendaDataSessione IsNot Nothing AndAlso bkParametriAgendaDataSessione.HasValue Then
                    objParametriAgenda.Data = bkParametriAgendaDataSessione
                End If

            End If

        End Try

        Return r

    End Function

    Private Shared Sub ControllaPresenzaRiferimentiCatastali(piva As String,
                                                             idAgenda As Integer,
                                                             ByRef objParametri_Server As AgronicaCoreParametri)

        Dim leggiContrattiImpreseParticelle As New ContrattiXImpreseXParticelle_R

        Dim dtContrattiImpreseParticelle = leggiContrattiImpreseParticelle.Leggi(piva,
                                                                                 0,
                                                                                 idAgenda,
                                                                                 0,
                                                                                 New ContrattiXImpreseXParticelle_Particella,
                                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                 "",
                                                                                 "",
                                                                                 objParametri_Server)

        If Not IsNothing(dtContrattiImpreseParticelle) AndAlso dtContrattiImpreseParticelle.Rows.Count > 0 Then

            Dim messaggioEccezione = "Cancellazione non consentita: riferimenti catastali presenti per il contratto di affitto."

            Throw New Exception(messaggioEccezione)

        End If

    End Sub

#End Region

#Region "Controllo Permessi Workflow"

    Public Shared Sub SeControllaPermessiStatoWorkflowCancellazione(ByVal lavCod As Integer,
                                                                    ByVal piva As String,
                                                                    ByVal IdAgenda As Integer,
                                                                    ByRef objParametriServer As AgronicaCoreParametri,
                                                                    ByRef objParametriUtenti As AgronicaCoreParametri)

        Const messaggioUtenteNonAbilitatoCancellazione = "Utente non abilitato alla cancellazione del documento."

        Dim objImpImp As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

        Dim diz_LavCod_ServizioCod = objImpImp.LeggiDizionario_WorkflowDocContabili(piva, objParametriServer)

        If objImpImp.LavCod_Gestisce_Workflow(lavCod, diz_LavCod_ServizioCod) Then

            Dim objPermessiStato As New AgronicaCoreUtentiBIZ.GruppiUtente_PermessiStato(objParametriServer, objParametriUtenti)

            If Not objPermessiStato.IsUtenteSuperUser() Then

                Dim gruppoUtenteCod = objPermessiStato.LeggiGruppoUtente(messaggioUtenteNonAbilitatoCancellazione)

                Dim servizioCod = objImpImp.Ottieni_ServizioCod_Da_LavCod(lavCod, diz_LavCod_ServizioCod)

                Dim statoCod = LeggiStatoPraticaAgenda(lavCod, piva, IdAgenda, objParametriServer)

                Dim permessiStato = objPermessiStato.LeggiPermessiStato(gruppoUtenteCod, servizioCod, statoCod)

                If permessiStato.Cancella = False Then

                    Throw New Exception(messaggioUtenteNonAbilitatoCancellazione)

                End If

            End If

        End If

    End Sub

    Public Function SeControllaPermessiStatoWorkflowAperturaDocumento(ByVal lavCod As Integer,
                                                                      ByVal objTestata As Contabilita_Testata,
                                                                      ByRef objParametriServer As AgronicaCoreParametri,
                                                                      ByRef objParametriUtenti As AgronicaCoreParametri,
                                                                      ByRef utenteAbilitatoLettura As Boolean,
                                                                      ByRef utenteAbilitatoScrittura As Boolean) As Boolean

        Const messaggioUtenteAccessoLimitato = "Utente con accesso limitato al documento."

        Dim permessiModificati As Boolean = False

        Dim objImpImp As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

        Dim diz_LavCod_ServizioCod = objImpImp.LeggiDizionario_WorkflowDocContabili(objTestata.Piva, objParametriServer)

        If objImpImp.LavCod_Gestisce_Workflow(lavCod, diz_LavCod_ServizioCod) Then

            Dim objPermessiStato As New AgronicaCoreUtentiBIZ.GruppiUtente_PermessiStato(objParametriServer, objParametriUtenti)

            If Not objPermessiStato.IsUtenteSuperUser() Then

                Dim gruppoUtenteCod = objPermessiStato.LeggiGruppoUtente(messaggioUtenteAccessoLimitato)

                Dim servizioCod = objImpImp.Ottieni_ServizioCod_Da_LavCod(lavCod, diz_LavCod_ServizioCod)

                Dim permessiStato = objPermessiStato.LeggiPermessiStato(gruppoUtenteCod, servizioCod, objTestata.StatoCodPratica)

                Dim permessiStatoUtente = objPermessiStato.DeterminaPermessiStatoUtente(permessiStato,
                                                                                        utenteAbilitatoLettura,
                                                                                        utenteAbilitatoScrittura)

                If utenteAbilitatoLettura <> permessiStatoUtente.Visualizza Then
                    utenteAbilitatoLettura = permessiStatoUtente.Visualizza
                    permessiModificati = True
                End If

                If utenteAbilitatoScrittura <> permessiStatoUtente.Modifica Then
                    utenteAbilitatoScrittura = permessiStatoUtente.Modifica
                    permessiModificati = True
                End If

            End If

        End If

        Return permessiModificati

    End Function

    Private Shared Function LeggiStatoPraticaAgenda(ByVal lavCod As Integer,
                                                    ByVal piva As String,
                                                    ByVal IdAgenda As Integer,
                                                    ByRef objParametriServer As AgronicaCoreParametri) As Integer

        Dim objAgenda = LeggiAgendaTestata(lavCod, piva, IdAgenda, objParametriServer)

        Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R

        Return objPratiche.LeggiStatoAttualePratica(objAgenda.Pratica_Cod, objParametriServer)

    End Function

    Private Shared Function LeggiAgendaTestata(ByVal lavCod As Integer,
                                               ByVal piva As String,
                                               ByVal IdAgenda As Integer,
                                               ByRef objParametriServer As AgronicaCoreParametri) As Operazione_Agenda

        Dim objAgendaHelper As New Agenda_Operazione_Helper

        Dim opzioniLetturaMovimenti = New Opzioni_Lettura_Movimenti With {
        .LeggiMovimentiDettagli = False,
        .LeggiMovimentiDettagliTecnici = False,
        .LeggiMovimentiDettagliTecniciExtra = False,
        .LeggiPagamenti = False
        }

        Dim opzioniLetturaAgenda = New Opzioni_Lettura_Agenda With {
        .LeggiGps = False,
        .LeggiNote = False,
        .LeggiRiferimenti = False,
        .LeggiMovimenti = False,
        .OpzioniLetturaMovimenti = opzioniLetturaMovimenti
        }

        Return objAgendaHelper.Leggi(piva,
                                     0,
                                     IdAgenda,
                                     lavCod,
                                     objParametriServer,
                                     opzioniLetturaAgenda:=opzioniLetturaAgenda)

    End Function

#End Region

#Region "Ricalcolo Dati Dettaglio"

    Public Shared Sub AggiornaDettagliEconomici(ByRef objContabDettaglio As Contabilita_Riga,
                                                ByVal lavCod As Integer)

        Const nomeRoutine = "UtilityHelper.AggiornaDettagliEconomici"
        Dim Qta As Decimal
        Dim Prezzo_Parziale As Decimal
        Dim Sconto_Totale As Decimal
        Dim Parziale As Decimal
        Dim Prezzo_Unitario As Decimal
        Dim Prezzo_Unitario_Netto As Decimal
        Dim bBypass As Boolean
        Dim Iva_Manuale As Decimal
        Dim Iva As Decimal
        Dim bCalcoloSconto As Boolean
        Dim Sconto As Decimal
        Dim kgDegrado As Decimal
        Dim kgNettiPesoPagamento As Decimal

        Try

            If objContabDettaglio.PrezzoRiferitoA Is Nothing Then
                Throw New Exception("Necessario impostare 'Prezzo Riferito A'")
            End If

            ' check se sono in conferimento pomodoro (da gestire con tipo accettazione -1)
            Dim conferimentoPomodoro As Boolean = IsAccettazione(lavCod) AndAlso objContabDettaglio.Conferimento_Speciale IsNot Nothing

            'TODO: prima di partire devo assicurarmi che tutti i campi (che sono nullable) non siano nothing, ma inizializzati adeguatamente (0?)
            objContabDettaglio.Quantita = If(objContabDettaglio.Quantita, 0)
            objContabDettaglio.NumConfezioni = If(objContabDettaglio.NumConfezioni, 0)
            objContabDettaglio.NumContenitori = If(objContabDettaglio.NumContenitori, 0)
            objContabDettaglio.NumImballi = If(objContabDettaglio.NumImballi, 0)

            objContabDettaglio.KgNetti = If(objContabDettaglio.KgNetti, 0)

            objContabDettaglio.Prezzo = If(objContabDettaglio.Prezzo, 0)
            objContabDettaglio.PrezzoNetto = If(objContabDettaglio.PrezzoNetto, 0)
            objContabDettaglio.PrezzoEffettivoKgL = If(objContabDettaglio.PrezzoEffettivoKgL, 0)

            objContabDettaglio.ScontoBase = If(objContabDettaglio.ScontoBase, 0)
            objContabDettaglio.MaggiorazioneBase = If(objContabDettaglio.MaggiorazioneBase, 0)
            objContabDettaglio.ScontoMaggiorazioneBase = If(objContabDettaglio.ScontoMaggiorazioneBase, 0)
            objContabDettaglio.ScontoBaseEuro = If(objContabDettaglio.ScontoBaseEuro, 0)

            objContabDettaglio.ScontoAddiz1 = If(objContabDettaglio.ScontoAddiz1, 0)
            objContabDettaglio.ScontoAddiz2 = If(objContabDettaglio.ScontoAddiz2, 0)
            objContabDettaglio.ScontoAddiz3 = If(objContabDettaglio.ScontoAddiz3, 0)
            objContabDettaglio.ScontoAddizTotalePerc = If(objContabDettaglio.ScontoAddizTotalePerc, 0)
            objContabDettaglio.ScontoAddizTotaleEuro = If(objContabDettaglio.ScontoAddizTotaleEuro, 0)

            objContabDettaglio.Iva = If(objContabDettaglio.Iva, 0)
            objContabDettaglio.AliquotaIva = If(objContabDettaglio.AliquotaIva, 0)

            objContabDettaglio.IvaModalita = If(objContabDettaglio.IvaModalita, enum_ModalitaIva.Nessuno)

            objContabDettaglio.IvaIndetraibilePerc = If(objContabDettaglio.IvaIndetraibilePerc, 0)
            objContabDettaglio.IvaIndetraibile = If(objContabDettaglio.IvaIndetraibile, 0)
            objContabDettaglio.IvaCreditoDebito = If(objContabDettaglio.IvaCreditoDebito, 0)

            objContabDettaglio.ImponibileTotale = If(objContabDettaglio.ImponibileTotale, 0)
            objContabDettaglio.ImponibileTotaleNetto = If(objContabDettaglio.ImponibileTotaleNetto, 0)
            objContabDettaglio.ImportoUnitario = If(objContabDettaglio.ImportoUnitario, 0)
            objContabDettaglio.ImportoTotale = If(objContabDettaglio.ImportoTotale, 0)

            objContabDettaglio.ProvvigionePercAgente = If(objContabDettaglio.ProvvigionePercAgente, 0)
            objContabDettaglio.ProvvigioneAgente = If(objContabDettaglio.ProvvigioneAgente, 0)
            objContabDettaglio.ProvvigionePercCapoArea = If(objContabDettaglio.ProvvigionePercCapoArea, 0)
            objContabDettaglio.ProvvigioneCapoArea = If(objContabDettaglio.ProvvigioneCapoArea, 0)

            objContabDettaglio.EsigibilitaIva = If(objContabDettaglio.EsigibilitaIva, 0)

            objContabDettaglio.ScontoMaggiorazioneBase = 0

            'Formattazione Parametri
            If objContabDettaglio.ScontoBase <> 0 Then
                objContabDettaglio.MaggiorazioneBase = ArrotondaVal_2(0)
                objContabDettaglio.ScontoMaggiorazioneBase = -objContabDettaglio.ScontoBase
            End If

            If objContabDettaglio.MaggiorazioneBase <> 0 Then
                objContabDettaglio.ScontoBase = ArrotondaVal_2(0)
                objContabDettaglio.ScontoMaggiorazioneBase = objContabDettaglio.MaggiorazioneBase
            End If


            objContabDettaglio.Degrado = If(objContabDettaglio.Degrado, 0)
            Dim pesoNetto As Decimal = objContabDettaglio.KgNetti

            'Se l'unità di misura arriva come Quintali o Tonnellate, converto il peso in Kg per calcolare il degrado,
            'potendo così arrotondare quest'ultimo e poi reimposto kgNettiPesoPagamento alla giusta unità di misura per i calcoli successivi
            'sui prezzi.
            If objContabDettaglio.UdM = enum_UnitaMisura.Quintali Then
                pesoNetto = objContabDettaglio.KgNetti * 100
            ElseIf objContabDettaglio.UdM = enum_UnitaMisura.Tonnellate Then
                pesoNetto = objContabDettaglio.KgNetti * 1000
            End If

            If objContabDettaglio.Degrado <> 0 Then

                'TODO: è da lasciare arrotondato all'intero?
                kgDegrado = ArrotondaVal_0(pesoNetto / 100 * objContabDettaglio.Degrado)

            Else
                kgDegrado = 0D
            End If

            kgNettiPesoPagamento = pesoNetto - kgDegrado
            If objContabDettaglio.UdM = enum_UnitaMisura.Quintali Then
                kgNettiPesoPagamento /= 100
            ElseIf objContabDettaglio.UdM = enum_UnitaMisura.Tonnellate Then
                kgNettiPesoPagamento /= 1000
            End If

            '-------------------------------------------------------------------------------------------

            'TODO: se è livello_prezzo = KG, devo mandarmi il prezzo effettivo al Kg (l'utente dovrebbe editare quello)
            'oppure mando il semplice prezzo_unitario, ma sono da rivedere tutti i conti (e cmq su db devo scrivere sempre anche il prezzo effettivo)

            ' se conferimento pomodoro forzo il prezzo unitario in quanto calcolato esternamente
            If conferimentoPomodoro Then

                objContabDettaglio.Quantita = kgNettiPesoPagamento 'objContabDettaglio.KgNetti
                Qta = Math.Abs(CDec(objContabDettaglio.Quantita))
                Prezzo_Unitario = objContabDettaglio.PrezzoNetto

            ElseIf objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Kg_Litri OrElse
               objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Udm_principale Then

                'TODO: se prezzo livello è KG, ai fini dei miei conteggi, Quantita deve essere = Kg Netti
                If objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Kg_Litri Then
                    'Se non mi è arrivata la qta, ma mi sono arrivati i kg netti, questi diventano a tutti gli effetti la qta da usare
                    If objContabDettaglio.Quantita = 0 AndAlso kgNettiPesoPagamento <> 0 Then
                        objContabDettaglio.Quantita = kgNettiPesoPagamento
                    End If
                End If

                Qta = Math.Abs(CDec(objContabDettaglio.Quantita))

                If objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Kg_Litri AndAlso
                   objContabDettaglio.PrezzoEffettivoKgL <> 0 AndAlso
                   kgNettiPesoPagamento > 0 AndAlso
                   Qta <> 0 Then

                    'AndAlso TOUCHSCREEN <> 2 Then

                    Prezzo_Unitario = ArrotondaVal_6(objContabDettaglio.PrezzoEffettivoKgL * (kgNettiPesoPagamento / Qta))

                Else
                    Prezzo_Unitario = objContabDettaglio.Prezzo
                End If

            ElseIf objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Confezione Then

                If objContabDettaglio.NumConfezioni = 0 Then
                    Qta = 0
                Else
                    'Qta = Math.Abs(Formattazione_Numerica(objContabDettaglio.NumConfezioni))
                    Qta = Math.Abs(CDec(objContabDettaglio.NumConfezioni))
                End If

                'If objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Kg_Litri AndAlso
                '   objContabDettaglio.PrezzoEffettivoKgL <> 0 AndAlso
                '   kgNettiPesoPagamento > 0 AndAlso
                '   Qta <> 0 Then

                '    Prezzo_Unitario = ArrotondaVal_6(objContabDettaglio.PrezzoEffettivoKgL * (kgNettiPesoPagamento / Qta))

                'Else

                If IsNumeric(objContabDettaglio.Prezzo) Then
                    Prezzo_Unitario = objContabDettaglio.Prezzo
                End If

                'End If

                'Prezzo_Unitario_Netto = objContabDettaglio.PrezzoNetto

            ElseIf objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Contenitore Then

                If objContabDettaglio.NumContenitori = 0 Then
                    Qta = 0
                Else
                    'Qta = Math.Abs(Formattazione_Numerica(objContabDettaglio.NumContenitori))
                    Qta = Math.Abs(CDec(objContabDettaglio.NumContenitori))
                End If

                'If objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Kg_Litri AndAlso
                '   objContabDettaglio.PrezzoEffettivoKgL <> 0 AndAlso
                '   kgNettiPesoPagamento > 0 AndAlso
                '   Qta <> 0 Then

                '    Prezzo_Unitario = ArrotondaVal_6(objContabDettaglio.PrezzoEffettivoKgL * (kgNettiPesoPagamento / Qta))

                'Else

                If IsNumeric(objContabDettaglio.Prezzo) Then
                    Prezzo_Unitario = objContabDettaglio.Prezzo
                End If

                'End If

                'Prezzo_Unitario_Netto = objContabDettaglio.PrezzoNetto

            ElseIf objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Imballo Then

                If objContabDettaglio.NumImballi = 0 Then
                    Qta = 0
                Else
                    'Qta = Math.Abs(Formattazione_Numerica(objContabDettaglio.NumImballi))
                    Qta = Math.Abs(CDec(objContabDettaglio.NumImballi))
                End If

                'If objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Kg_Litri AndAlso
                '   IsNumeric(objContabDettaglio.PrezzoEffettivoKgL) AndAlso
                '   kgNettiPesoPagamento > 0 AndAlso
                '   Qta <> 0 Then

                '    Prezzo_Unitario = ArrotondaVal_3(objContabDettaglio.PrezzoEffettivoKgL * (kgNettiPesoPagamento / Qta))

                'Else

                If IsNumeric(objContabDettaglio.Prezzo) Then
                    Prezzo_Unitario = objContabDettaglio.Prezzo
                End If
                'Prezzo_Unitario_Netto = objContabDettaglio.PrezzoNetto

                'End If

            End If


            Select Case objContabDettaglio.ValoreRiferimentoPrezzo


               '#########################################################################################
               '##################### MODALITA' INSERIMENTO = IMPORTO TOTALE  ###########################
               '#########################################################################################

                Case enum_EditImporto.Importo

                    'Aggiorno il costo unitario della registrazione
                    If Qta > 0 And IsNumeric(objContabDettaglio.ImportoTotale) Then

                        If objContabDettaglio.ForzaIva = False Then

                            'Iva
                            'If CmbIva.ListIndex <> -1 Then
                            If objContabDettaglio.CodIva <> -1 Then
                                objContabDettaglio.Iva = (objContabDettaglio.ImportoTotale / (100 + objContabDettaglio.AliquotaIva)) * objContabDettaglio.AliquotaIva
                                objContabDettaglio.Iva = FormattazioneIva(objContabDettaglio.Iva, lavCod, objContabDettaglio)
                            Else
                                objContabDettaglio.Iva = ArrotondaVal_2(0) 'FCI, NI
                            End If

                        End If

                        Iva = objContabDettaglio.Iva

                        objContabDettaglio.ImponibileTotaleNetto = ArrotondaVal_2(objContabDettaglio.ImportoTotale - Iva)


                        objContabDettaglio.ScontoAddizTotaleEuro = 0 'Inizializzazione
                        Parziale = objContabDettaglio.ImponibileTotaleNetto

                        objContabDettaglio.ScontoAddizTotaleEuro = objContabDettaglio.ScontoAddizTotaleEuro + (((Parziale * 100) / (100 - objContabDettaglio.ScontoAddiz3)) - Parziale) / Qta
                        Parziale = (Parziale * 100) / (100 - objContabDettaglio.ScontoAddiz3)

                        objContabDettaglio.ScontoAddizTotaleEuro = objContabDettaglio.ScontoAddizTotaleEuro + (((Parziale * 100) / (100 - objContabDettaglio.ScontoAddiz2)) - Parziale) / Qta
                        Parziale = (Parziale * 100) / (100 - objContabDettaglio.ScontoAddiz2)

                        objContabDettaglio.ScontoAddizTotaleEuro = objContabDettaglio.ScontoAddizTotaleEuro + (((Parziale * 100) / (100 - objContabDettaglio.ScontoAddiz1)) - Parziale) / Qta
                        Parziale = (Parziale * 100) / (100 - objContabDettaglio.ScontoAddiz1)


                        'Calcolo il prezzo ottenuto dopo lo sconto sul cliente
                        Prezzo_Parziale = objContabDettaglio.ImponibileTotaleNetto + (objContabDettaglio.ScontoAddizTotaleEuro * Qta)

                        'Controllo Overflow
                        If objContabDettaglio.ScontoMaggiorazioneBase = -100 Then
                            objContabDettaglio.ScontoBaseEuro = 0
                        Else
                            objContabDettaglio.ScontoBaseEuro = (((Prezzo_Parziale * 100) / (100 + objContabDettaglio.ScontoMaggiorazioneBase)) - Prezzo_Parziale) / Qta
                        End If


                        'Prezzo_Unitario = ArrotondaVal_6((objContabDettaglio.ImponibileTotale / Qta))
                        Prezzo_Unitario_Netto = ArrotondaVal_6(objContabDettaglio.ImponibileTotaleNetto / Qta)
                        Prezzo_Unitario = ArrotondaVal_6(Prezzo_Unitario_Netto + objContabDettaglio.ScontoBaseEuro + objContabDettaglio.ScontoAddizTotaleEuro)
                        objContabDettaglio.ImponibileTotale = ArrotondaVal_2(CDec(Qta) * Prezzo_Unitario)

                        objContabDettaglio.ScontoBaseEuro = ArrotondaVal_6(Math.Abs(CDec(objContabDettaglio.ScontoBaseEuro)))
                        objContabDettaglio.ScontoAddizTotaleEuro = ArrotondaVal_6(objContabDettaglio.ScontoAddizTotaleEuro)

                        'Ricalcolo iva (inizialmente era calcolata sul costo già arrotondato a 2 cifre e quindi ci possono essere differenze sul cambio el modo di calcolo
                        If objContabDettaglio.ForzaIva = False Then
                            objContabDettaglio.Iva = FormattazioneIva((objContabDettaglio.ImponibileTotaleNetto / 100) * objContabDettaglio.AliquotaIva, lavCod, objContabDettaglio)
                        End If

                        Iva = objContabDettaglio.Iva


                        If Qta <> 0 Then

                            objContabDettaglio.ImportoUnitario = ArrotondaVal_6(Math.Abs(CDec(objContabDettaglio.ImportoTotale) / Qta))

                            'Aggiustamento Segno Prezzo Unitario
                            If objContabDettaglio.ImportoTotale < 0 Then
                                objContabDettaglio.ImportoUnitario = ArrotondaVal_6(-objContabDettaglio.ImportoUnitario)
                            End If

                        Else
                            objContabDettaglio.ImportoUnitario = ArrotondaVal_2(0)
                        End If

                    Else

                        Prezzo_Unitario_Netto = ArrotondaVal_2(0)
                        objContabDettaglio.ImponibileTotale = ArrotondaVal_2(0)
                        objContabDettaglio.ImponibileTotaleNetto = ArrotondaVal_2(0)
                        Prezzo_Unitario = ArrotondaVal_2(0)
                        objContabDettaglio.ScontoBaseEuro = ArrotondaVal_2(0)
                        objContabDettaglio.ScontoAddizTotaleEuro = ArrotondaVal_2(0)
                        objContabDettaglio.ImportoUnitario = ArrotondaVal_2(0)

                    End If



               '#########################################################################################
               '##################### MODALITA' INSERIMENTO = PREZZO UNITARIO  ##########################
               '#########################################################################################

                Case enum_EditImporto.PrezzoUnitario

                    'Aggiorno il costo complessivo della registrazione
                    If IsNumeric(Qta) And IsNumeric(Prezzo_Unitario) Then

                        Prezzo_Unitario_Netto = Prezzo_Unitario + ((Prezzo_Unitario / 100) * objContabDettaglio.ScontoMaggiorazioneBase)
                        objContabDettaglio.ScontoBaseEuro = Prezzo_Unitario_Netto - Prezzo_Unitario

                        'Sottraggo gli Sconti Addizionali in Cascata
                        Prezzo_Unitario_Netto = ArrotondaVal_6(Prezzo_Unitario_Netto - ((Prezzo_Unitario_Netto / 100) * objContabDettaglio.ScontoAddiz1))
                        Prezzo_Unitario_Netto = ArrotondaVal_6(Prezzo_Unitario_Netto - ((Prezzo_Unitario_Netto / 100) * objContabDettaglio.ScontoAddiz2))
                        Prezzo_Unitario_Netto = ArrotondaVal_6(Prezzo_Unitario_Netto - ((Prezzo_Unitario_Netto / 100) * objContabDettaglio.ScontoAddiz3))


                        objContabDettaglio.ScontoAddizTotaleEuro = ArrotondaVal_6(Math.Abs(Prezzo_Unitario - Prezzo_Unitario_Netto + CDec(objContabDettaglio.ScontoBaseEuro)))
                        objContabDettaglio.ScontoBaseEuro = ArrotondaVal_6(Math.Abs(CDec(objContabDettaglio.ScontoBaseEuro)))

                        bCalcoloSconto = False

                        Select Case objContabDettaglio.ScontoModalita

                            Case enModalitaSconto.Percentuale

                                objContabDettaglio.ImponibileTotale = ArrotondaVal_2(Qta * Prezzo_Unitario)
                                'objContabDettaglio.ImponibileTotaleNetto = ArrotondaVal_2(Qta * Prezzo_Unitario_Netto)
                                bCalcoloSconto = True

                            Case enModalitaSconto.Omaggio_SenzaRivalsaIva

                                objContabDettaglio.ImponibileTotale = ArrotondaVal_2(Qta * Prezzo_Unitario)
                                objContabDettaglio.ImponibileTotaleNetto = objContabDettaglio.ImponibileTotale 'ArrotondaVal_2(Qta * Prezzo_Unitario)
                                bCalcoloSconto = False

                            Case enModalitaSconto.Sconto_Merce, enModalitaSconto.Campioni_Gratuiti

                                objContabDettaglio.ImponibileTotale = ArrotondaVal_2(Qta * Prezzo_Unitario)
                                objContabDettaglio.ImponibileTotaleNetto = ArrotondaVal_2(0)

                            Case enModalitaSconto.Omaggio_ConRivalsaIva

                                objContabDettaglio.ImponibileTotale = ArrotondaVal_2(Qta * Prezzo_Unitario)
                                objContabDettaglio.ImponibileTotaleNetto = objContabDettaglio.ImponibileTotale 'ArrotondaVal_2(Qta * Prezzo_Unitario)
                                bCalcoloSconto = False

                        End Select


                        If bCalcoloSconto Then

                            'Sottraggo gli Sconti Addizionali in Cascata
                            Sconto = -objContabDettaglio.ScontoMaggiorazioneBase
                            Sconto = Sconto + ((100 - Sconto) / 100 * objContabDettaglio.ScontoAddiz1)
                            Sconto = Sconto + ((100 - Sconto) / 100 * objContabDettaglio.ScontoAddiz2)
                            Sconto = ArrotondaVal_6(Sconto + ((100 - Sconto) / 100 * objContabDettaglio.ScontoAddiz3))
                            objContabDettaglio.ImponibileTotaleNetto = ArrotondaVal_2(objContabDettaglio.ImponibileTotale * (100 - Sconto) / 100)

                        End If

                        If objContabDettaglio.ForzaIva = False Then

                            'Iva
                            'If CmbIva.ListIndex <> -1 Then
                            If objContabDettaglio.CodIva <> -1 Then
                                objContabDettaglio.Iva = (objContabDettaglio.ImponibileTotaleNetto / 100) * objContabDettaglio.AliquotaIva
                            Else
                                objContabDettaglio.Iva = 0
                            End If

                            objContabDettaglio.Iva = FormattazioneIva(objContabDettaglio.Iva, lavCod, objContabDettaglio)

                        End If

                        Iva = objContabDettaglio.Iva

                        Select Case objContabDettaglio.ScontoModalita

                            Case enModalitaSconto.Percentuale

                                objContabDettaglio.ImportoTotale = ArrotondaVal_2(objContabDettaglio.ImponibileTotaleNetto + Iva)

                            Case enModalitaSconto.Omaggio_ConRivalsaIva

                                'Paga solo l'iva
                                objContabDettaglio.ImportoTotale = ArrotondaVal_2(Iva)

                            Case Else

                                objContabDettaglio.ImportoTotale = ArrotondaVal_2(0)

                        End Select

                        If Qta <> 0 Then
                            objContabDettaglio.ImportoUnitario = ArrotondaVal_6(Math.Abs(CDec(objContabDettaglio.ImportoTotale) / Qta))

                            'Aggiustamento Segno Prezzo Unitario
                            If objContabDettaglio.ImportoTotale < 0 Then
                                objContabDettaglio.ImportoUnitario = ArrotondaVal_6(-objContabDettaglio.ImportoUnitario)
                            End If

                        Else
                            objContabDettaglio.ImportoUnitario = ArrotondaVal_2(0)
                        End If

                    Else

                        objContabDettaglio.ImponibileTotaleNetto = ArrotondaVal_2(0)
                        objContabDettaglio.ScontoBaseEuro = ArrotondaVal_2(0)
                        objContabDettaglio.ScontoAddizTotaleEuro = ArrotondaVal_2(0)
                        Prezzo_Unitario_Netto = ArrotondaVal_2(0)
                        objContabDettaglio.ImportoUnitario = ArrotondaVal_2(0)
                        objContabDettaglio.ImportoTotale = ArrotondaVal_2(0)
                        objContabDettaglio.ImponibileTotale = ArrotondaVal_2(0)

                    End If




               '#########################################################################################
               '##################### MODALITA' INSERIMENTO = IMPONIBILE ################################
               '#########################################################################################

                Case enum_EditImporto.Imponibile


                    'Aggiorno il costo unitario della registrazione
                    If Qta > 0 And IsNumeric(objContabDettaglio.ImponibileTotale) Then

                        'If ArrotondaVal_2(Qta * Prezzo_Unitario) = objContabDettaglio.ImponibileTotale Then
                        '   'Non occorre reimpostare il prezzo unitario poiché dà lo stesso imponibile
                        'Else
                        Prezzo_Unitario = ArrotondaVal_6(objContabDettaglio.ImponibileTotale / Qta)
                        'End If

                        objContabDettaglio.ScontoBaseEuro = (objContabDettaglio.ImponibileTotale * (-objContabDettaglio.ScontoMaggiorazioneBase)) / (100 * Qta)

                        'Calcolo il prezzo ottenuto dopo lo sconto sul cliente
                        Prezzo_Parziale = objContabDettaglio.ImponibileTotale - (objContabDettaglio.ScontoBaseEuro * Qta)

                        Parziale = 0
                        objContabDettaglio.ScontoAddizTotaleEuro = 0

                        Parziale = (Prezzo_Parziale * objContabDettaglio.ScontoAddiz3) / (100 * Qta)
                        Prezzo_Parziale -= Parziale
                        objContabDettaglio.ScontoAddizTotaleEuro += Parziale

                        Parziale = ArrotondaVal_6((Prezzo_Parziale * objContabDettaglio.ScontoAddiz2) / (100 * Qta))
                        Prezzo_Parziale -= Parziale
                        objContabDettaglio.ScontoAddizTotaleEuro += Parziale

                        Parziale = ArrotondaVal_6((Prezzo_Parziale * objContabDettaglio.ScontoAddiz1) / (100 * Qta))
                        Prezzo_Parziale -= Parziale
                        objContabDettaglio.ScontoAddizTotaleEuro += Parziale


                        '
                        '                     If bCalcoloSconto Then

                        'Sottraggo gli Sconti Addizionali in Cascata
                        Sconto = -objContabDettaglio.ScontoMaggiorazioneBase
                        Sconto = Sconto + ((100 - Sconto) / 100 * objContabDettaglio.ScontoAddiz1)
                        Sconto = Sconto + ((100 - Sconto) / 100 * objContabDettaglio.ScontoAddiz2)
                        Sconto = ArrotondaVal_6(Sconto + ((100 - Sconto) / 100 * objContabDettaglio.ScontoAddiz3))
                        objContabDettaglio.ImponibileTotaleNetto = ArrotondaVal_2(objContabDettaglio.ImponibileTotale * (100 - Sconto) / 100)

                        '                     End If


                        'Sottraggo lo Sconto Listino (in cascata)
                        'objContabDettaglio.ImponibileTotaleNetto = ArrotondaVal_2(objContabDettaglio.ImponibileTotale - ((objContabDettaglio.ScontoBaseEuro + objContabDettaglio.ScontoAddizTotaleEuro) * Qta))

                        objContabDettaglio.ScontoAddizTotaleEuro = ArrotondaVal_6(Math.Abs(CDec(objContabDettaglio.ScontoAddizTotaleEuro)))
                        objContabDettaglio.ScontoBaseEuro = ArrotondaVal_6(Math.Abs(CDec(objContabDettaglio.ScontoBaseEuro)))
                        Prezzo_Unitario_Netto = ArrotondaVal_6(objContabDettaglio.ImponibileTotaleNetto / Qta)



                        If objContabDettaglio.ForzaIva = False Then

                            'Iva
                            'If CmbIva.ListIndex <> -1 Then
                            If objContabDettaglio.CodIva <> -1 Then
                                objContabDettaglio.Iva = (objContabDettaglio.ImponibileTotaleNetto / 100) * objContabDettaglio.AliquotaIva
                                objContabDettaglio.Iva = FormattazioneIva(objContabDettaglio.Iva, lavCod, objContabDettaglio)
                            Else
                                objContabDettaglio.Iva = ArrotondaVal_2(0) 'FCI, NI
                            End If

                        End If


                        Iva = objContabDettaglio.Iva

                        objContabDettaglio.ImportoTotale = ArrotondaVal_2(objContabDettaglio.ImponibileTotaleNetto + Iva)

                        If Qta <> 0 Then
                            objContabDettaglio.ImportoUnitario = ArrotondaVal_6(Math.Abs(CDec(objContabDettaglio.ImportoTotale) / Qta))
                        Else
                            objContabDettaglio.ImportoUnitario = ArrotondaVal_2(0)
                        End If

                    Else

                        Prezzo_Unitario_Netto = ArrotondaVal_2(0)
                        objContabDettaglio.ImportoUnitario = ArrotondaVal_2(0)
                        objContabDettaglio.ImportoTotale = ArrotondaVal_2(0)
                        Prezzo_Unitario = ArrotondaVal_2(0)
                        objContabDettaglio.ScontoBaseEuro = ArrotondaVal_2(0)
                        objContabDettaglio.ScontoAddizTotaleEuro = ArrotondaVal_2(0)
                        objContabDettaglio.ImponibileTotaleNetto = ArrotondaVal_2(0)

                    End If



               '#########################################################################################
               '##################### MODALITA' INSERIMENTO = IMPORTO UNITARIO  #########################
               '#########################################################################################

                Case enum_EditImporto.Importo_Unitario

                    'Aggiorno il costo unitario della registrazione
                    FormCorrezioneAutomaticaImporto(objContabDettaglio, lavCod)

                    If Qta > 0 And IsNumeric(objContabDettaglio.ImportoUnitario) Then

                        objContabDettaglio.ImportoTotale = ArrotondaVal_2(Qta * objContabDettaglio.ImportoUnitario)

                        If objContabDettaglio.ForzaIva = False Then

                            'Iva
                            'If CmbIva.ListIndex <> -1 Then
                            If objContabDettaglio.CodIva <> -1 Then
                                objContabDettaglio.Iva = (objContabDettaglio.ImportoTotale / (100 + objContabDettaglio.AliquotaIva)) * objContabDettaglio.AliquotaIva
                                objContabDettaglio.Iva = FormattazioneIva(objContabDettaglio.Iva, lavCod, objContabDettaglio)
                            Else
                                objContabDettaglio.Iva = ArrotondaVal_2(0) 'FCI, NI
                            End If

                        End If

                        Iva = objContabDettaglio.Iva

                        objContabDettaglio.ImponibileTotaleNetto = ArrotondaVal_2(objContabDettaglio.ImportoTotale - Iva)

                        objContabDettaglio.ScontoAddizTotaleEuro = 0 'Inizializzazione
                        Parziale = objContabDettaglio.ImponibileTotaleNetto

                        objContabDettaglio.ScontoAddizTotaleEuro = ArrotondaVal_6(objContabDettaglio.ScontoAddizTotaleEuro + (((Parziale * 100) / (100 - objContabDettaglio.ScontoAddiz3)) - Parziale) / Qta)
                        Parziale = (Parziale * 100) / (100 - objContabDettaglio.ScontoAddiz3)

                        objContabDettaglio.ScontoAddizTotaleEuro = ArrotondaVal_6(objContabDettaglio.ScontoAddizTotaleEuro + (((Parziale * 100) / (100 - objContabDettaglio.ScontoAddiz2)) - Parziale) / Qta)
                        Parziale = (Parziale * 100) / (100 - objContabDettaglio.ScontoAddiz2)

                        objContabDettaglio.ScontoAddizTotaleEuro = ArrotondaVal_6(objContabDettaglio.ScontoAddizTotaleEuro + (((Parziale * 100) / (100 - objContabDettaglio.ScontoAddiz1)) - Parziale) / Qta)
                        Parziale = (Parziale * 100) / (100 - objContabDettaglio.ScontoAddiz1)


                        'Calcolo il prezzo ottenuto dopo lo sconto sul cliente
                        Prezzo_Parziale = objContabDettaglio.ImponibileTotaleNetto + (objContabDettaglio.ScontoAddizTotaleEuro * Qta)

                        'Controllo Overflow
                        If objContabDettaglio.ScontoMaggiorazioneBase = -100 Then
                            objContabDettaglio.ScontoBaseEuro = 0
                            objContabDettaglio.ImponibileTotaleNetto = 0
                        Else
                            objContabDettaglio.ScontoBaseEuro = (((Prezzo_Parziale * 100) / (100 + objContabDettaglio.ScontoMaggiorazioneBase)) - Prezzo_Parziale) / Qta
                        End If

                        'Prezzo_Unitario = ArrotondaVal_6(objContabDettaglio.ImponibileTotale / Qta)
                        Prezzo_Unitario_Netto = ArrotondaVal_6(objContabDettaglio.ImponibileTotaleNetto / Qta)
                        Prezzo_Unitario = ArrotondaVal_6(Prezzo_Unitario_Netto + objContabDettaglio.ScontoBaseEuro + objContabDettaglio.ScontoAddizTotaleEuro)
                        objContabDettaglio.ImponibileTotale = ArrotondaVal_2(Qta * Prezzo_Unitario)

                        objContabDettaglio.ScontoBaseEuro = ArrotondaVal_6(Math.Abs(CDec(objContabDettaglio.ScontoBaseEuro)))

                        'Ricalcolo iva (inizialmente era calcolata sul costo già arrotondato a 2 cifre e quindi ci possono essere differenze sul cambio del modo di calcolo
                        If objContabDettaglio.ForzaIva = False Then
                            objContabDettaglio.Iva = FormattazioneIva((objContabDettaglio.ImponibileTotaleNetto / 100) * objContabDettaglio.AliquotaIva, lavCod, objContabDettaglio)
                        End If

                        Iva = objContabDettaglio.Iva

                    ElseIf Qta <> 0 Then

                        Prezzo_Unitario_Netto = ArrotondaVal_2(0)
                        objContabDettaglio.ImponibileTotale = ArrotondaVal_2(0)
                        objContabDettaglio.ImponibileTotaleNetto = ArrotondaVal_2(0)
                        Prezzo_Unitario = ArrotondaVal_2(0)
                        objContabDettaglio.ScontoBaseEuro = ArrotondaVal_2(0)
                        objContabDettaglio.ScontoAddizTotaleEuro = ArrotondaVal_2(0)
                        objContabDettaglio.ImportoUnitario = ArrotondaVal_2(0)

                    Else
                        objContabDettaglio.ImportoTotale = ArrotondaVal_2(0)
                    End If

            End Select

            'Indico lo sconto totale (base + addizionali) calcolato
            objContabDettaglio.ScontoCalcolato = ArrotondaVal_6(AgronicaCoreContabHLP.Contabilita.CalcolaScontoComplessivo(
                objContabDettaglio.ScontoBase,
                objContabDettaglio.ScontoAddiz1,
                objContabDettaglio.ScontoAddiz2,
                objContabDettaglio.ScontoAddiz3))

            'TODO: Indico lo sconto totale (base + addizionali) in euro 
            objContabDettaglio.ScontoCalcolatoEuro = If(objContabDettaglio.ScontoBaseEuro, 0) + If(objContabDettaglio.ScontoAddizTotaleEuro, 0)

            '#########################################################################################
            '#####################            IVA          ###########################################
            '#########################################################################################

            If objContabDettaglio.ForzaIva = False Then
                objContabDettaglio.Iva = FormattazioneIva(Iva, lavCod, objContabDettaglio)
            End If

            Select Case objContabDettaglio.IvaModalita

                Case enum_ModalitaIva.Nessuno

                    objContabDettaglio.IvaIndetraibilePerc = ArrotondaVal_2(0)
                    objContabDettaglio.IvaIndetraibile = ArrotondaVal_2(0)

                Case enum_ModalitaIva.Indetraibile

                    If IsNumeric(objContabDettaglio.IvaIndetraibilePerc) Then
                        'Iva in compensazione calcolata sull'imposta
                        objContabDettaglio.IvaIndetraibile = ArrotondaVal_4((Iva / 100) * objContabDettaglio.IvaIndetraibilePerc)
                    Else
                        objContabDettaglio.IvaIndetraibilePerc = ArrotondaVal_2(0)
                        objContabDettaglio.IvaIndetraibile = ArrotondaVal_2(0)
                    End If

                    objContabDettaglio.IvaCreditoDebito = ArrotondaVal_2(Iva - objContabDettaglio.IvaIndetraibile)

                Case enum_ModalitaIva.Compensazione

                    If IsNumeric(objContabDettaglio.IvaIndetraibilePerc) Then

                        Select Case objContabDettaglio.ScontoModalita

                            Case enModalitaSconto.Omaggio_ConRivalsaIva

                                'Iva in compensazione calcolata sull'imponibile netto
                                objContabDettaglio.IvaIndetraibile = ArrotondaVal_4(((Qta * Prezzo_Unitario) / 100) * objContabDettaglio.IvaIndetraibilePerc)

                            Case Else

                                'Iva in compensazione calcolata sull'imponibile netto
                                objContabDettaglio.IvaIndetraibile = ArrotondaVal_4((objContabDettaglio.ImponibileTotaleNetto / 100) * objContabDettaglio.IvaIndetraibilePerc)

                        End Select

                    Else

                        objContabDettaglio.IvaIndetraibilePerc = ArrotondaVal_2(0)
                        objContabDettaglio.IvaIndetraibile = ArrotondaVal_2(0)

                    End If

                    objContabDettaglio.IvaCreditoDebito = ArrotondaVal_2(Iva - objContabDettaglio.IvaIndetraibile)

            End Select


            '=====================================================================================================================
            'Aggiornamento Importo in caso di SplitPayment --> sezione con esigibilità = 3 e modalità sconto = percentuale
            '---------------------------------------------------------------------------------------------------------------------
            If objContabDettaglio.ScontoModalita = enModalitaSconto.Percentuale AndAlso objContabDettaglio.EsigibilitaIva = 3 Then

                objContabDettaglio.ImportoTotale = objContabDettaglio.ImponibileTotaleNetto

                If Qta <> 0 Then
                    objContabDettaglio.ImportoUnitario = ArrotondaVal_2(objContabDettaglio.ImportoTotale / Qta)
                Else
                    objContabDettaglio.ImportoUnitario = 0
                End If

            End If



            '=====================================================================================================================
            'Calcolo Provvigione
            '---------------------------------------------------------------------------------------------------------------------
            'Agente
            If objContabDettaglio.ProvvigionePercAgente <> 0 Then
                objContabDettaglio.ProvvigioneAgente = ArrotondaVal_2((objContabDettaglio.ImponibileTotaleNetto / 100) * objContabDettaglio.ProvvigionePercAgente)
            Else
                objContabDettaglio.ProvvigioneAgente = ArrotondaVal_2(0)
            End If

            'Capo Area
            If objContabDettaglio.ProvvigionePercCapoArea <> 0 Then
                objContabDettaglio.ProvvigioneCapoArea = ArrotondaVal_2((objContabDettaglio.ImponibileTotaleNetto / 100) * objContabDettaglio.ProvvigionePercCapoArea)
            Else
                objContabDettaglio.ProvvigioneCapoArea = ArrotondaVal_2(0)
            End If

            '=====================================================================================================================

            'Aggiornamento TxtBox
            Select Case objContabDettaglio.PrezzoRiferitoA

                Case enum_PrezzoLivello.Kg_Litri, enum_PrezzoLivello.Udm_principale, enum_PrezzoLivello.Confezione

                    If objContabDettaglio.PrezzoRiferitoA <> enum_PrezzoLivello.Kg_Litri Then

                        If Prezzo_Unitario <> Replace(Agro_SQL_SaveNum(objContabDettaglio.Prezzo), ".", ",") Then 'And ArrotondaVal_2((Qta) * Replace(Agro_SQL_SaveNum(objContabDettaglio.Prezzo), ".", ",")) <> objContabDettaglio.ImponibileTotale Then
                            objContabDettaglio.Prezzo = ArrotondaVal_6(Prezzo_Unitario)
                        End If

                        'If Not bInizializzazione AndAlso
                        If (Prezzo_Unitario <> Replace(Agro_SQL_SaveNum(objContabDettaglio.Prezzo), ".", ",") OrElse
                            objContabDettaglio.PrezzoRiferitoA <> enum_PrezzoLivello.Kg_Litri) Then

                            If kgNettiPesoPagamento <> 0 Then
                                objContabDettaglio.PrezzoEffettivoKgL = ArrotondaVal_6(objContabDettaglio.ImponibileTotale / kgNettiPesoPagamento)
                            Else
                                objContabDettaglio.PrezzoEffettivoKgL = ArrotondaVal_6(0)
                            End If

                        End If
                        objContabDettaglio.PrezzoNetto = ArrotondaVal_6(Prezzo_Unitario_Netto)

                    End If


                Case enum_PrezzoLivello.Contenitore, enum_PrezzoLivello.Imballo

                    If Prezzo_Unitario <> Replace(Agro_SQL_SaveNum(objContabDettaglio.Prezzo), ".", ",") Then

                        If IsNumeric(objContabDettaglio.ImponibileTotale) AndAlso Qta <> 0 Then
                            objContabDettaglio.Prezzo = ArrotondaVal_6(objContabDettaglio.ImponibileTotale / Qta)
                        Else
                            objContabDettaglio.Prezzo = ArrotondaVal_6(0)
                        End If

                    End If

                    'Not bInizializzazione AndAlso
                    If (Prezzo_Unitario <> Replace(Agro_SQL_SaveNum(objContabDettaglio.Prezzo), ".", ",") OrElse
                        objContabDettaglio.PrezzoRiferitoA <> enum_PrezzoLivello.Kg_Litri) Then

                        If kgNettiPesoPagamento <> 0 AndAlso IsNumeric(objContabDettaglio.ImponibileTotale) Then
                            objContabDettaglio.PrezzoEffettivoKgL = ArrotondaVal_6(objContabDettaglio.ImponibileTotale / kgNettiPesoPagamento)
                        Else
                            objContabDettaglio.PrezzoEffettivoKgL = ArrotondaVal_6(0)
                        End If

                    End If

                    objContabDettaglio.PrezzoNetto = ArrotondaVal_6(Prezzo_Unitario_Netto)

            End Select

            'TODO: ?????
            If Not conferimentoPomodoro Then
                objContabDettaglio.Prezzo = Prezzo_Unitario
                objContabDettaglio.PrezzoNetto = Prezzo_Unitario_Netto
            End If

            'FormLivelloPrezzo

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]"))
        End Try

    End Sub

    Private Shared Sub FormCorrezioneAutomaticaImporto(ByRef objContabDettaglio As Contabilita_Riga,
                                                       ByVal lavCod As Integer)
        Dim imponibile As Decimal
        Dim importo As Decimal
        Dim iva As Decimal
        Dim differenza As Decimal

        'If Not bInizializzazione AndAlso Not mbLetturaDati AndAlso Not TimerGiacenze.enabled Then

        If (objContabDettaglio.ValoreRiferimentoPrezzo = enum_EditImporto.Importo OrElse
            objContabDettaglio.ValoreRiferimentoPrezzo = enum_EditImporto.Importo_Unitario) AndAlso
           (IsNumeric(objContabDettaglio.Quantita) AndAlso
            IsNumeric(objContabDettaglio.ImponibileTotaleNetto) AndAlso
            IsNumeric(objContabDettaglio.Iva) AndAlso
            IsNumeric(objContabDettaglio.ImportoTotale)) Then

            'Nel Caso di impostazione da importo l'imponibile+iva matematicamente potrebbe non coincidere con l'importo --> forzo l'iva manualmente
            Select Case lavCod

                Case 1069, 1020, 1053 'DDT Corrispettivi, Scontrini, Ricevute

                    If objContabDettaglio.ForzaIva = False Then
                        objContabDettaglio.LblAsteriscoImportoUnitario = ""
                        objContabDettaglio.LblAsteriscoImportoTotale = ""
                    End If

                    importo = objContabDettaglio.ImportoTotale
                    imponibile = objContabDettaglio.ImponibileTotaleNetto
                    iva = ArrotondaVal_2(objContabDettaglio.Iva)
                    differenza = ArrotondaVal_2(importo - (imponibile + iva))

                    If differenza <> 0 AndAlso objContabDettaglio.Quantita <> 0 Then

                        'Controllo soluzione
                        If differenza > 0 Then

                            'Importo inferiore --> aumento l'iva
                            objContabDettaglio.ForzaIva = True 'ssChecked
                            'TODO: il lan al cambio ForzaIva richiamava FormAggiornaCosto e FormCorrezioneAutomaticaImporto
                            objContabDettaglio.Iva = iva + differenza

                            AggiornaDettagliEconomici(objContabDettaglio, lavCod)

                        Else

                            'Importo superiore
                            objContabDettaglio.ForzaIva = True 'ssChecked
                            'TODO: il lan al cambio ForzaIva richiamava FormAggiornaCosto e FormCorrezioneAutomaticaImporto
                            objContabDettaglio.Iva = ArrotondaVal_2(objContabDettaglio.Iva)

                            AggiornaDettagliEconomici(objContabDettaglio, lavCod)

                        End If

                        If objContabDettaglio.ValoreRiferimentoPrezzo = enum_EditImporto.Importo Then
                            objContabDettaglio.LblAsteriscoImportoTotale = "(*) E' stata impostata una variazione automatica all'iva"
                            'objContabDettaglio.LblAsteriscoImportoTotale.Caption = "*"
                        Else
                            objContabDettaglio.LblAsteriscoImportoUnitario = "(*) E' stata impostata una variazione automatica all'iva"
                            'objContabDettaglio.LblAsteriscoImportoUnitario.Caption = "*"
                        End If

                    Else

                        'Formattazione a 2 decimali per evitare casini (es. ricevute ibride con iva manuale e non formattata a 4)
                        objContabDettaglio.Iva = ArrotondaVal_2(objContabDettaglio.Iva)

                    End If


                Case Else

                    objContabDettaglio.LblAsteriscoImportoUnitario = ""
                    objContabDettaglio.LblAsteriscoImportoTotale = ""

                    If objContabDettaglio.ForzaIva = False Then

                        importo = objContabDettaglio.ImportoTotale
                        imponibile = objContabDettaglio.ImponibileTotaleNetto
                        iva = ArrotondaVal_2(objContabDettaglio.Iva)
                        differenza = ArrotondaVal_2(importo - (imponibile + iva))

                        If differenza <> 0 AndAlso objContabDettaglio.Quantita <> 0 Then

                            If objContabDettaglio.ValoreRiferimentoPrezzo = enum_EditImporto.Importo Then
                                objContabDettaglio.LblAsteriscoImportoTotale = "(!) La somma imponibile+iva non può coincidere con l'importo totale impostato. Fare riferimento alla sezione castelletto iva del documento."
                                'objContabDettaglio.LblAsteriscoImportoTotale.Caption = "!"
                            Else
                                objContabDettaglio.LblAsteriscoImportoUnitario = "(!) La somma imponibile+iva non può coincidere con l'importo unitario impostato. Fare riferimento alla sezione castelletto iva del documento."
                                'objContabDettaglio.LblAsteriscoImportoUnitario.Caption = "!"
                            End If

                        End If

                    End If

            End Select

        Else
            objContabDettaglio.LblAsteriscoImportoUnitario = ""
            objContabDettaglio.LblAsteriscoImportoTotale = ""
        End If

        'Else
        '    objContabDettaglio.LblAsteriscoImportoUnitario = ""
        '    objContabDettaglio.LblAsteriscoImportoTotale = ""
        'End If

    End Sub

    Private Shared Function FormattazioneIva(ByVal iva As Decimal,
                                             ByVal lavCod As Integer,
                                             ByRef objContabDettaglio As Contabilita_Riga
                                             ) As Decimal

        'Inizializzazione
        Dim ivaFormattata As Decimal = ArrotondaVal_4(iva)

        If objContabDettaglio.ForzaIva = False AndAlso
           (objContabDettaglio.ValoreRiferimentoPrezzo = enum_EditImporto.Importo OrElse
            objContabDettaglio.ValoreRiferimentoPrezzo = enum_EditImporto.Importo_Unitario) Then

            Select Case lavCod

                Case LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_VENDITA, LAVCOD_RICEVUTA_EMESSA '1020=Scontrini

                    ivaFormattata = ArrotondaVal_2(iva)

            End Select

        End If

        Return ivaFormattata

    End Function

    Public Shared Sub CalcolaScontoAddizionaleCascata(ByRef objContabDettaglio As Contabilita_Riga)

        Const nomeRoutine = "UtilityHelper.CalcolaScontoAddizionaleCascata"

        Try

            Dim scontoAddizionaleTotale As Decimal = AgronicaCoreContabHLP.Contabilita.CalcolaScontoComplessivo(
                objContabDettaglio.ScontoAddiz1,
                objContabDettaglio.ScontoAddiz2,
                objContabDettaglio.ScontoAddiz3)

            Dim scontoTesto As String = AgronicaCoreContabHLP.Contabilita.ComponiScontoAddizionaleTesto(
                objContabDettaglio.ScontoAddiz1,
                objContabDettaglio.ScontoAddiz2,
                objContabDettaglio.ScontoAddiz3)

            'If objContabDettaglio.ScontoAddiz1 IsNot Nothing Then
            '    scontoAddizionaleTotale += objContabDettaglio.ScontoAddiz1
            '    If objContabDettaglio.ScontoAddiz1 <> 0 Then
            '        scontoTesto &= If(Trim(scontoTesto) = "", "", "-") & CStr(objContabDettaglio.ScontoAddiz1)
            '    End If
            'End If

            'If objContabDettaglio.ScontoAddiz2 IsNot Nothing Then
            '    scontoAddizionaleTotale += ((100 - scontoAddizionaleTotale) * objContabDettaglio.ScontoAddiz2) / 100
            '    If objContabDettaglio.ScontoAddiz2 <> 0 Then
            '        scontoTesto &= If(Trim(scontoTesto) = "", "", "-") & CStr(objContabDettaglio.ScontoAddiz2)
            '    End If
            'End If

            'If objContabDettaglio.ScontoAddiz3 IsNot Nothing Then
            '    scontoAddizionaleTotale += ((100 - scontoAddizionaleTotale) * objContabDettaglio.ScontoAddiz3) / 100
            '    If objContabDettaglio.ScontoAddiz3 <> 0 Then
            '        scontoTesto &= If(Trim(scontoTesto) = "", "", "-") & CStr(objContabDettaglio.ScontoAddiz3)
            '    End If
            'End If

            objContabDettaglio.ScontoAddizTotalePerc = -1 * ArrotondaVal_6(scontoAddizionaleTotale)  'Salvo il negativo analogamente lo sconto
            objContabDettaglio.ScontoAddizTesto = scontoTesto

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

    End Sub


    'Public Shared Sub AggiornaDettaglioEconomico(ByRef objContabDettaglio As Movimento_Dettaglio)

    '    Const nomeRoutine = "UtilityHelper.AggiornaDettaglioEconomico"
    '    Dim Qta As Decimal
    '    Dim Prezzo_Parziale As Decimal
    '    Dim Sconto_Totale As Decimal
    '    Dim Parziale As Decimal
    '    Dim Prezzo_Unitario As Decimal
    '    Dim Prezzo_Unitario_Netto As Decimal
    '    Dim bBypass As Boolean
    '    Dim Iva_Manuale As Decimal
    '    Dim Iva As Decimal
    '    Dim bCalcoloSconto As Boolean
    '    Dim Sconto As Decimal
    '    Dim kgDegrado As Decimal
    '    Dim kgNettiPesoPagamento As Decimal

    '    Try

    '        '#########################################################################################
    '        '##################### MODALITA' INSERIMENTO = PREZZO UNITARIO  ##########################
    '        '#########################################################################################

    '        'Aggiorno il costo complessivo della registrazione
    '        If IsNumeric(Qta) And IsNumeric(Prezzo_Unitario) Then

    '            Prezzo_Unitario_Netto = Prezzo_Unitario + ((Prezzo_Unitario / 100) * objContabDettaglio.Sconto)
    '            'objContabDettaglio.ScontoBaseEuro = Prezzo_Unitario_Netto - Prezzo_Unitario

    '            ''Sottraggo gli Sconti Addizionali in Cascata
    '            'Prezzo_Unitario_Netto = ArrotondaVal_6(Prezzo_Unitario_Netto - ((Prezzo_Unitario_Netto / 100) * objContabDettaglio.ScontoAddiz1))
    '            'Prezzo_Unitario_Netto = ArrotondaVal_6(Prezzo_Unitario_Netto - ((Prezzo_Unitario_Netto / 100) * objContabDettaglio.ScontoAddiz2))
    '            'Prezzo_Unitario_Netto = ArrotondaVal_6(Prezzo_Unitario_Netto - ((Prezzo_Unitario_Netto / 100) * objContabDettaglio.ScontoAddiz3))


    '            'objContabDettaglio.ScontoAddizTotaleEuro = ArrotondaVal_6(Math.Abs(Prezzo_Unitario - Prezzo_Unitario_Netto + CDec(objContabDettaglio.ScontoBaseEuro)))
    '            'objContabDettaglio.ScontoBaseEuro = ArrotondaVal_6(Math.Abs(CDec(objContabDettaglio.ScontoBaseEuro)))

    '            bCalcoloSconto = False

    '            Select Case objContabDettaglio.Sconto_Modalita

    '                Case enModalitaSconto.Percentuale

    '                    objContabDettaglio.Imponibile = ArrotondaVal_2(Qta * Prezzo_Unitario)
    '                    'objContabDettaglio.Imponibile_Netto = ArrotondaVal_2(Qta * Prezzo_Unitario_Netto)
    '                    bCalcoloSconto = True

    '                Case enModalitaSconto.Omaggio_SenzaRivalsaIva

    '                    objContabDettaglio.Imponibile = ArrotondaVal_2(Qta * Prezzo_Unitario)
    '                    objContabDettaglio.Imponibile_Netto = objContabDettaglio.Imponibile 'ArrotondaVal_2(Qta * Prezzo_Unitario)
    '                    bCalcoloSconto = False

    '                Case enModalitaSconto.Sconto_Merce, enModalitaSconto.Campioni_Gratuiti

    '                    objContabDettaglio.Imponibile = ArrotondaVal_2(Qta * Prezzo_Unitario)
    '                    objContabDettaglio.Imponibile_Netto = ArrotondaVal_2(0)

    '                Case enModalitaSconto.Omaggio_ConRivalsaIva

    '                    objContabDettaglio.Imponibile = ArrotondaVal_2(Qta * Prezzo_Unitario)
    '                    objContabDettaglio.Imponibile_Netto = objContabDettaglio.Imponibile 'ArrotondaVal_2(Qta * Prezzo_Unitario)
    '                    bCalcoloSconto = False

    '            End Select


    '            If bCalcoloSconto Then

    '                ''Sottraggo gli Sconti Addizionali in Cascata
    '                'Sconto = -objContabDettaglio.ScontoMaggiorazioneBase
    '                'Sconto = Sconto + ((100 - Sconto) / 100 * objContabDettaglio.ScontoAddiz1)
    '                'Sconto = Sconto + ((100 - Sconto) / 100 * objContabDettaglio.ScontoAddiz2)
    '                'Sconto = ArrotondaVal_6(Sconto + ((100 - Sconto) / 100 * objContabDettaglio.ScontoAddiz3))
    '                'objContabDettaglio.Imponibile_Netto = ArrotondaVal_2(objContabDettaglio.Imponibile * (100 - Sconto) / 100)

    '            End If

    '            If objContabDettaglio.ChkIva_Manuale = 0 Then

    '                'Iva
    '                'If CmbIva.ListIndex <> -1 Then
    '                If objContabDettaglio.Cod_Iva <> -1 Then
    '                    objContabDettaglio.Iva = (objContabDettaglio.Imponibile_Netto / 100) * Aliquota_Iva
    '                Else
    '                    objContabDettaglio.Iva = 0
    '                End If

    '                objContabDettaglio.Iva = ArrotondaVal_4(objContabDettaglio.Iva)

    '            End If

    '            Iva = objContabDettaglio.Iva

    '            Select Case objContabDettaglio.Sconto_Modalita

    '                Case enModalitaSconto.Percentuale

    '                    'objContabDettaglio.ImportoTotale = ArrotondaVal_2(objContabDettaglio.Imponibile_Netto + Iva)

    '                Case enModalitaSconto.Omaggio_ConRivalsaIva

    '                    'Paga solo l'iva
    '                    objContabDettaglio.ImportoTotale = ArrotondaVal_2(Iva)

    '                Case Else

    '                    objContabDettaglio.ImportoTotale = ArrotondaVal_2(0)

    '            End Select

    '            'If Qta <> 0 Then
    '            '    objContabDettaglio.ImportoUnitario = ArrotondaVal_6(Math.Abs(CDec(objContabDettaglio.ImportoTotale) / Qta))

    '            '    'Aggiustamento Segno Prezzo Unitario
    '            '    If objContabDettaglio.ImportoTotale < 0 Then
    '            '        objContabDettaglio.ImportoUnitario = ArrotondaVal_6(-objContabDettaglio.ImportoUnitario)
    '            '    End If

    '            'Else
    '            '    objContabDettaglio.ImportoUnitario = ArrotondaVal_2(0)
    '            'End If

    '        Else

    '            objContabDettaglio.Imponibile_Netto = ArrotondaVal_2(0)
    '            'objContabDettaglio.ScontoBaseEuro = ArrotondaVal_2(0)
    '            'objContabDettaglio.ScontoAddizTotaleEuro = ArrotondaVal_2(0)
    '            Prezzo_Unitario_Netto = ArrotondaVal_2(0)
    '            'objContabDettaglio.ImportoUnitario = ArrotondaVal_2(0)
    '            'objContabDettaglio.ImportoTotale = ArrotondaVal_2(0)
    '            objContabDettaglio.Imponibile = ArrotondaVal_2(0)

    '        End If




    '        ''Indico lo sconto totale (base + addizionali) calcolato
    '        'objContabDettaglio.ScontoCalcolato = ArrotondaVal_6(AgronicaCoreContabHLP.Contabilita.CalcolaScontoComplessivo(
    '        '    objContabDettaglio.ScontoBase,
    '        '    objContabDettaglio.ScontoAddiz1,
    '        '    objContabDettaglio.ScontoAddiz2,
    '        '    objContabDettaglio.ScontoAddiz3))

    '        'TODO: Indico lo sconto totale (base + addizionali) in euro 
    '        'objContabDettaglio.ScontoCalcolatoEuro = If(objContabDettaglio.ScontoBaseEuro, 0) + If(objContabDettaglio.ScontoAddizTotaleEuro, 0)

    '        '#########################################################################################
    '        '#####################            IVA          ###########################################
    '        '#########################################################################################

    '        If objContabDettaglio.ChkIva_Manuale = 0 Then
    '            objContabDettaglio.Iva = FormattazioneIva(Iva, objContabDettaglio.Lav_Cod, objContabDettaglio)
    '        End If

    '        'Select Case objContabDettaglio.Cod_IvaIndetraibile

    '        '    Case 0

    '        '        objContabDettaglio.IvaIndetraibilePerc = ArrotondaVal_2(0)
    '        '        objContabDettaglio.IvaIndetraibile = ArrotondaVal_2(0)

    '        '    Case else

    '        '        If IsNumeric(objContabDettaglio.IvaIndetraibilePerc) Then
    '        '            'Iva in compensazione calcolata sull'imposta
    '        '            objContabDettaglio.IvaIndetraibile = ArrotondaVal_4((Iva / 100) * objContabDettaglio.IvaIndetraibilePerc)
    '        '        Else
    '        '            objContabDettaglio.IvaIndetraibilePerc = ArrotondaVal_2(0)
    '        '            objContabDettaglio.IvaIndetraibile = ArrotondaVal_2(0)
    '        '        End If

    '        '        objContabDettaglio.IvaCreditoDebito = ArrotondaVal_2(Iva - objContabDettaglio.IvaIndetraibile)

    '        '    Case enum_ModalitaIva.Compensazione

    '        '        If IsNumeric(objContabDettaglio.IvaIndetraibilePerc) Then

    '        '            Select Case objContabDettaglio.ScontoModalita

    '        '                Case enModalitaSconto.Omaggio_ConRivalsaIva

    '        '                    'Iva in compensazione calcolata sull'imponibile netto
    '        '                    objContabDettaglio.IvaIndetraibile = ArrotondaVal_4(((Qta * Prezzo_Unitario) / 100) * objContabDettaglio.IvaIndetraibilePerc)

    '        '                Case Else

    '        '                    'Iva in compensazione calcolata sull'imponibile netto
    '        '                    objContabDettaglio.IvaIndetraibile = ArrotondaVal_4((objContabDettaglio.Imponibile_Netto / 100) * objContabDettaglio.IvaIndetraibilePerc)

    '        '            End Select

    '        '        Else

    '        '            objContabDettaglio.IvaIndetraibilePerc = ArrotondaVal_2(0)
    '        '            objContabDettaglio.IvaIndetraibile = ArrotondaVal_2(0)

    '        '        End If

    '        '        objContabDettaglio.IvaCreditoDebito = ArrotondaVal_2(Iva - objContabDettaglio.IvaIndetraibile)

    '        'End Select


    '        ''=====================================================================================================================
    '        ''Aggiornamento Importo in caso di SplitPayment --> sezione con esigibilità = 3 e modalità sconto = percentuale
    '        ''---------------------------------------------------------------------------------------------------------------------
    '        'If objContabDettaglio.ScontoModalita = enModalitaSconto.Percentuale AndAlso objContabDettaglio.EsigibilitaIva = 3 Then

    '        '    objContabDettaglio.ImportoTotale = objContabDettaglio.Imponibile_Netto

    '        '    If Qta <> 0 Then
    '        '        objContabDettaglio.ImportoUnitario = ArrotondaVal_2(objContabDettaglio.ImportoTotale / Qta)
    '        '    Else
    '        '        objContabDettaglio.ImportoUnitario = 0
    '        '    End If

    '        'End If



    '        ''=====================================================================================================================
    '        ''Calcolo Provvigione
    '        ''---------------------------------------------------------------------------------------------------------------------
    '        ''Agente
    '        'If objContabDettaglio.ProvvigionePercAgente <> 0 Then
    '        '    objContabDettaglio.ProvvigioneAgente = ArrotondaVal_2((objContabDettaglio.Imponibile_Netto / 100) * objContabDettaglio.ProvvigionePercAgente)
    '        'Else
    '        '    objContabDettaglio.ProvvigioneAgente = ArrotondaVal_2(0)
    '        'End If

    '        ''Capo Area
    '        'If objContabDettaglio.ProvvigionePercCapoArea <> 0 Then
    '        '    objContabDettaglio.ProvvigioneCapoArea = ArrotondaVal_2((objContabDettaglio.Imponibile_Netto / 100) * objContabDettaglio.ProvvigionePercCapoArea)
    '        'Else
    '        '    objContabDettaglio.ProvvigioneCapoArea = ArrotondaVal_2(0)
    '        'End If

    '        ''=====================================================================================================================

    '        ''Aggiornamento TxtBox
    '        'Select Case objContabDettaglio.PrezzoRiferitoA

    '        '    Case enum_PrezzoLivello.Kg_Litri, enum_PrezzoLivello.Udm_principale, enum_PrezzoLivello.Confezione

    '        '        If objContabDettaglio.PrezzoRiferitoA <> enum_PrezzoLivello.Kg_Litri Then

    '        '            If Prezzo_Unitario <> Replace(Agro_SQL_SaveNum(objContabDettaglio.Prezzo), ".", ",") Then 'And ArrotondaVal_2((Qta) * Replace(Agro_SQL_SaveNum(objContabDettaglio.Prezzo), ".", ",")) <> objContabDettaglio.Imponibile Then
    '        '                objContabDettaglio.Prezzo = ArrotondaVal_6(Prezzo_Unitario)
    '        '            End If

    '        '            'If Not bInizializzazione AndAlso
    '        '            If (Prezzo_Unitario <> Replace(Agro_SQL_SaveNum(objContabDettaglio.Prezzo), ".", ",") OrElse
    '        '                objContabDettaglio.PrezzoRiferitoA <> enum_PrezzoLivello.Kg_Litri) Then

    '        '                If kgNettiPesoPagamento <> 0 Then
    '        '                    objContabDettaglio.PrezzoEffettivoKgL = ArrotondaVal_6(objContabDettaglio.Imponibile / kgNettiPesoPagamento)
    '        '                Else
    '        '                    objContabDettaglio.PrezzoEffettivoKgL = ArrotondaVal_6(0)
    '        '                End If

    '        '            End If
    '        '            objContabDettaglio.PrezzoNetto = ArrotondaVal_6(Prezzo_Unitario_Netto)

    '        '        End If


    '        '    Case enum_PrezzoLivello.Contenitore, enum_PrezzoLivello.Imballo

    '        '        If Prezzo_Unitario <> Replace(Agro_SQL_SaveNum(objContabDettaglio.Prezzo), ".", ",") Then

    '        '            If IsNumeric(objContabDettaglio.Imponibile) AndAlso Qta <> 0 Then
    '        '                objContabDettaglio.Prezzo = ArrotondaVal_6(objContabDettaglio.Imponibile / Qta)
    '        '            Else
    '        '                objContabDettaglio.Prezzo = ArrotondaVal_6(0)
    '        '            End If

    '        '        End If

    '        '        'Not bInizializzazione AndAlso
    '        '        If (Prezzo_Unitario <> Replace(Agro_SQL_SaveNum(objContabDettaglio.Prezzo), ".", ",") OrElse
    '        '            objContabDettaglio.PrezzoRiferitoA <> enum_PrezzoLivello.Kg_Litri) Then

    '        '            If kgNettiPesoPagamento <> 0 AndAlso IsNumeric(objContabDettaglio.Imponibile) Then
    '        '                objContabDettaglio.PrezzoEffettivoKgL = ArrotondaVal_6(objContabDettaglio.Imponibile / kgNettiPesoPagamento)
    '        '            Else
    '        '                objContabDettaglio.PrezzoEffettivoKgL = ArrotondaVal_6(0)
    '        '            End If

    '        '        End If

    '        '        objContabDettaglio.PrezzoNetto = ArrotondaVal_6(Prezzo_Unitario_Netto)

    '        'End Select



    '    Catch ex As Exception
    '        Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]"))
    '    End Try

    'End Sub




#End Region

#Region "Aggiornamento totali documento"

    Public Shared Function AggiornaConteggiTotali(ByVal piva As String,
                                                  ByVal lavCod As Integer,
                                                  ByVal idAgenda As Integer,
                                                  ByRef objParametriServer As AgronicaCoreParametri,
                                                  Optional ByVal idMovT As Integer = 0
                                                  ) As Contabilita_Totali_Testata

        Const nomeRoutine = "UtilityHelper.AggiornaConteggiTotali()"
        Dim conteggiTotali As Contabilita_Totali_Testata = Nothing

        Try
            Dim dtTotali As DataTable = Nothing

            If Not IsMovimentoMagazzino(lavCod) Then
                Dim objContabR As New AgronicaCoreContabDAL.Contabilita_R
                dtTotali = objContabR.ConteggioTotaliDocumento(piva, lavCod, idAgenda,
                                                                            "", objParametriServer)
            End If

            If Not dtTotali Is Nothing AndAlso dtTotali.Rows.Count > 0 Then

                If dtTotali.Rows.Count > 1 Then
                    Throw New Exception(String.Format("Sono presenti {0} righe di Conteggio Totali", dtTotali.Rows.Count))
                End If

                'Devo Aggiornare

                Dim newNumColli As Decimal = CDec(dtTotali.Rows(0).Item("Num_Colli_Auto_Tot"))
                Dim newPesoNettoProd As Decimal = CDec(dtTotali.Rows(0).Item("Netto_Prod_Tot"))
                Dim newTaraImballi As Decimal = CDec(dtTotali.Rows(0).Item("Tara_Prod_Tot"))
                Dim newImballiVuoti As Decimal = CDec(dtTotali.Rows(0).Item("Imballi_Vuoti_Tot"))
                Dim newTaraVeicolo As Decimal = CDec(dtTotali.Rows(0).Item("Tara_Veicolo_DB"))
                Dim pesoLordoDocDb As Decimal = CDec(dtTotali.Rows(0).Item("Peso_DB"))

                Dim newPesoLordoProd As Decimal = newPesoNettoProd + newTaraImballi

                'Se accettazione non devo ricalcolare il peso lordo totale del documento
                Dim newPesoLordoDoc As Decimal? = Nothing
                If IsAccettazione(lavCod) Then
                    newPesoLordoDoc = pesoLordoDocDb
                Else
                    newPesoLordoDoc = newPesoLordoProd + newImballiVuoti + newTaraVeicolo
                End If


                If idMovT = 0 Then

                    'arrivo dalla cancellazione e non ho questo valore ==> lo trovo da DB
                    Dim objMovR As New AgronicaCoreContabDAL.Movimenti_R
                    idMovT = objMovR.IdMov_from_IdAgendaCauMov(piva, idAgenda, CAU_REGISTRAZIONI, "", objParametriServer)

                End If


                '=============================================================================================================================
                'Marco: Ricalcolo Castelletto e Totali
                '-----------------------------------------------------------------------------------------------------------------------------
                Dim Variazioni As Decimal = 0
                Dim Imposta As Decimal = 0
                Dim newimporto As Decimal = 0
                Dim Imponibile_Netto As Decimal = 0
                Dim Imponibile_Lordo As Decimal = 0
                Dim listCastelletto As List(Of Contabilita_Castelletto_Iva) = Nothing
                Dim objMovimenti_DettagliHelper As New Agenda_Movimenti_Dettagli_Helper
                Dim objMovDets As List(Of Movimento_Dettaglio)
                Dim utilityHelper As New AgronicaCoreModello.UtilityHelper
                Dim dt As New DataTable
                Dim dr As DataRow
                Dim RiepilogoImporti As Contabilita_Riepilogo_Importi
                Dim RiepilogoCastelletto As String = ""

                'Lettura Dettagli
                objMovDets = objMovimenti_DettagliHelper.Leggi(piva, 0, idAgenda, 0, 0, objParametriServer)

                objMovDets = objMovDets.Where(Function(det) Not {RIGA_IMBALLI_CONTENTI_PRODOTTI, RIGA_IMBALLI_VUOTI_IN_ENTRATA}.Contains(det.Ordine_Det)).ToList()

                newimporto = utilityHelper.Calcola_Importo_Documento(Imponibile_Netto,
                                                                      Imponibile_Lordo,
                                                                      Variazioni,
                                                                      Imposta,
                                                                      listCastelletto,
                                                                      lavCod,
                                                                      0,
                                                                      objMovDets,
                                                                      objParametriServer)



                dt.Columns.Add(New DataColumn("Cod_Iva", GetType(Integer)))
                dt.Columns.Add(New DataColumn("Imponibile", GetType(Decimal)))
                dt.Columns.Add(New DataColumn("Aliquota", GetType(String)))
                dt.Columns.Add(New DataColumn("Imposta", GetType(Decimal)))


                Dim objContabHelper As New AgronicaCoreContabHLP.Contabilita

                If listCastelletto.Count > 0 Then

                    For Each Riga As Contabilita_Castelletto_Iva In listCastelletto

                        dr = dt.NewRow

                        'Select Case lavCod

                        '    Case LAVCOD_ORDINE_ACQUISTO, LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA  'RICEVIMENTO

                        '        dr("Imponibile") = -Riga.Imponibile_Netto

                        '    Case Else

                        '        dr("Imponibile") = Riga.Imponibile_Netto


                        'End Select

                        dr("Imponibile") = Riga.Imponibile_Netto
                        dr("Imposta") = Riga.Iva
                        dr("Cod_Iva") = Riga.Cod_Iva
                        dr("Aliquota") = Riga.Iva_Des


                        dt.Rows.Add(dr)

                    Next

                End If

                Dim serializerSettings As New JsonSerializerSettings With {
                    .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }

                RiepilogoCastelletto = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)


                'Non dovrebbe servire perché la funzione Calcola_Importo_Documento stessa restituisce il valore calcolato per newimporto
                'Select Case lavCod

                '    Case LAVCOD_ORDINE_ACQUISTO, LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA  'RICEVIMENTO

                '        newimporto = -(Imponibile_Netto - Imposta)

                '    Case Else

                '        newimporto = Imponibile_Netto + Imposta


                'End Select

                RiepilogoImporti = New Contabilita_Riepilogo_Importi With {
                    .ImponibileLordo = Imponibile_Lordo,
                    .Variazioni = Variazioni,
                    .ImponibileNetto = Imponibile_Netto,
                    .Imposta = Imposta,
                    .TotaleDocumento = newimporto
                }

                '=============================================================================================================================


                'TODO: devo aggiornare sempre o solo per determinati tipi di lav_cod?!?

                'Aggiorno solo se so esattamente qual è l'Id_Mov di testata
                If idMovT <> 0 AndAlso idMovT <> -1 Then
                    Dim objMovHelper As New Agenda_Movimenti_Helper
                    objMovHelper.ModificaPuntuale(piva, 0, idAgenda, idMovT,
                                                  objParametriServer,
                                                  tipoPeso:=CInt(enum_TipoPeso.Peso_Lordo),
                                                  peso:=newPesoLordoDoc,
                                                  colli:=newNumColli,
                                                  taraImballi:=newTaraImballi,
                                                  taraVeicolo:=newTaraVeicolo,
                                                  Num_Protocollo:=newimporto)
                End If

                'TODO: tara veicolo è da mettere anche in Peso di Mov_Dettaglio_Tecnico_Extra di testata


                'devo passarmi indietro anche i totali, perché sono da aggiornare in interfaccia!!!
                conteggiTotali = New Contabilita_Totali_Testata() With {
                    .Piva = piva,
                    .IdAgenda = idAgenda,
                    .IdMov = idMovT,
                    .Colli = newNumColli,
                    .PesoNettoProd = newPesoNettoProd,
                    .TaraImballi = newTaraImballi,
                    .PesoLordoProd = newPesoLordoProd,
                    .ImballiVuoti = newImballiVuoti,
                    .TaraVeicolo = newTaraVeicolo,
                    .PesoLordoDoc = newPesoLordoDoc,
                    .RiepilogoCastelletto = RiepilogoCastelletto,
                    .RiepilogoImporti = RiepilogoImporti
                }

            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return conteggiTotali

    End Function

    'Public Sub Calcola_ImportoDocumento_CastellettoIva(ByVal piva As String,
    '                                                   ByVal idAgenda As Integer,
    '                                                   ByVal lavCod As Integer,
    '                                                   ByRef objParametriServer As AgronicaCoreParametri)

    '    Const nomeRoutine = "UtilityHelper.Calcola_ImportoDocumento_CastellettoIva()"

    '    Try

    '        'Calcolo Totale Documento
    '        Dim Imponibile_Lordo As Decimal = 0
    '        Dim Variazioni As Decimal = 0
    '        Dim Imponibile_Netto As Decimal = 0
    '        Dim Imposta As Decimal = 0
    '        Dim Importo As Decimal = 0

    '        Dim listCastelletto As List(Of Contabilita_Castelletto_Iva) = Nothing

    '        Dim helperDet As New Agenda_Movimenti_Dettagli_Helper
    '        Dim objMovDets = helperDet.Leggi(piva, 0, idAgenda, 0, 0,
    '                                         objParametriServer, leggiEsclusivamenteDettaglio:=False)

    '        Importo = Calcola_Importo_Documento(Imponibile_Netto,
    '                                            Imponibile_Lordo,
    '                                            Variazioni,
    '                                            Imposta,
    '                                            listCastelletto,
    '                                            lavCod,
    '                                            0,
    '                                            objMovDets,
    '                                            objParametriServer)

    '        Dim j = 9

    '    Catch ex As Exception
    '        Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
    '    End Try
    'End Sub

    Public Function Calcola_Importo_Documento(ByRef Imponibile_Netto As Decimal,
                                              ByRef Imponibile_Lordo As Decimal,
                                              ByRef Variazioni As Decimal,
                                              ByRef Imposta As Decimal,
                                              ByRef listCastelletto As List(Of Contabilita_Castelletto_Iva),
                                              ByVal Lav_Cod As Integer,
                                              ByVal EsigibilitaIva As Integer,
                                              ByVal objMovDets As List(Of Movimento_Dettaglio),
                                              ByRef objParametri_Server As AgronicaCoreParametri
                                              ) As Decimal

        Const nomeRoutine = "UtilityHelper.Calcola_Importo_Documento()"

        Dim Importo As Decimal = 0

        Dim i As Integer = 0

        Dim Sconto As Decimal = 0
        Dim Aliquota_Iva As Decimal = 0
        Dim Sigla_Iva As String = ""

        Dim Imposta_Campione_Omaggio As Decimal = 0
        Dim Importo_Campione_Omaggio As Decimal = 0
        Dim Totale_Importo_Omaggi As Decimal = 0
        Dim Totale_Imposta_Omaggi As Decimal = 0
        Dim Iva_Split As Decimal = 0

        Dim objMovDet As Movimento_Dettaglio
        Dim objContabHelper As New AgronicaCoreContabHLP.Contabilita

        Try

            Variazioni = ArrotondaVal_2(0)
            Imposta = ArrotondaVal_2(0)
            Imponibile_Lordo = ArrotondaVal_2(0)
            Imponibile_Netto = ArrotondaVal_2(0)

            listCastelletto = New List(Of Contabilita_Castelletto_Iva)

            Dim objIva As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R
            Dim dtIva As DataTable = objIva.Leggi(0, -1, -1, -1, NATURA_ESCLUSIONE_NOFILTRO, "", "", objParametri_Server)
            Dim dr_search() As DataRow


            For i = 0 To objMovDets.Count - 1

                objMovDet = objMovDets(i)

                ''NOTA: LA QTA VA IN ABS PER GESTIRE I RESI!!!
                objMovDet.Qta = Math.Abs(objMovDet.Qta)

                'Controllo Riga Descrizione
                If objMovDet.Elem_Cod <> RIGA_DESCRIZIONE Then

                    'Calcolo del prezzo netto
                    Select Case objMovDet.Sconto_Modalita

                        Case enModalitaSconto.Percentuale

                            'Sottraggo lo sconto cliente
                            objMovDet.Prezzo_Unitario_Netto = ArrotondaVal(objMovDet.Prezzo_Unitario + (CDec(objMovDet.Prezzo_Unitario / 100) * objMovDet.Sconto), 8)

                            'Sottraggo lo sconto listino
                            objMovDet.Prezzo_Unitario_Netto = ArrotondaVal(objMovDet.Prezzo_Unitario_Netto - (CDbl(objMovDet.Prezzo_Unitario_Netto / 100) * objMovDet.Sconto_Listino), 8)


                        Case enModalitaSconto.Sconto_Merce, enModalitaSconto.Campioni_Gratuiti

                            objMovDet.Prezzo_Unitario_Netto = ArrotondaVal_2(0)


                        Case enModalitaSconto.Omaggio_SenzaRivalsaIva, enModalitaSconto.Omaggio_ConRivalsaIva

                            objMovDet.Prezzo_Unitario_Netto = ArrotondaVal(objMovDet.Prezzo_Unitario, 6)

                    End Select


                    'Imponibile_Netto = objMovDet.Imponibile_Netto
                    'Imponibile_Lordo = objMovDet.Imponibile


                    'Cerco se l'aliquota Iva è già presente in griglia               
                    Dim rigaCastelletto As Contabilita_Castelletto_Iva = listCastelletto.FirstOrDefault(Function(x) x.Cod_Iva = objMovDet.Cod_Iva)

                    If rigaCastelletto Is Nothing Then
                        'Prima volta di questa aliquota iva
                        dr_search = dtIva.Select("Codice = " & objMovDet.Cod_Iva)

                        If dr_search.Count > 0 Then
                            Aliquota_Iva = CDec(If(IsDBNull(dr_search(0).Item("Aliquota")), 0, dr_search(0).Item("Aliquota")))
                            Sigla_Iva = dr_search(0).Item("Sigla")
                        Else
                            'Ecceione
                            Aliquota_Iva = 0
                            Sigla_Iva = "Non Ivabile"
                        End If

                        rigaCastelletto = New Contabilita_Castelletto_Iva With {
                            .Cod_Iva = objMovDet.Cod_Iva,
                            .Aliquota_Iva = Aliquota_Iva,
                            .Iva_Des = Sigla_Iva,
                            .Imponibile_Netto = ArrotondaVal_2(0),
                            .Imponibile_Lordo = ArrotondaVal_2(0),
                            .Iva = ArrotondaVal_2(0),
                            .Imponibile_Campioni_Omaggio_Detrazione = ArrotondaVal_2(0),
                            .ImponibilexImposta_Campioni_Omaggio_Detrazione = ArrotondaVal_2(0)
                        }
                        listCastelletto.Add(rigaCastelletto)
                    End If

                    'Sconto = ArrotondaVal_2(Sconto)

                    Dim impNettoUtente = objContabHelper.Leggi_Imponibile_PositivoNegativo(Lav_Cod, objMovDet.Imponibile_Netto)
                    Dim impLordoUtente = objContabHelper.Leggi_Imponibile_PositivoNegativo(Lav_Cod, objMovDet.Imponibile)
                    Dim impostaUtente = objContabHelper.Leggi_IVA_PositivaNegativa(Lav_Cod, objMovDet.Iva)

                    'Imponibili e Imposta complessivi di tutte le righe:
                    Imponibile_Netto += impNettoUtente
                    Imponibile_Lordo += impLordoUtente
                    Imposta += impostaUtente

                    Sconto = impLordoUtente - impNettoUtente

                    'Raggruppamento per AliquotaIVA per castelletto
                    rigaCastelletto.Iva += impostaUtente
                    rigaCastelletto.Imponibile_Netto += impNettoUtente
                    rigaCastelletto.Imponibile_Lordo += impLordoUtente

                    Select Case CInt(objMovDet.Sconto_Modalita)

                        Case enModalitaSconto.Percentuale

                            '=======================================================================================================
                            'Importo dettaglio
                            '-------EsigibilitaIva------------------------------------------------------------------------------------------------

                            'Controllo Split Payment
                            If EsigibilitaIva = 3 Then

                                Iva_Split += Imposta
                                'Imposta = 0

                            End If

                            'Importo = ArrotondaVal_2(impNettoUtente - impostaUtente)
                            Variazioni -= Sconto

                            '=======================================================================================================


                        Case enModalitaSconto.Omaggio_SenzaRivalsaIva

                            'Indico che l'imponibile non verrà considerato nel calcolo importo
                            rigaCastelletto.Imponibile_Campioni_Omaggio_Detrazione += impNettoUtente
                            rigaCastelletto.ImponibilexImposta_Campioni_Omaggio_Detrazione += impNettoUtente

                            Totale_Importo_Omaggi += impNettoUtente

                            If IsNumeric(rigaCastelletto.Aliquota_Iva) AndAlso rigaCastelletto.Aliquota_Iva <> 0D Then
                                Totale_Imposta_Omaggi += ArrotondaVal_2(impNettoUtente * rigaCastelletto.Aliquota_Iva / 100)
                            End If


                            'Importo = 0

                        Case enModalitaSconto.Omaggio_ConRivalsaIva

                            rigaCastelletto.Imponibile_Campioni_Omaggio_Detrazione += impNettoUtente

                            Totale_Importo_Omaggi += impNettoUtente

                            'Importo = impostaUtente

                        Case Else

                            'Importo = 0

                    End Select

                End If

            Next i

            'Imponibile_Netto = 0
            'Imponibile_Lordo = 0
            'Imposta = 0
            'Importo = 0


            'Calcolo Imponibile
            'For i = 0 To objMovDets.Count - 1

            '    If objMovDets(i).Elem_Cod <> RIGA_DESCRIZIONE Then
            '        Imponibile_Lordo += objMovDets(i).Imponibile
            '    End If

            'Next i

            'Calcolo Imposta
            'For Each row In listCastelletto
            '    row.Iva = ArrotondaVal_2(row.Iva)

            '    Imponibile_Netto += row.Imponibile_Netto

            '    Imposta += row.Iva

            '    Totale_Importo_Omaggi += row.Imponibile_Campioni_Omaggio_Detrazione

            '    If IsNumeric(row.Aliquota_Iva) AndAlso row.Aliquota_Iva <> 0D Then
            '        Totale_Imposta_Omaggi += ArrotondaVal_2(row.ImponibilexImposta_Campioni_Omaggio_Detrazione * row.Aliquota_Iva / 100)
            '    End If
            'Next


            'Aggiornamento Importi
            'Imponibile_Lordo = ArrotondaVal_2(Imponibile_Lordo)
            'Imponibile_Netto = ArrotondaVal_2(Imponibile_Netto)
            'Imposta = ArrotondaVal_2(Imposta)

            Importo = ArrotondaVal_2(Imponibile_Netto + Imposta - Totale_Importo_Omaggi - Totale_Imposta_Omaggi - Iva_Split)

            '#################################################################################################################################
            '#################################################################################################################################
            '#################################################################################################################################

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return Importo

    End Function





    '    Private Sub FormImpostaModalitaPagamento(ByVal Cod_Contatto As String)
    '        Dim ObjDati As Object
    '        Dim RsContatto_Codici As ADODB.Recordset

    '        Select Case mId_Agenda_Riferimento

    '            Case 0 'Impostazione sul default del contatto

    'Contatto_Default:

    '                If Trim(Cod_Contatto) <> "" Then

    '              'Modlità di Pagamento Default
    '              Set ObjDati = CreateObject("Agro_Contab_AD.Contatti_Codici_R")
    '              Set RsContatto_Codici = ObjDati.Leggi_Modalita_Pagamento(mPiva, 0, Cod_Contatto, 4004, , 2, objConnessione, , , ConnessioneAlternativa)

    '              If RsContatto_Codici.State <> 0 Then

    '                        CmbPagamento.ListIndex = -1
    '                        FormCmbSeleziona CmbPagamento_P, RsContatto_Codici("Cau_Pagamento"), ""

    '              Else

    '                        'PRESELEZIONE CASSA (DEFAULT) SE NON IMPOSTATO!!
    '                        FormCmbSeleziona CmbPagamento_P, 3, ""

    '              End If

    '                Else

    '                    'PRESELEZIONE CASSA (DEFAULT) SE NON IMPOSTATO!!
    '                    FormCmbSeleziona CmbPagamento_P, 3, ""

    '           End If


    '            Case Else

    '                'Lettura Pagamento del Riferimento (es ordine allegato a ddt)
    '                Dim cMagazzino As New cMagazzino
    '                Dim RsPagamento As ADODB.Recordset

    '           Set RsPagamento = cMagazzino.ListaPagamenti(mPiva, , mId_Agenda_Riferimento)

    '           If RsPagamento.State <> 0 Then

    '                    CmbPagamento.ListIndex = -1
    '                    FormCmbSeleziona CmbPagamento_P, RsPagamento("Cau_Pagamento"), ""


    '           Else

    '                    GoTo Contatto_Default

    '                End If




    '        End Select

    '    End Sub


    Public Function Impostazione_Modalita_Pagamento(ByVal Piva As String,
                                                    ByVal Cod_Contatto As String,
                                                    ByVal Cod_Risum As Integer,
                                                    ByVal Lav_Cod As Integer,
                                                    ByVal Cau_Pagamento As Integer,
                                                    ByVal Importo_Pagamento As Decimal,
                                                    ByVal Id_Agenda As Integer,
                                                    ByVal Id_Mov As Integer,
                                                    ByVal Data_Documento As DateTime,
                                                    ByRef Data_Scadenza As DateTime,
                                                    ByRef objParametri_Server As AgronicaCoreParametri) As Pagamento

        Const nomeRoutine = "DocContabile_WS.Impostazione_Modalita_Pagamento()"

        Dim r As New RispostaStandard


        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim Giorni_Scadenza As Integer = 0
        Dim Opzione As Integer = 0
        Dim Giorno As Integer = 0
        Dim Mese As Integer = 0
        Dim xFiltroAggiuntivo As String = ""
        Dim strFiltro As String = ""

        Dim Cod_Conto_Crediti As Integer = CONTO_PAT_CREDITI_VS_CLIENTI
        Dim Cod_Conto_Depositi As Integer = CONTO_PAT_DEPOSITI
        Dim Cod_Conto_Cassa As Integer = CONTO_PAT_CASSA

        Dim Cod_Liquidita_Dare As Integer = 0
        Dim Cod_Liquidita_Avere As Integer = 0

        Dim CodiceIbanDefault As Integer = 0
        Dim Cau_Risorsa As Integer = 0

        Dim Cod_Deposito_Default_Piva As Integer = 0
        Dim Cod_Liquidita_Default_Piva As Integer = 0
        Dim Cod_Deposito_Default_Contatto As Integer = 0

        'Creazione Oggetti
        Dim DT_Causali_Pagamento As New DataTable
        Dim DT_Contatto_Codici As New DataTable
        Dim DT_Conti_Pat As New DataTable
        Dim DT_Risorse As New DataTable
        Dim DT_Contatti As New DataTable
        Dim dr_search() As DataRow

        Dim Leggi_Causali_Pagamento As New AgronicaCoreContabDAL.Pagamenti_Causali_R
        Dim Leggi_Contatto_Codici As New AgronicaCoreAnagrafeDAL.Contatti_Codici_R
        Dim Leggi_Conti_Pat As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R
        Dim Leggi_Risorse As New AgronicaCoreContabDAL.Liquidita_R
        Dim Leggi_Contatto As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

        Dim objPagamentiHelper As New Agenda_Pagamenti_Helper

        Dim objPagamento As New Pagamento

        Try
            objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO
            objParametri_Server.FinestraTemporaleFine = AGRODATAFINE

            If Trim(Cod_Contatto) = "" Then

                'Impostazione Cod_Contatto
                DT_Contatti = Leggi_Contatto.Leggi(Piva, "", Cod_Risum, 0, 0, "", True, True, "", "", objParametri_Server)

                Cod_Contatto = DT_Contatti(0).Item("Cod_Contatto")

            End If


            '=====================================================================================================
            'Lettura Risorse Finanziarie (Incassi e Pagamenti) Default
            '-----------------------------------------------------------------------------------------------------
            'Piva --> Liquidita Immediata
            xFiltroAggiuntivo = "Riferimento = '" & Piva & "' And ChkAbilitazione = 1 And Cau_Risorsa = " & LIQUIDITA_IMMEDIATA
            DT_Risorse = Leggi_Risorse.Leggi(Piva, xFiltroAggiuntivo, "ChkDefault Desc", objParametri_Server)

            'Controllo Presenza Default
            If DT_Risorse.Rows.Count > 0 Then
                Cod_Liquidita_Default_Piva = DT_Risorse(0).Item("Cod_Liquidita")
            End If

            'Piva --> Risorsa Finanziaria (Deposito)
            xFiltroAggiuntivo = "Riferimento = '" & Piva & "' And ChkAbilitazione = 1 And Cau_Risorsa = " & RISORSA_FINANZIARIA
            DT_Risorse = Leggi_Risorse.Leggi(Piva, xFiltroAggiuntivo, "ChkDefault Desc", objParametri_Server)

            'Controllo Presenza Default
            If DT_Risorse.Rows.Count > 0 Then
                Cod_Deposito_Default_Piva = DT_Risorse(0).Item("Cod_Liquidita")
            End If

            'Contatto
            xFiltroAggiuntivo = "Riferimento = '" & Cod_Contatto & "' And ChkAbilitazione = 1" '
            DT_Risorse = Leggi_Risorse.Leggi(Piva, xFiltroAggiuntivo, "ChkDefault Desc", objParametri_Server)

            'Controllo Presenza Default
            If DT_Risorse.Rows.Count > 0 Then
                Cod_Deposito_Default_Contatto = DT_Risorse(0).Item("Cod_Liquidita")
            End If
            '=====================================================================================================


            '=====================================================================================================
            'Impostazione Enumerativi Conti Patrimoniali
            '-----------------------------------------------------------------------------------------------------

            '34 = Crediti vs Clienti
            '46 = Depositi
            '48 = Cassa

            'Nota: Non dovrebbero cambiare... Riferimento = Cod_Conto... per correttezza lo faccio ma può essere bypassato
            xFiltroAggiuntivo = "Codifica_Conto_Pat In (34, 46, 48)"
            DT_Conti_Pat = Leggi_Conti_Pat.Leggi(Piva, 2, Year(Data_Documento), 0, "", xFiltroAggiuntivo, "", objParametri_Server)

            'Controllo Presenza Default
            If DT_Conti_Pat.Rows.Count > 0 Then

                strFiltro = "Codifica_Conto_Pat = 34"
                dr_search = DT_Conti_Pat.Select(strFiltro)
                If dr_search.Count > 0 Then
                    Cod_Conto_Crediti = dr_search(0).Item("Cod_Conto_Pat")
                End If

                strFiltro = "Codifica_Conto_Pat = 46"
                dr_search = DT_Conti_Pat.Select(strFiltro)
                If dr_search.Count > 0 Then
                    Cod_Conto_Depositi = dr_search(0).Item("Cod_Conto_Pat")
                End If

                strFiltro = "Codifica_Conto_Pat = 48"
                dr_search = DT_Conti_Pat.Select(strFiltro)
                If dr_search.Count > 0 Then
                    Cod_Conto_Cassa = dr_search(0).Item("Cod_Conto_Pat")
                End If

            End If


            '=========================================================================================================================
            'Modalità di Pagamento Default
            '-------------------------------------------------------------------------------------------------------------------------

            'Controllo se la modalità di pagamento è ereditata
            Select Case Cau_Pagamento

                Case 9999 'Indefinito --> Letta da anagrafe contatto

                    'Default Cassa                                 
                    Cau_Risorsa = LIQUIDITA_IMMEDIATA

                    DT_Contatto_Codici = Leggi_Contatto_Codici.Leggi("", Cod_Contatto, 4004, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                    'Controllo Presenza Default
                    If DT_Contatto_Codici.Rows.Count > 0 Then

                        Cau_Pagamento = DT_Contatto_Codici(0).Item("Val_Cod")

                    End If


                Case Else

                    'Causale Ereditata da Documento Precedente

            End Select

            'Inizializzazione
            Giorni_Scadenza = 0
            Opzione = 0

            If CInt(Cau_Pagamento) <> 0 Then

                'Lettura della Causale di Pagamento per determinazione Scadenza               
                DT_Causali_Pagamento = Leggi_Causali_Pagamento.Leggi("", Cau_Pagamento, "", "", objParametri_Server)

                'Controllo Presenza Record
                If DT_Causali_Pagamento.Rows.Count > 0 Then

                    'Impostazione Tipo Risoesa (Liquidità Immediata o Risorsa Finanziaria
                    Cau_Risorsa = DT_Causali_Pagamento(0).Item("Cau_Risorsa")

                    'Determinazione Giorni Scadenza ed Opzione
                    Giorni_Scadenza = DT_Causali_Pagamento(0).Item("Giorni_Scadenza")
                    Opzione = DT_Causali_Pagamento(0).Item("Opzione")

                End If

            End If

            '=========================================================================================================================


            '=========================================================================================================================
            'Calcolo Data Scadenza
            '-------------------------------------------------------------------------------------------------------------------------
            Data_Scadenza = CalcolaDataScadenzaPagamento(Opzione, Giorni_Scadenza, Data_Documento)


            '=========================================================================================================================
            'Costruzione Xml
            '-------------------------------------------------------------------------------------------------------------------------
            'Pagamento
            objPagamento.Id_Agenda = 0
            objPagamento.Piva = Piva
            objPagamento.Sa_Cod = 0
            objPagamento.Id_Agenda = Id_Agenda
            objPagamento.Id_Mov = Id_Mov
            objPagamento.Cod_Pagamento = 0
            objPagamento.Previsto_Avvenuto = 0 'Piano Pagamento
            objPagamento.Percentuale = If(Importo_Pagamento = 0, 100, 0) 'gestione dinamica sulla percentuale
            objPagamento.Importo = Importo_Pagamento

            objPagamento.ChkDataScadenza_Manuale = 0

            objPagamento.Data_Pagamento = AGRODATAFINE 'Si basa su Previsto_Avvenuto, ora è fissa perché fisso a pagamento Previsto
            objPagamento.Note = ""
            objPagamento.Cau_Risorsa = Cau_Risorsa

            'Inizializzazione
            objPagamento.Tipo_Dare = 0
            objPagamento.Tipo_Avere = 0
            objPagamento.Tipo_Cod_Dare = 0
            objPagamento.Tipo_Cod_Avere = 0
            objPagamento.Cod_Conto_Dare = 0
            objPagamento.Cod_Conto_Avere = 0
            objPagamento.Cod_Conto_Pat_Dare = 0
            objPagamento.Cod_Conto_Pat_Avere = 0
            objPagamento.Cod_Liquidita_Dare = -1 'Indefinito
            objPagamento.Cod_Liquidita_Avere = -1 'Indefinito
            '=====================================================================================================

            Select Case CInt(Cau_Risorsa)

                Case LIQUIDITA_IMMEDIATA 'Cassa

                    objPagamento.Cod_Conto_Pat_Dare = Cod_Conto_Cassa
                    objPagamento.Cod_Conto_Pat_Avere = Cod_Conto_Crediti
                    objPagamento.Tipo_Dare = 0
                    objPagamento.Tipo_Cod_Dare = 0
                    objPagamento.Tipo_Avere = 1
                    objPagamento.Tipo_Cod_Avere = Cod_Risum

                    objPagamento.Cod_Liquidita_Dare = Cod_Liquidita_Default_Piva 'Pagamento in contanti
                    objPagamento.Cod_Liquidita_Avere = -1 'indefinito


                Case Else

                    objPagamento.Cod_Conto_Pat_Dare = Cod_Conto_Depositi

                    'If bLav_Cod_Invertito Then 'vedi Lan per gestione Note credito
                    objPagamento.Cod_Conto_Pat_Avere = Cod_Conto_Crediti
                    objPagamento.Tipo_Dare = 0
                    objPagamento.Tipo_Cod_Dare = 0
                    objPagamento.Tipo_Avere = 1
                    objPagamento.Tipo_Cod_Avere = Cod_Risum


                    'Impostazione Codice Iban Default Superuser
                    DT_Contatto_Codici = Leggi_Contatto_Codici.Leggi("", Cod_Contatto, 4012, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                    'Inizializzazione
                    CodiceIbanDefault = Cod_Deposito_Default_Piva

                    'Controllo Presenza Default
                    If DT_Contatto_Codici.Rows.Count > 0 Then

                        If IsNumeric(DT_Contatto_Codici(0).Item("Val_Cod")) Then
                            If CInt(DT_Contatto_Codici(0).Item("Val_Cod")) <> 0 Then
                                CodiceIbanDefault = DT_Contatto_Codici(0).Item("Val_Cod")
                            End If
                        End If

                    End If

                    '==============================================================================================================

                    objPagamento.Cod_Liquidita_Dare = CodiceIbanDefault
                    objPagamento.Cod_Liquidita_Avere = Cod_Deposito_Default_Contatto


            End Select

            objPagamento.Anno = Year(Data_Scadenza)
            objPagamento.Ric_Cod = 2
            objPagamento.Ric_Cod_Pat = 2

            objPagamento.Cau_Pagamento = Cau_Pagamento
            objPagamento.Extra_Str = ""
            objPagamento.Extra_Int = 0
            objPagamento.Extra_Date = AGRODATAFINE
            objPagamento.Validita_Inizio = AGRODATAINIZIO
            objPagamento.Validita_Fine = AGRODATAFINE

        Catch ex As Exception
            'r.RispostaOK = False
            'r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return objPagamento

    End Function

    Public Function CalcolaDataScadenzaPagamento(ByVal opzione As Integer, ByVal giorniScadenza As Integer, ByVal dataDocumento As DateTime) As DateTime
        Dim dataScadenza As DateTime = dataDocumento
        Dim giorno As Integer = 0
        Dim mese As Integer = 0

        Select Case opzione
            Case 0
                ' Normale aggiunta di giorni
                dataScadenza = DateAdd("d", giorniScadenza, dataDocumento)

            Case 1 ' Pagamento Fine Mese
                ' Nel caso i giorni siano multiplo di 30 viene inteso in mesi
                If giorniScadenza > 0 And giorniScadenza Mod 30 = 0 Then
                    dataScadenza = DateAdd("M", CInt(giorniScadenza / 30), dataDocumento)
                Else
                    dataScadenza = DateAdd("d", giorniScadenza, dataDocumento)
                End If

                mese = Month(dataScadenza)
                Select Case mese
                    Case 1 : giorno = 31
                    Case 2
                        If Year(dataScadenza) Mod 4 = 0 Then
                            giorno = 29
                        Else
                            giorno = 28
                        End If
                    Case 3 : giorno = 31
                    Case 4 : giorno = 30
                    Case 5 : giorno = 31
                    Case 6 : giorno = 30
                    Case 7 : giorno = 31
                    Case 8 : giorno = 31
                    Case 9 : giorno = 30
                    Case 10 : giorno = 31
                    Case 11 : giorno = 30
                    Case 12 : giorno = 31
                End Select

                dataScadenza = CDate(giorno & "/" & mese & "/" & Year(dataScadenza))

            Case 2 ' Pagamento Data Fine Mese
                dataScadenza = dataDocumento
                mese = Month(dataDocumento)
                Select Case mese
                    Case 1 : giorno = 31
                    Case 2 : giorno = 28
                    Case 3 : giorno = 31
                    Case 4 : giorno = 30
                    Case 5 : giorno = 31
                    Case 6 : giorno = 30
                    Case 7 : giorno = 31
                    Case 8 : giorno = 31
                    Case 9 : giorno = 30
                    Case 10 : giorno = 31
                    Case 11 : giorno = 30
                    Case 12 : giorno = 31
                End Select

                ' Ricavo il fine mese
                dataScadenza = CDate(giorno & "/" & mese & "/" & Year(dataScadenza))
                ' Aggiungo i giorni
                dataScadenza = DateAdd("d", giorniScadenza, dataScadenza)
        End Select

        Return dataScadenza
    End Function


    Public Shared Sub SistemaImballaggiDocumento(ByVal piva As String,
                                                 ByVal idAgenda As Integer,
                                                 ByVal idMovCS As Integer,
                                                 ByVal tipoOperazioneDettaglio As enum_TipoOperazioneDB,
                                                 ByRef objParametriServer As AgronicaCoreParametri,
                                                 Optional ByVal moduloGias As enum_Omni_Modulo_Generazione = enum_Omni_Modulo_Generazione.Nessuno,
                                                 Optional ByRef objDettaglio As Movimento_Dettaglio = Nothing)

        Const nomeRoutine = "UtilityHelper.SistemaImballaggiDocumento()"
        Dim objDetHelper As New Agenda_Movimenti_Dettagli_Helper

        Try

            'Eseguo la query che mi dice la somma degli imballi divisi per tipo e Sa_Cod
            Dim objMovExtraR As New AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_R
            Dim dtContenitori = objMovExtraR.ConfezionamentiAgenda(piva, idAgenda, idMovCS, True,
                                                                   "", "Sa_Cod", objParametriServer)

            Dim contenitoriList As List(Of Contabilita_Confezionamento) = (
                From dr In dtContenitori.Rows
                Select New Contabilita_Confezionamento() With {
                    .SaCod = CInt(dr("Sa_Cod")),
                    .Tipo = CInt(dr("Tipo")),
                    .Quantita = CInt(dr("Quantita")),
                    .ElemCod = CInt(dr("Elem_Cod")),
                    .Codice = CInt(dr("Prodotto_Cod")),
                    .Descrizione = dr("Descrizione").ToString(),
                    .Tara_Unitaria = CDec(dr("Tara_Unitaria"))
                }).ToList()


            'Devo recuperare i mov_dettagli (+ figli) con ordine_det = 1000
            Dim objDettagliR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim dtDettagli = objDettagliR.Leggi(piva, 0, idAgenda, idMovCS,
                                                0, 0, 0, 0, "", 0,
                                                0, 0, 0, 0, 0,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "(Ordine_Det = 1000 AND Jolly_Int = " & MagazzinoMovimentato & ")",
                                                "", objParametriServer)

            'Confronto i due elenchi e capisco quali mov_dettagli devo eliminare e/o creare gli oggetti dei dettagli da scrivere
            For Each dr In dtDettagli.Rows

                Dim objProd = contenitoriList.FirstOrDefault(Function(x) x.SaCod = dr.Item("Sa_Cod") AndAlso
                                                                         x.ElemCod = dr.Item("Elem_Cod") AndAlso
                                                                         x.Codice = dr.Item("Mat_Cod") AndAlso
                                                                         x.Tara_Unitaria = dr.Item("Qta_Extra"))

                If objProd Is Nothing Then

                    'Non c'è più il prodotto corrispondente ==> dovrò cancellare la riga del dettaglio e figli
                    objDetHelper.Cancella(dr.Item("Piva"), 0, dr.Item("Id_Agenda"),
                                          dr.Item("Id_Mov"), dr.Item("Id_Mov_Det"),
                                          objParametriServer)

                ElseIf CDec(objProd.Quantita) <> CDec(dr.Item("Qta")) Then

                    'Il prodotto c'è, ma è cambiata la qta ==> aggiorno solo la qta in maniera mirata dove serve

                    'Qta in Movimenti_Dettagli
                    objDetHelper.ModificaPuntuale(dr.Item("Piva"), 0, dr.Item("Id_Agenda"),
                                                  dr.Item("Id_Mov"), dr.Item("Id_Mov_Det"),
                                                  objParametriServer,
                                                  qta:=CDec(objProd.Quantita),
                                                  qtaExtraTotale:=CDec(objProd.Quantita) * objProd.Tara_Unitaria)

                    'Qta in Mov_destinazioni
                    Dim objDestHelper As New Agenda_Movimenti_Destinazioni_Helper
                    objDestHelper.ModificaPuntuale(dr.Item("Piva"), 0, dr.Item("Id_Agenda"),
                                                   dr.Item("Id_Mov"), dr.Item("Id_Mov_Det"),
                                                   0, 0, objParametriServer,
                                                   qta:=CDec(objProd.Quantita))

                    'Dopo aver fatto, rimuovo l'elemento dalla lista
                    contenitoriList.Remove(objProd)

                Else

                    'Il prodotto c'è e non è cambiato nulla, lo rimuovo direttamente dalla lista
                    contenitoriList.Remove(objProd)

                End If

            Next

            'Se ho chiamato questa funzione perché ho cancellato la riga, di sicuro non avrò nessuna riga da scrivere
            If contenitoriList.Count > 0 AndAlso tipoOperazioneDettaglio <> enum_TipoOperazioneDB.Cancellazione Then

                'Alla fine, avendo ciclato sui dettagli, potrei avere cmq delle righe che sono sulla lista oggetti e non c'erano sui dettagli

                'Avendo ordinato per Sa_Cod, posso permettermi di andare a leggere il magazzino specifico degli imballi solo ad ogni cambio sa_cod

                Dim saCodImballi As Integer = 0
                Dim fabbricatoCodImballi As Integer = 0

                '==> sono prodotti nuovi, devo creare le relative righe
                For Each item In contenitoriList

                    'Devo sapere in quale magazzino caricare/scaricare imballi
                    If fabbricatoCodImballi = 0 OrElse item.SaCod <> saCodImballi Then

                        saCodImballi = item.SaCod
                        fabbricatoCodImballi = 0

                        Dim genAnagrafeLog As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
                        genAnagrafeLog.Recupera_Magazzino_Imballaggi(piva, moduloGias,
                                                                     "", objParametriServer,
                                                                     saCodImballi, fabbricatoCodImballi)

                        If saCodImballi = 0 OrElse fabbricatoCodImballi = 0 Then
                            Throw New Exception(String.Format("Non sono in grado di ricavare il magazzino per gli imballi del centro {0}.", saCodImballi))
                        End If
                    Else
                        'é ancora lo stesso centro e so già qual è il magazzino imballi
                    End If

                    Dim magazzinoImballi As New Contabilita_Magazzino With {
                        .TipoDestinazione = MAGAZZINO,
                        .SaCod = saCodImballi,
                        .IdDestinazione = fabbricatoCodImballi
                    }

                    'Cod_Iva=0, Contabilizzato=1, Pendente=4, Extra_Int=0, Cal_Cod=0, Jolly_Int=0, Udm_Cod_Extra=2, Qta_Extra=31, Qta_Extra_Totale=0, Tara=0

                    Dim objDettaglioConfezionamento = GeneraRigaDettaglioConfezionamento(item.ElemCod, item.Codice,
                                                                                         item.Descrizione, item.Quantita,
                                                                                         item.Tara_Unitaria, magazzinoImballi,
                                                                                         objDettaglio)

                    objDetHelper.Scrivi(objDettaglioConfezionamento, objParametriServer)

                Next

            End If

            'TODO: l'alternativa a fare tutto questo casino per capire quali sono le righe cambiate,
            'sarebbe quella di cancellare tutte le righe dei confezionamenti e riscriverle in base al nuovo elenco della lista degli oggetti

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

    End Sub

    Private Shared Function GeneraRigaDettaglioConfezionamento(ByVal elemCod As Integer,
                                                               ByVal matCod As Integer,
                                                               ByVal matDes As String,
                                                               ByVal qta As Decimal,
                                                               ByVal tara As Decimal,
                                                               ByVal magazzino As Contabilita_Magazzino,
                                                               ByRef objDettaglioProdotto As Movimento_Dettaglio
                                                               ) As Movimento_Dettaglio

        Const nomeRoutine = "ContabilitaHelper.GeneraRigaDettaglioConfezionamento()"
        Dim objDettaglio As Movimento_Dettaglio

        Try

            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO
            '------------------------------------------------

            'TODO: Udm_Cod_Extra = sempre KG ?!?
            'TODO Contabilizzato = NONCONTABILE ?!?
            'TODO: Extra_Int = 1 ?!?

            objDettaglio = New Movimento_Dettaglio With {
                .Piva = objDettaglioProdotto.Piva,
                .Sa_Cod = If(magazzino.SaCod, 0),
                .Id_Agenda = objDettaglioProdotto.Id_Agenda,
                .Id_Mov = objDettaglioProdotto.Id_Mov,
                .Id_Mov_Det = 0,
                .Data = objDettaglioProdotto.Data,
                .Validita_Inizio = objDettaglioProdotto.Validita_Inizio,
                .Lav_Cod = objDettaglioProdotto.Lav_Cod,
                .Ordine_Det = 1000,
                .Elem_Cod = elemCod,
                .Pro_Cod = 0,
                .Mat_Cod = matCod,
                .Mov_Det_Des = matDes,
                .Cal_Cod = 0,
                .Udm_Cod = enum_UnitaMisura.Numero,
                .Qta = qta,
                .Udm_Cod_Extra = enum_UnitaMisura.KG,
                .Qta_Extra = tara,
                .Qta_Extra_Totale = qta * tara,
                .Cod_Iva = 0,
                .Extra_Int = 1,
                .Lotto = "",
                .Jolly_Int = MagazzinoMovimentato,
                .Mezzo_Det = 0,
                .Contabilizzato = objDettaglioProdotto.Contabilizzato,
                .Anno = Year(objDettaglioProdotto.Data),
                .Ric_Cod = 2,
                .Ric_Cod_Pat = 2,
                .Pendente = objDettaglioProdotto.Pendente,
                .Username_Creazione = objDettaglioProdotto.Username_Modifica,
                .Username_Modifica = objDettaglioProdotto.Username_Modifica
            }


            '------------------------------------------------
            '----- MOVIMENTO DESTINAZIONE
            '------------------------------------------------
            Dim objDestinazione = New Movimento_Destinazione With {
                .Piva = objDettaglio.Piva,
                .Sa_Cod = If(magazzino.SaCod, 0),
                .Id_Agenda = objDettaglio.Id_Agenda,
                .Id_Mov = objDettaglio.Id_Mov,
                .Id_Mov_Det = objDettaglio.Id_Mov_Det,
                .Appezza = If(magazzino.Appezza, 0),
                .Id_Destinazione = If(magazzino.IdDestinazione, 0),
                .Tipo = If(magazzino.TipoDestinazione, 0),
                .Data = objDettaglio.Data,
                .Qta = objDettaglio.Qta,
                .Qta2 = 0,
                .Qta_Dest1 = 0,
                .Qta_Dest2 = 0,
                .Username_Creazione = objDettaglioProdotto.Username_Modifica,
                .Username_Modifica = objDettaglioProdotto.Username_Modifica
            }

            'Aggancio il movimento destinazione sul movimento dettaglio
            objDettaglio.Movimenti_Destinazioni = New List(Of Movimento_Destinazione) From {
                objDestinazione
            }

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return objDettaglio

    End Function

#End Region

#Region "Conti"

    Public Shared Function GetConto(ByVal lavCod As Integer,
                                    ByVal tipoConto As enumTipoConto,
                                    Optional ByVal contoDefault As Integer? = Nothing
                                    ) As Integer

        Const nomeRoutine = "UtilityHelper.GetConto()"
        Dim codConto As Integer = 0

        Try

            Select Case lavCod

                Case LAVCOD_SCARICO, LAVCOD_BOLLA_EMESSA, LAVCOD_FATTURA_EMESSA,
                    LAVCOD_VENDITA, LAVCOD_NOTA_ACCREDITO_EMESSA,
                    LAVCOD_ORDINE_VENDITA

                    Select Case tipoConto
                        Case enumTipoConto.Economico
                            codConto = CONTO_ECO_RICAVI_VENDITE
                        Case enumTipoConto.Patrimoniale
                            codConto = CONTO_PAT_CREDITI_VS_CLIENTI
                    End Select

                Case LAVCOD_CARICO, LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA,
                    LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
                    LAVCOD_ORDINE_ACQUISTO

                    Select Case tipoConto
                        Case enumTipoConto.Economico
                            codConto = CONTO_ECO_COSTI_MATERIE_PRIME
                        Case enumTipoConto.Patrimoniale
                            codConto = CONTO_PAT_DEBITI_VS_FORNITORI
                    End Select

                Case Else

                    'Bypass: in alcuni contesti non vorrei l'errore ma impostare un default o zero (passato come contoDefault)
                    If contoDefault IsNot Nothing Then
                        codConto = contoDefault
                    Else
                        Throw New Exception(String.Format("Il Lav_Cod {0} non è gestito correttamente", lavCod))
                    End If

            End Select

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return codConto

    End Function

#End Region

#Region "Controlli su giacenze"

    ''' <summary>
    ''' Ritorna se deve essere effettuato il controllo di giacenza
    ''' </summary>
    ''' <param name="elemCod">Codice elemento per verifica controllo giacenza</param>
    ''' <param name="objParametriUtenti">Oggetto parametri utenti</param>
    ''' <param name="Flag_QtaNoZero">Flag</param>
    ''' <returns>Boolean che indica se deve essere effettuato il controllo</returns>

    Public Shared Function SeCheckGiacenze(elemCod As Integer,
                                           ByRef objParametriUtenti As AgronicaCoreParametri
                                           ) As UtilityHelperCheckGiacenze

        Dim CheckGiacenze = New UtilityHelperCheckGiacenze

        Dim impostazioneGiacenze As String() = GetImpostazioneGiacenze(objParametriUtenti)
        Dim bloccaUtentePerSottogiacenza As Boolean = GetBloccaPerSottogiacenza(objParametriUtenti)

        CheckGiacenze.eseguiCheckGiacenze = ControllaFlagGiacenze(elemCod,
                                                                  impostazioneGiacenze,
                                                                  bloccaUtentePerSottogiacenza,
                                                                  CheckGiacenze.flag_QtaNoZero,
                                                                  CheckGiacenze.flag_QtaMaggioreZero)

        Return CheckGiacenze

    End Function

    Public Shared Function GetParametriGiacenze(ByRef objParametriUtenti As AgronicaCoreParametri) As UtilityHelperParametriGiacenze

        Dim parametriGiacenze As New UtilityHelperParametriGiacenze

        parametriGiacenze.bloccaUtentePerSottogiacenza = GetBloccaPerSottogiacenza(objParametriUtenti)

        Dim impostazioneGiacenze As String() = GetImpostazioneGiacenze(objParametriUtenti)

        Dim elementoImpostazioneGiacenze() As String
        Const indElemCod As Integer = 0
        Const indGestGiac As Integer = 1
        Dim elemCod As String
        Dim gestioneGiacenze As String

        If Not impostazioneGiacenze Is Nothing Then

            For Each s As String In impostazioneGiacenze

                elementoImpostazioneGiacenze = s.Split("_")

                elemCod = elementoImpostazioneGiacenze(indElemCod)
                gestioneGiacenze = elementoImpostazioneGiacenze(indGestGiac)

                'Controllo che entrambi i dati siano numerici
                If IsNumeric(elemCod) AndAlso IsNumeric(gestioneGiacenze) Then

                    'Controllo che l'indicativo gestione giacenze sia definito in enum
                    If [Enum].IsDefined(GetType(enum_Gestione_Giacenze), CInt(gestioneGiacenze)) Then

                        parametriGiacenze.dizionarioGestioneGiacenze.Add(elemCod, gestioneGiacenze)

                    End If

                End If

            Next

        End If

        Return parametriGiacenze

    End Function

    Private Shared Function GetImpostazioneGiacenze(ByRef objParametriUtenti As AgronicaCoreParametri) As String()

        Dim impostazioneGiacenze As String()

        Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim xFiltroImpostazioni As String = " Impostazione_Cod IN ("
        xFiltroImpostazioni += CStr(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE)
        xFiltroImpostazioni += ") "

        Dim dtImpostazioniGiacenze = ui_R.Leggi(0,
                                                2,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                xFiltroImpostazioni,
                                                "",
                                                objParametriUtenti)

        If dtImpostazioniGiacenze.Rows.Count = 1 Then
            impostazioneGiacenze = dtImpostazioniGiacenze.Rows(0).Item("Impostazione_Valore_1").Split("|")
        End If

        Return impostazioneGiacenze

    End Function

    Private Shared Function GetBloccaPerSottogiacenza(ByRef objParametriUtenti As AgronicaCoreParametri) As Boolean

        Dim bloccaUtentePerSottogiacenza As Boolean = False

        Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim xFiltroImpostazioni As String = " Impostazione_Cod IN ("
        xFiltroImpostazioni += CStr(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE)
        xFiltroImpostazioni += ") "

        Dim dtImpostazioniGiacenze = ui_R.Leggi(0,
                                                1,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                xFiltroImpostazioni,
                                                "",
                                                objParametriUtenti)

        If dtImpostazioniGiacenze.Rows.Count = 1 Then
            Select Case dtImpostazioniGiacenze.Rows(0).Item("Impostazione_Valore_1")
                Case "0"
                    bloccaUtentePerSottogiacenza = False
                Case Else
                    bloccaUtentePerSottogiacenza = True
            End Select
        End If

        Return bloccaUtentePerSottogiacenza

    End Function

    Private Shared Function ControllaFlagGiacenze(ByVal Elem_Cod As Integer,
                                                  ByVal impostazioneGiacenze As String(),
                                                  ByVal bloccaUtentePerSottogiacenza As Boolean,
                                                  ByRef Flag_QtaNoZero As Boolean,
                                                  ByRef Flag_QtaMaggioreZero As Boolean) As Boolean

        Dim eseguiCheckGiacenze As Boolean = True
        Dim elem_cod_impostazione() As String = Nothing

        'ByRef causaleDaUtilizzare As String,
        'causaleDaUtilizzare = CAU_SCARICO

        If bloccaUtentePerSottogiacenza Then
            Flag_QtaNoZero = True
            Flag_QtaMaggioreZero = True
        Else
            If Not impostazioneGiacenze Is Nothing Then
                For Each s As String In impostazioneGiacenze
                    elem_cod_impostazione = s.Split("_")
                    If CInt(elem_cod_impostazione(0)) = Elem_Cod Then  '0=tutti; 1=soloMovimentati (default); 2=soloPresenti
                        If elem_cod_impostazione(1) = enum_Gestione_Giacenze.TuttiProdotti Then
                            'causaleDaUtilizzare = CAU_CARICO
                            eseguiCheckGiacenze = False
                        Else
                            If elem_cod_impostazione(1) = enum_Gestione_Giacenze.SoloPresenti Then
                                Flag_QtaNoZero = True
                                Flag_QtaMaggioreZero = True
                            Else
                                If elem_cod_impostazione(1) = enum_Gestione_Giacenze.SoloMovimentati Then
                                    Flag_QtaNoZero = False
                                    Flag_QtaMaggioreZero = False
                                End If
                            End If
                        End If
                        Exit For
                    End If
                Next
            End If
        End If

        Return eseguiCheckGiacenze

    End Function

#End Region

End Class

Public Class UtilityHelperCheckGiacenze
    Public eseguiCheckGiacenze As Boolean = False
    Public flag_QtaNoZero As Boolean = False
    Public flag_QtaMaggioreZero As Boolean = False
End Class

Public Class UtilityHelperParametriGiacenze
    Public bloccaUtentePerSottogiacenza As Boolean = False
    Public dizionarioGestioneGiacenze As New Dictionary(Of Integer, enum_Gestione_Giacenze)
End Class