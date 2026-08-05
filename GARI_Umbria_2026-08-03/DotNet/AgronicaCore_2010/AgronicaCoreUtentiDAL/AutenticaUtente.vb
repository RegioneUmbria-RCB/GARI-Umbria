Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.My.Resources
Imports System.Web
Imports System.Reflection
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData
Imports AgronicaCoreModelsSTD.utente
Imports AgronicaCoreModelsSTD.exceptions

Public Class AutenticaUtente

    Public ChiaveWS As JObject = Nothing

    ''' <summary>
    ''' La funzione esegue effettivamente il login per lo username passato, confrontando la password fornita con quella su database
    ''' La password fornita come argomento verrà sostituita dalla password corretta a db
    ''' </summary>
    ''' <param name="username"></param>
    ''' <param name="passwordEsterna"></param>
    ''' <param name="gestioneHashAbilitata"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns></returns>
    Public Function VerificaCorrettezzaPassword(ByVal username As String, ByVal passwordEsterna As String, ByVal gestioneHashAbilitata As Boolean,
                                                 ByRef objParametri_Utenti As AgronicaCoreParametri, Optional ByVal passwordDB_Ext As String = "",
                                                 Optional ByVal isPasswordDBHashed_Ext As Integer = 0,
                                                 Optional ByRef outPasswordCorretta As String = "") As Boolean
        Dim risp As Boolean = False

        Dim objUtenti As New Utenti_Read
        Dim objSicurezza As New Sicurezza

        Dim passwordDB As String = ""
        Dim isPasswordDBHashed As Integer = 0

        If Not passwordDB_Ext = "" Then
            passwordDB = passwordDB_Ext
            isPasswordDBHashed = isPasswordDBHashed_Ext
        Else
            Dim dtUtenti = objUtenti.Leggi2(username, "", "", objParametri_Utenti)
            If Not dtUtenti.Rows.Count = 1 Then
                'Errore
            End If
            passwordDB = dtUtenti.Rows(0)("Password")
            isPasswordDBHashed = dtUtenti.Rows(0)("Flag_Encrypted")
        End If

        Dim isPasswordEsternaHashed As Boolean = objSicurezza.OttieniBytesDaHash(passwordEsterna) IsNot Nothing

        outPasswordCorretta = passwordDB

        If isPasswordDBHashed = 1 Then

            Dim hashConfronto As String
            If isPasswordEsternaHashed = True Then
                hashConfronto = passwordEsterna
            Else
                hashConfronto = objSicurezza.GeneraHashConfronto(passwordDB, passwordEsterna)
            End If

            If hashConfronto = passwordDB Then
                risp = True
            End If
        Else
            If isPasswordEsternaHashed = False AndAlso passwordEsterna = passwordDB Then
                risp = True

                If gestioneHashAbilitata = True Then
                    'Sovrascrivo la password con il suo hash
                    Dim hashPassword = objSicurezza.GeneraNuovoHash(passwordEsterna)
                    Dim objUtentiWrite As New Utenti_Write()
                    objUtentiWrite.Modifica(username, hashPassword, gestioneHashAbilitata, True, objParametri_Utenti)

                    outPasswordCorretta = hashPassword
                End If
            End If
        End If

        Return risp
    End Function

    Public Sub ASG_Autenticazione_Utente_verificaPPT(
                ByRef x_Username_Utente As String,
                ByRef x_Password_Utente As String,
                ByVal hashPasswordAbilitato As Boolean,
                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim urlWS As String

        Dim x_Password_Corretta As String = ""

        Try

            Dim isPasswordRight = VerificaCorrettezzaPassword(x_Username_Utente, x_Password_Utente, hashPasswordAbilitato, objParametri_Utenti,
                                    "", 0, x_Password_Corretta)


            ' se la pass è ok,nessuna chiamata a web service.
            If isPasswordRight = True Then
                'Imposto la password passata per riferimento al suo hash per successivi confronti
                x_Password_Utente = x_Password_Corretta
                Exit Sub
            End If

            Dim ci As ConstructorInfo
            ' recupero il tipo della classe ascoltatrice dell'evento
            Dim ty As Type = Type.GetType("AgronicaCoreVarieDAL.Configurazione_Siti_R, AgronicaCoreVarieDAL")
            ' Invoco il suo costruttore senza parametri e chiamo il metodo gestisciEvento sull'oggetto creato
            ci = ty.GetConstructor(System.Type.EmptyTypes)

            Dim ConfSitiRConstructor As ConstructorInfo = ty.GetConstructor(Type.EmptyTypes)
            Dim ConfSitiRClassObject As Object = ConfSitiRConstructor.Invoke(New Object() {})

            Dim params() As Object = {} 'preparo l'array vuoto di parametri
            Dim mm As MethodInfo = ty.GetMethod("Leggi_Valore")
            urlWS = mm.Invoke(ConfSitiRClassObject, New Object() {6, "GiasOnline_WS_GiasWSAggiorna_AgroWS_GiasWSAggiorna", "", "", objParametri_Server})


            'urlWS = vDal.Leggi_Valore(6, "GiasOnline_WS_GiasWSAggiorna_AgroWS_GiasWSAggiorna", "", "", objParametri_Server)
        Catch ex As Exception
            Exit Sub
        End Try


        Dim xWS As New ws_ppt.WS_Ppt
        If urlWS <> "" Then
            xWS.Url = urlWS.Replace("Gias_service.asmx", "Ws_Ppt.asmx")
        End If

        'lavez - 16/03/2022 - disattivato perchè in cloud va in timeout n volte se il pool è spento
        'xWS.Timeout = 4000

        Dim autenticatoWS As Boolean = False
        Try
            Dim x_Password_Utentecript As String = Sicurezza.Stringa_Codifica_LANCompatibile(x_Password_Utente, CostantiPersonalizzate.AgroKey_EncoderDecoder)
            autenticatoWS = xWS.ppt(x_Password_Utentecript)
        Catch ex As Exception
            Exit Sub
        End Try


        If autenticatoWS Then
            x_Password_Utente = x_Password_Corretta
        End If
    End Sub

    'Verifica PPT
    Public Function ASG_Verifica_PPT(ByRef x_Password_Utente As String,
                                     ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim autenticatoWS As Boolean = False

        Dim urlWS As String

        Try

            Dim ci As ConstructorInfo

            'Recupero il tipo della classe ascoltatrice dell'evento
            Dim ty As Type = Type.GetType("AgronicaCoreVarieDAL.Configurazione_Siti_R, AgronicaCoreVarieDAL")

            'Invoco il suo costruttore senza parametri e chiamo il metodo gestisciEvento sull'oggetto creato
            ci = ty.GetConstructor(System.Type.EmptyTypes)

            Dim ConfSitiRConstructor As ConstructorInfo = ty.GetConstructor(Type.EmptyTypes)
            Dim ConfSitiRClassObject As Object = ConfSitiRConstructor.Invoke(New Object() {})

            Dim params() As Object = {} 'preparo l'array vuoto di parametri
            Dim mm As MethodInfo = ty.GetMethod("Leggi_Valore")
            urlWS = mm.Invoke(ConfSitiRClassObject, New Object() {6, "GiasOnline_WS_GiasWSAggiorna_AgroWS_GiasWSAggiorna", "", "", objParametri_Server})

        Catch ex As Exception
            Return False
        End Try

        Dim xWS As New ws_ppt.WS_Ppt
        If urlWS <> "" Then
            xWS.Url = urlWS.Replace("Gias_service.asmx", "Ws_Ppt.asmx")
        End If

        Try
            Dim x_Password_Utentecript As String = Sicurezza.Stringa_Codifica_LANCompatibile(x_Password_Utente, CostantiPersonalizzate.AgroKey_EncoderDecoder)
            autenticatoWS = xWS.ppt(x_Password_Utentecript)
        Catch ex As Exception
            Return False
        End Try

        Return autenticatoWS

    End Function

    'Letturea Chiave WS e Gestione Update
    Public Function ASG_GestioneChiave_PPT(ByRef Data_Scadenza As Date, ByRef Chiave_Locale As String, ByRef Chiave_WS As String, ByRef bChiave_Aggiornata As Boolean, ByVal urlWS As String, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim yWS As New ws_ppt.WS_Ppt

        Try

            'Lettura chiave in locale           
            Dim objUtenti_Codici_R As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
            Dim dtCodice As DataTable = objUtenti_Codici_R.LeggiChiave(objParametri_Utenti.SuperUserUsername, "", objParametri_Utenti)
            Chiave_Locale = dtCodice.Rows(0)("GiasOnline_Key").ToString

            'Lettura chiave WS
            yWS.Url = urlWS.Replace("Gias_service.asmx", "Ws_Ppt.asmx")
            'yWS.Url = "http://localhost/AgronicaWebService/Ws_Ppt.asmx"
            'yWS.Url = "Test Errore"

            'lavez - 16/03/2022 - disattivato perchè in cloud va in timeout n volte se il pool è spento
            'yWS.Timeout = 4000
            Chiave_WS = yWS.LeggiChiave(objParametri_Utenti.SuperUserUsername, enum_Id_Servizio.GiasOnline)

            ASG_GestioneChiave_PPT = True

            '3. Controllo Disallineamento
            If Chiave_WS <> Chiave_Locale Then

                'Aggiornamento Chiave Locale
                Dim objUtenti_Codici_W As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPro_W
                objUtenti_Codici_W.AggiornaChiave(Chiave_WS, objParametri_Utenti)

                bChiave_Aggiornata = True
                Chiave_Locale = Chiave_WS

            End If

            ASG_GestioneChiave_PPT = True


        Catch ex As Exception

            ASG_GestioneChiave_PPT = False

        Finally


        End Try

        Return ASG_GestioneChiave_PPT


    End Function

    '.Verifica Scadenza Chiave, Azioni e Ritorno Messaggio di Allerta
    Public Function ASG_VerificaChiave(ByVal Chiave_Locale As String, ByVal bChiave_Aggiornata As Boolean, ByVal bBlocco_Precedente As Boolean, ByRef GiorniFranchigia_Rimanenti As Integer, ByRef sms As String, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim GiorniFranchigia As Integer = 0
        Dim auK As New AgronicaCoreUtentiDAL.AutenticaUtente
        Dim Data_Scadenza As Date = AGRODATAINIZIO
        Dim objUtenti_Permessi_W As New AgronicaCoreUtentiDAL.Utenti_Permessi_W


        Try
            ASG_VerificaChiave = True
            GiorniFranchigia_Rimanenti = -1 'Dummy per Chiave Ok


            'Controllo Chiave Non Ancora Impostata
            If Chiave_Locale = "" Then

                auK.ASG_LeggiChiave(Data_Scadenza, objParametri_Utenti)

            Else

                'Determinazione Scadenza da Chiave Locale
                auK.ASG_LeggiChiaveScadenza(Chiave_Locale, Data_Scadenza, objParametri_Utenti)

            End If


            '. Controllo se la data è valida ed evenutale impostazione gg fracnihgia                    
            If (Date.Today > Data_Scadenza) Then

                'Lettura Giorni Franchigia
                Dim ObjUtenti_Impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim dtImpostazioni As DataTable = ObjUtenti_Impostazioni.Leggi(enum_Impostazioni_Utenti.SUPERUSER_Licenza_Giorni_Franchigia, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

                If dtImpostazioni.Rows.Count > 0 Then

                    If IsNumeric(dtImpostazioni.Rows(0)("Impostazione_Valore_1")) Then

                        GiorniFranchigia = dtImpostazioni.Rows(0)("Impostazione_Valore_1")

                        If GiorniFranchigia > 60 Then
                            GiorniFranchigia = 60
                        End If

                        Data_Scadenza = DateAdd("d", GiorniFranchigia, Data_Scadenza)


                    End If

                    If IsNumeric(dtImpostazioni.Rows(0)("Impostazione_Valore_2")) Then

                        bBlocco_Precedente = CBool(dtImpostazioni.Rows(0)("Impostazione_Valore_2"))

                    Else

                        bBlocco_Precedente = True


                    End If

                End If

                '. Nuvovo Controllo Validità Compresa Franchigia                
                If (Date.Today > Data_Scadenza) Then

                    'Blocco Operazioni (Da 2 --> 9)
                    objUtenti_Permessi_W.Blocca_Permessi(enum_Id_Servizio.GiasOnline, "Chiave Scaduta", objParametri_Utenti)

                    sms = "Attenzione. La chiave è scaduta. Gias ONLINE sarà utilizzabile esclusivamente per la consultazione dei dati!."

                    'Chiave Scaduta
                    ASG_VerificaChiave = False

                Else

                    If GiorniFranchigia <> 0 Then

                        GiorniFranchigia_Rimanenti = DateDiff("d", Date.Today, Data_Scadenza)
                    Else
                        GiorniFranchigia_Rimanenti = 0

                    End If


                End If

            End If


            ' Controllo Rispristino Operazioni per Chiave Aggiornata ed ora Valida
            If (Date.Today <= Data_Scadenza) And (bChiave_Aggiornata Or bBlocco_Precedente) Then

                'Ripristino Operazioni (Da 9 --> 2)
                objUtenti_Permessi_W.Ripristino_Permessi(enum_Id_Servizio.GiasOnline, objParametri_Utenti)

            End If


        Catch ex As Exception

            'Blocco Operazioni (Da 2 --> 9)
            objUtenti_Permessi_W.Blocca_Permessi(enum_Id_Servizio.GiasOnline, ex.Message, objParametri_Utenti)

            sms = "Attenzione. La chiave non è valida. Gias ONLINE sarà utilizzabile esclusivamente per la consultazione dei dati!."

            ASG_VerificaChiave = False

        Finally


        End Try

        Return ASG_VerificaChiave

    End Function


    'Public Function ASG_AggiornaChiave_PPT(ByRef Data_Scadenza As Date, ByVal urlWS As String, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional ByVal bRead As Boolean = False) As Boolean

    '    Dim Chiave_Criptata As String = ""
    '    Dim Chiave As String = ""
    '    Dim yWS As New ws_ppt.WS_Ppt

    '    Try

    '        yWS.Url = urlWS.Replace("Gias_service.asmx", "Ws_Ppt.asmx")

    '        'yWS.Url = "http://localhost/AgronicaWebService/Ws_Ppt.asmx"

    '        yWS.Timeout = 4000

    '        'Lettura Chiave WS
    '        Chiave = yWS.LeggiChiave(objParametri_Utenti.SuperUserUsername, 5)

    '        'Ricavo la Data di Scadenza
    '        Select Case bRead

    '            Case True

    '                If Trim(Chiave) <> "" Then

    '                    Dim UsernameOUT As String = ""
    '                    Dim CodiceProgressivoGIAS As Integer
    '                    Dim DataAttivazione As Date
    '                    Dim NumeroAziende As Integer
    '                    Dim NumeroUtenti As Integer
    '                    Dim SuperficieTotale As Integer
    '                    Dim NumeroAccessi As Integer
    '                    Dim NumeroUtilizzi As Integer
    '                    Dim DataGenerazioneChiave As Date
    '                    Dim VersioneCodifica As Integer
    '                    Dim VersioneChiave As Integer
    '                    Dim Modulo_Flag() As Integer
    '                    Dim Modulo_Inizio() As Date
    '                    Dim Modulo_Fine() As Date
    '                    Dim Checksum As String
    '                    Dim MessaggioErrore As String
    '                    Dim Risultato As Boolean = False


    '                    Dim AgroDecodifica As New AgronicaCoreDataProvider.Sicurezza
    '                    Risultato = AgroDecodifica.ChiaveGiasOnline_Decodifica(
    '                                                Chiave,
    '                                                objParametri_Utenti.SuperUserUsername,
    '                                                UsernameOUT,
    '                                                CodiceProgressivoGIAS,
    '                                                DataAttivazione,
    '                                                Data_Scadenza,
    '                                                NumeroAziende,
    '                                                NumeroUtenti,
    '                                                SuperficieTotale,
    '                                                NumeroAccessi,
    '                                                NumeroUtilizzi,
    '                                                DataGenerazioneChiave,
    '                                                VersioneCodifica,
    '                                                VersioneChiave,
    '                                                Modulo_Flag,
    '                                                Modulo_Inizio,
    '                                                Modulo_Fine,
    '                                                Checksum,
    '                                                MessaggioErrore)

    '                Else

    '                End If

    '            Case False

    '                'Aggiornamento Chiave Locale
    '                Dim objUtenti_Codici_W As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPro_W
    '                objUtenti_Codici_W.AggiornaChiave(Chiave, objParametri_Utenti)


    '        End Select


    '        ASG_AggiornaChiave_PPT = True


    '    Catch ex As Exception

    '        ASG_AggiornaChiave_PPT = False

    '        'Lettura Giorni Franchigia
    '        Dim ObjUtenti_Codici As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
    '        Dim GiorniFranchigia As Integer = 0

    '        Dim dtCodice As DataTable = ObjUtenti_Codici.Leggi(enum_Impostazioni_Utenti.SUPERUSER_Licenza_Giorni_Franchigia, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

    '        If dtCodice.Rows.Count > 0 Then

    '            If IsNumeric(dtCodice.Rows(0)("Impostazione_Valore_1")) Then

    '                GiorniFranchigia = dtCodice.Rows(0)("Impostazione_Valore_1")

    '                Data_Scadenza = DateAdd("d", GiorniFranchigia, Data_Scadenza)

    '                'Controllo se la data è valida                    
    '                If (Date.Today > Data_Scadenza) Then

    '                    'I Giorni di franchigia non sono sufficienti. Verrà effettuato un nuovo controllo che andrà in "licenza scaduta"
    '                    ASG_AggiornaChiave_PPT = True

    '                Else

    '                    'I giorni di franchigia sono sufficienti. Verrà visualizzato un msg di allerta.


    '                End If



    '            End If

    '        End If




    '    Finally


    '    End Try

    '    Return ASG_AggiornaChiave_PPT


    'End Function


    Public Sub ASG_LeggiChiave(ByRef Data_Scadenza As Date, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        'Controllo la chiave in locale per verificare la scadenza
        Dim objUtenti_Codici As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Dim bLeggiChiaveWS As Boolean = False
        Dim Chiave_ONLINE As String = ""
        Dim Risultato As Boolean = False

        'Lettura Chiave
        Dim dtCodice As DataTable = objUtenti_Codici.LeggiChiave(objParametri_Utenti.SuperUserUsername, "", objParametri_Utenti)


        If dtCodice.Rows.Count > 0 Then

            Dim UsernameOUT As String = ""
            Dim CodiceProgressivoGIAS As Integer
            Dim DataAttivazione As Date
            Dim NumeroAziende As Integer
            Dim NumeroUtenti As Integer
            Dim SuperficieTotale As Integer
            Dim NumeroAccessi As Integer
            Dim NumeroUtilizzi As Integer
            Dim DataGenerazioneChiave As Date
            Dim VersioneCodifica As Integer
            Dim VersioneChiave As Integer
            Dim Modulo_Flag() As Integer
            Dim Modulo_Inizio() As Date
            Dim Modulo_Fine() As Date
            Dim Checksum As String
            Dim MessaggioErrore As String

            Chiave_ONLINE = dtCodice.Rows(0)("GiasOnline_Key").ToString

            '---------------------------------------------------------------------------
            '----- Decodifico ...
            '--------------------------------------------------------------------------            

            If Trim(Chiave_ONLINE) <> "" Then

                Dim AgroDecodifica As New AgronicaCoreDataProvider.Sicurezza
                Risultato = AgroDecodifica.ChiaveGiasOnline_Decodifica(
                                            Chiave_ONLINE,
                                            objParametri_Utenti.SuperUserUsername,
                                            UsernameOUT,
                                            CodiceProgressivoGIAS,
                                            DataAttivazione,
                                            Data_Scadenza,
                                            NumeroAziende,
                                            NumeroUtenti,
                                            SuperficieTotale,
                                            NumeroAccessi,
                                            NumeroUtilizzi,
                                            DataGenerazioneChiave,
                                            VersioneCodifica,
                                            VersioneChiave,
                                            Modulo_Flag,
                                            Modulo_Inizio,
                                            Modulo_Fine,
                                            Checksum,
                                            MessaggioErrore)



            End If

        End If


    End Sub



    Public Sub ASG_LeggiChiaveScadenza(ByVal Chiave As String, ByRef Data_Scadenza As Date, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim bLeggiChiaveWS As Boolean = False
        Dim Risultato As Boolean = False



        Dim UsernameOUT As String = ""
        Dim CodiceProgressivoGIAS As Integer
        Dim DataAttivazione As Date
        Dim NumeroAziende As Integer
        Dim NumeroUtenti As Integer
        Dim SuperficieTotale As Integer
        Dim NumeroAccessi As Integer
        Dim NumeroUtilizzi As Integer
        Dim DataGenerazioneChiave As Date
        Dim VersioneCodifica As Integer
        Dim VersioneChiave As Integer
        Dim Modulo_Flag() As Integer
        Dim Modulo_Inizio() As Date
        Dim Modulo_Fine() As Date
        Dim Checksum As String
        Dim MessaggioErrore As String

        '---------------------------------------------------------------------------
        '----- Decodifico ...
        '---------------------------------------------------------------------------

        If Trim(Chiave) <> "" Then

            Dim AgroDecodifica As New AgronicaCoreDataProvider.Sicurezza
            Risultato = AgroDecodifica.ChiaveGiasOnline_Decodifica(
                                            Chiave,
                                            objParametri_Utenti.SuperUserUsername,
                                            UsernameOUT,
                                            CodiceProgressivoGIAS,
                                            DataAttivazione,
                                            Data_Scadenza,
                                            NumeroAziende,
                                            NumeroUtenti,
                                            SuperficieTotale,
                                            NumeroAccessi,
                                            NumeroUtilizzi,
                                            DataGenerazioneChiave,
                                            VersioneCodifica,
                                            VersioneChiave,
                                            Modulo_Flag,
                                            Modulo_Inizio,
                                            Modulo_Fine,
                                            Checksum,
                                            MessaggioErrore)




        End If


    End Sub

    Public Sub ASG_Autenticazione_Utente_viaToken(
            ByVal token As String,
            ByRef x_Username_Utente As String,
            ByRef x_Password_Utente As String,
            ByRef x_codice_GIAS As Integer,
            ByRef TokenParametriByRef As TokenParametri,
            ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        )

        Dim xEccezioneNota As Boolean = False

        Try

            Dim xLeggiToken As New Utenti_Token_R
            Dim dtToken As DataTable
            dtToken = xLeggiToken.Leggi_Token(token, "", objParametri_Super_Server)

            'se il token non esiste esco
            If dtToken.Rows.Count = 0 Then
                xEccezioneNota = True
                Throw New Exception("Token non valido")
            End If

            Dim cfgTokenParametri As String = dtToken.Rows(0)("Parametri")
            Dim superUserUsername As String = dtToken.Rows(0)("superUser_Username")
            Dim utenteUsername As String = dtToken.Rows(0)("utente_Username")
            Dim dataOraRilascioToken As DateTime = dtToken.Rows(0)("data_rilascio")

            'verifica la validita del token
            Dim xLeggiValiditaToken As New Utenti_Validita_Token_R
            Dim dtLeggiValiditaToken As DataTable
            dtLeggiValiditaToken = xLeggiValiditaToken.Leggi_Impostazioni_Abusi_AWS(superUserUsername, "", objParametri_Super_Server)

            If dtLeggiValiditaToken.Rows.Count = 0 Then
                xEccezioneNota = True
                Throw New Exception("Nessuna impostazione di validità token trovata")
            End If

            Dim Adesso As DateTime = Now
            Dim validitaMinuti As Integer = dtLeggiValiditaToken.Rows(0)("Validita_Minuti")

            If validitaMinuti > 0 Then
                'se il token è scaduto esco
                If DateDiff(DateInterval.Minute, dataOraRilascioToken, Adesso) > validitaMinuti Then
                    xEccezioneNota = True
                    Throw New Exception("Token Scaduto")
                End If
            End If


            Dim objAgronicaCore As New DataProvider

            x_Username_Utente = utenteUsername

            TokenParametriByRef =
                Newtonsoft.Json.JsonConvert.DeserializeObject(Of AgronicaCoreUtentiDAL.TokenParametri)(cfgTokenParametri)

            Dim xStrPath2Ini As String = ""
            If Not IsNumeric(TokenParametriByRef.idDB_Server) Then
                xStrPath2Ini = CostantiPersonalizzate.PathFileINI
            End If
            Dim StringaConnessione_Server As String = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(
                xStrPath2Ini, TokenParametriByRef.idDB_Server)

            Dim StringaConnessione_Utenti As String = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(
                xStrPath2Ini, TokenParametriByRef.idDB_Utenti)


            Dim objParametriHLP As New AgronicaCoreParametri_Helper

            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = objParametriHLP.Crea_ObjParametri(New DateTime(&H851055320574000), New DateTime(&H9325D82E8380000), enumCancellazioneLogica.CancellazioneFisica, enumVisibilita.Visibilita_Tutti, "", "", superUserUsername, "", utenteUsername, "", StringaConnessione_Utenti)


            Dim objUtenti As New Utenti_Read
            x_Password_Utente = objUtenti.Password_From_UserName(x_Username_Utente, objParametri_Utenti)

            Dim objCodGias As New Utenti_CodiciGiasPRO_R
            x_codice_GIAS = objCodGias.ProgressivoGias_from_Superuser(objParametri_Utenti)

        Catch ex As Exception
            Dim MessaggioErrore As String = ""
            If xEccezioneNota Then
                MessaggioErrore = ex.Message
            Else
                MessaggioErrore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            End If
            Throw New Exception("[Autentica_viaToken] : " & MessaggioErrore, ex)
        End Try

    End Sub

    Public Sub ASG_Autenticazione_Utente_viaToken(
            ByVal token As String,
            ByRef x_Username_Utente As String,
            ByRef x_Password_Utente As String,
            ByRef ID_DB_Sel As Integer,
            ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal LoginWithCUAAByDemetra As Boolean = False
        )

        Dim log As New LogProvider
        Dim xEccezioneNota As Boolean = False

        Try


            Dim xLeggiToken As New Utenti_Token_R
            Dim dtToken As DataTable
            dtToken = xLeggiToken.Leggi_Token(token, "", objParametri_Super_Server)

            'se il token non esiste esco
            If dtToken.Rows.Count = 0 Then
                xEccezioneNota = True
                If LoginWithCUAAByDemetra Then
                    Throw New AgroEccezioni_LoginFallito_Exception("Token non valido")
                Else
                    Throw New Exception("Token non valido")
                End If
            End If

            Dim cfgTokenParametri As String = dtToken.Rows(0)("Parametri")
            Dim superUserUsername As String = dtToken.Rows(0)("superUser_Username")
            Dim utenteUsername As String = dtToken.Rows(0)("utente_Username")
            Dim dataOraRilascioToken As DateTime = dtToken.Rows(0)("data_rilascio")
            Dim fr_cod As Integer = dtToken.Rows(0)("Fr_Cod")

            'verifica la validita del token
            Dim xLeggiValiditaToken As New Utenti_Validita_Token_R
            Dim dtLeggiValiditaToken As DataTable
            dtLeggiValiditaToken = xLeggiValiditaToken.Leggi_Impostazioni_Abusi_AWS(superUserUsername, "", objParametri_Super_Server)

            If dtLeggiValiditaToken.Rows.Count = 0 Then
                xEccezioneNota = True
                Throw New Exception("Nessuna impostazione di validità token trovata")
            End If

            Dim Adesso As DateTime = Now
            Dim validitaMinuti As Integer = dtLeggiValiditaToken.Rows(0)("Validita_Minuti")

            If validitaMinuti > 0 Then
                'se il token è scaduto esco
                If DateDiff(DateInterval.Minute, dataOraRilascioToken, Adesso) > validitaMinuti Then
                    xEccezioneNota = True
                    Throw New Exception("Token Scaduto")
                End If
            End If

            'lavez - 07/02/2024 - invalidazione alla login (token mono uso)
            If LoginWithCUAAByDemetra Then
                Dim xScriviToken As New Utenti_Token_W

                If xScriviToken.Modifica_Token(Guid.NewGuid().ToString(), superUserUsername, utenteUsername, fr_cod, "", objParametri_Super_Server) = False Then
                    Throw New AgroEccezioni_LoginFallito_Exception("Errore invalidazione token")
                End If
            End If

            Dim objAgronicaCore As New DataProvider

            If String.IsNullOrEmpty(x_Username_Utente) Then
                x_Username_Utente = utenteUsername
            End If

            Dim oTokenParametri As AgronicaCoreUtentiDAL.TokenParametri =
                Newtonsoft.Json.JsonConvert.DeserializeObject(Of AgronicaCoreUtentiDAL.TokenParametri)(cfgTokenParametri)

            Dim StringaConnessione_Server As String = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(
                "", oTokenParametri.idDB_Server)

            Dim StringaConnessione_Utenti As String = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(
                "", oTokenParametri.idDB_Utenti)

            ID_DB_Sel = oTokenParametri.idDB_Server

            Dim objParametriHLP As New AgronicaCoreParametri_Helper

            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = objParametriHLP.Crea_ObjParametri(New DateTime(&H851055320574000), New DateTime(&H9325D82E8380000), enumCancellazioneLogica.CancellazioneFisica, enumVisibilita.Visibilita_Tutti, "", "", superUserUsername, "", utenteUsername, "", StringaConnessione_Utenti)


            Dim objUtenti As New Utenti_Read
            x_Password_Utente = objUtenti.Password_From_UserName(x_Username_Utente, objParametri_Utenti)

        Catch ex As AgroEccezioni_LoginFallito_Exception

            log.Scrivi_LOG(objParametri_Super_Server, "ASG_Autenticazione_Utente_viaToken", ex.Message, False)
            If (ex.InnerException IsNot Nothing) Then
                log.Scrivi_LOG(objParametri_Super_Server, "ASG_Autenticazione_Utente_viaToken", ex.InnerException.Message, False)
            End If
            Throw New AgroEccezioni_LoginFallito_Exception(ex.Message)

        Catch ex As Exception

            log.Scrivi_LOG(objParametri_Super_Server, "ASG_Autenticazione_Utente_viaToken", ex.Message, False)
            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = ""

            If xEccezioneNota Then
                MessaggioErrore = ex.Message
            Else
                MessaggioErrore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            End If


            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[Autentica_viaToken] : " & MessaggioErrore, ex)

        End Try


    End Sub


    Public Class TokenSuperUser
        Public Piva_SuperUser As String
        Public Username_SuperUser As String
    End Class
    Public Function ASG_Autenticazione_Utente_viaToken(token As String, objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As TokenSuperUser

        Dim objToken As TokenSuperUser = Nothing

        Dim xEccezioneNota As Boolean = False

        Try

            Dim xLeggiToken As New Utenti_Token_R
            Dim dtToken As DataTable = xLeggiToken.Leggi_Token(token, "", objParametri_Super_Server)

            'se il token non esiste esco
            If dtToken.Rows.Count = 0 Then
                xEccezioneNota = True
                Throw New Exception("Token non valido")
            End If

            'Dim cfgTokenParametri As String = dtToken.Rows(0)("Parametri")
            Dim superUserUsername As String = dtToken.Rows(0)("superUser_Username")
            'Dim utenteUsername As String = dtToken.Rows(0)("utente_Username")
            Dim dataOraRilascioToken As DateTime = dtToken.Rows(0)("data_rilascio")

            'verifica la validita del token
            Dim xLeggiValiditaToken As New Utenti_Validita_Token_R
            Dim dtLeggiValiditaToken As DataTable = xLeggiValiditaToken.Leggi_Impostazioni_Abusi_AWS(superUserUsername, "", objParametri_Super_Server)

            If dtLeggiValiditaToken.Rows.Count = 0 Then
                xEccezioneNota = True
                Throw New Exception("Nessuna impostazione di validità token trovata")
            End If

            Dim Adesso As DateTime = Now
            Dim validitaMinuti As Integer = dtLeggiValiditaToken.Rows(0)("Validita_Minuti")

            If validitaMinuti > 0 Then
                'se il token è scaduto esco
                If DateDiff(DateInterval.Minute, dataOraRilascioToken, Adesso) > validitaMinuti Then
                    xEccezioneNota = True
                    Throw New Exception("Token Scaduto")
                End If
            End If

            objToken = New TokenSuperUser With {
                .Piva_SuperUser = dtToken.Rows(0)("PivaSuperUser"),
                .Username_SuperUser = dtToken.Rows(0)("SuperUser_Username")
            }

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = ""

            If xEccezioneNota Then
                MessaggioErrore = ex.Message
            Else
                MessaggioErrore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            End If

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[Autentica_viaToken] : " & MessaggioErrore, ex)
        End Try

        Return objToken
    End Function

    '############################################################################

    ''' <param name="authWithSSO">Specify if the user is authenticating via SSO. If <tt>True</tt>, skip the password verification.</param>
    Public Function ASG_Autenticazione_Utente(
        ByVal Username_Utente As String,
        ByVal Password_Utente As String,
        ByVal Id_Attivita As Integer,
        ByVal Id_Operazione As Integer,
        ByVal Flag_Tunnel As Boolean,
        ByVal gestioneHashAbilitata As Boolean,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal xFiltroAggiuntivo As String = "",
        Optional authWithSSO As Boolean = False
    ) As String
        Dim x_UserName_Utente, x_Password_Utente, x_Utente_CodFiscale As String
        Dim Messaggio As String = ""
        Dim Flag_AccessoNegato As Boolean

        '----- Recupero i valori digitati dall'utente
        x_UserName_Utente = Username_Utente
        x_Password_Utente = Password_Utente

        '----- Verifico che nessuno dei due sia nullo ...
        If (x_UserName_Utente = "") Or (x_Password_Utente = "") Then
            Return Gias.InserireUsernamePassword
        End If

        Dim passwordCorretta As String = ""

        Dim DT As DataTable
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read

        DT = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(x_UserName_Utente, "",
                                                    "",
                                                    Id_Operazione,
                                                    Id_Attivita,
                                                    Date.Now,
                                                    CType(Now.Hour, Short),
                                                    True,
                                                    5, xFiltroAggiuntivo, "", objParametri_Utenti)
        If IsNothing(DT) Then
            'la query ha dato errore
            Flag_AccessoNegato = True
        ElseIf DT.Rows.Count = 1 Then
            'Autentico l'utente
            Dim passwordDB As String = DT.Rows(0)("Password")
            Dim isPasswordDBHashed As Integer = DT.Rows(0)("Flag_Encrypted")
            x_Utente_CodFiscale = DT.Rows(0)("CodFisc")

            Flag_AccessoNegato = Not authWithSSO AndAlso Not VerificaCorrettezzaPassword(
                x_UserName_Utente, x_Password_Utente, gestioneHashAbilitata,
                objParametri_Utenti, passwordDB, isPasswordDBHashed, passwordCorretta
            )
        Else
            'no permesso lettura menù
            'opp
            'no profilo
            Flag_AccessoNegato = True
        End If

        '########################################################

        If Flag_AccessoNegato Then
            '---------------------------------------------------
            '----- Pulisco le variabili di objSessione ------------
            '---------------------------------------------------
            'ASG_SessioneStarGate_Utente_Azzera(objServer, objSession, objPage)
            '---------------------------------------------------
            '----- Ridirigo l'utente all'uscita ----------------
            '---------------------------------------------------
            Messaggio = Gias.CredenzialiErrateMancanzaPermessi
            'Imposta_MsgErrore(objSession, Flag_Tunnel, Messaggio)
            'objResponse.Redirect(URL_Redirect_Errore)
            '---------------------------------------------------
        Else
#Region "Salvo i parametri utente in sessione"

            Messaggio = InitSession(objParametri_Server, objParametri_Utenti, DT, x_UserName_Utente, passwordCorretta)

            '-----------------------------------------------------------------------
            '---- Leggo e memorizzo la Finestra temporale dell'utente --------------
            '-----------------------------------------------------------------------
            Dim DT_FinestraTemp As DataTable
            DT_FinestraTemp = objUtenti.Leggi_Da_Gias_Server(x_UserName_Utente,
                                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "", "", objParametri_Server)

            'DT_FinestraTemp = NewCom_Utenti_GiasServer_Leggi(objServer, objSession, objPage, CStr(x_UserName_Utente))

            If IsNothing(DT_FinestraTemp) Then
                'errore query finestra temp
                HttpContext.Current.Session("ASG_FinestraTemporale_Inizio") = Estremo_Validita_Inizio
                HttpContext.Current.Session("ASG_FinestraTemporale_Fine") = Estremo_Validita_Fine
            ElseIf DT_FinestraTemp.Rows.Count <> 0 Then

                '###############################
                '##### Il record ESISTE ########
                '###############################

                'Leggo le date
                If Not IsDate(DT_FinestraTemp.Rows(0).Item("FinestraTemp_Inizio")) Then
                    HttpContext.Current.Session("ASG_FinestraTemporale_Inizio") = Estremo_Validita_Inizio
                Else
                    HttpContext.Current.Session("ASG_FinestraTemporale_Inizio") = CDate(DT_FinestraTemp.Rows(0).Item("FinestraTemp_Inizio"))
                End If

                If Not IsDate(DT_FinestraTemp.Rows(0).Item("FinestraTemp_Fine")) Then
                    HttpContext.Current.Session("ASG_FinestraTemporale_Fine") = Estremo_Validita_Fine
                Else
                    HttpContext.Current.Session("ASG_FinestraTemporale_Fine") = CDate(DT_FinestraTemp.Rows(0).Item("FinestraTemp_Fine"))
                End If

            Else
                '################################
                '##### Il record NON ESISTE #####
                '################################

                Dim NumeroRecordInteressati As Integer
                Dim objUtentiW As New AgronicaCoreUtentiDAL.Utenti_Write

                objUtentiW.Utenti_GiasServer_Scrivi(NumeroRecordInteressati,
                                                x_UserName_Utente,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                x_Utente_CodFiscale,
                                                "", 0, Date.Today, 0, 0, "", 0,
                                                AGRODATAINIZIO, AGRODATAFINE,
                                                objParametri_Server)

                HttpContext.Current.Session("ASG_FinestraTemporale_Inizio") = Estremo_Validita_Inizio
                HttpContext.Current.Session("ASG_FinestraTemporale_Fine") = Estremo_Validita_Fine
            End If

            objParametri_Server.FinestraTemporaleInizio = HttpContext.Current.Session("ASG_FinestraTemporale_Inizio")
            objParametri_Utenti.FinestraTemporaleInizio = HttpContext.Current.Session("ASG_FinestraTemporale_Inizio")

            objParametri_Server.FinestraTemporaleFine = HttpContext.Current.Session("ASG_FinestraTemporale_Fine")
            objParametri_Utenti.FinestraTemporaleFine = HttpContext.Current.Session("ASG_FinestraTemporale_Fine")

            '--------------------------------------------------------
            '--------------  UTENTI CONNESSI ------------------------
            '--------------------------------------------------------
            If Not TableHashUtentiConnessi.ContainsKey(objParametri_Server.UtenteUsername) Then
                TableHashUtentiConnessi.Add(objParametri_Server.UtenteUsername, "")
            End If
#End Region
        End If

        Return Messaggio
    End Function

    Private Function InitSession(
        objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri,
        DT As DataTable, x_UserName_Utente As String, passwordCorretta As String
    ) As String
        If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
            Throw New InvalidOperationException
        End If

        '-----------------------------------------------------------------
        '----- Memorizzo la LINGUA dell'utente -----------
        '-----------------------------------------------------------------
        Dim linguaCodice = DT.Rows(0).Item("Lingua_Cod")
        HttpContext.Current.Session("LinguaCorrente") = linguaCodice
        objParametri_Server.Lingua_Cod = linguaCodice
        objParametri_Utenti.Lingua_Cod = linguaCodice

        '-----------------------------------------------------------------
        '----- Memorizzo la USERNAME e la PASSWORD dell'utente -----------
        '-----------------------------------------------------------------

        HttpContext.Current.Session("ASG_Utente_Username") = x_UserName_Utente
        HttpContext.Current.Session("ASG_Utente_Password") = passwordCorretta

        '---------------------------------------------------------
        '----- Memorizzo la USERNAME E PASSWORD del SUPERUSER ----
        '---------------------------------------------------------

        Dim x_UserName_SuperUser As String = DT.Rows(0).Item("Utente_Profilo")
        Dim x_Password_SuperUser As String = DT.Rows(0).Item("Password_SuperUser")

        HttpContext.Current.Session("ASG_SuperUser_Username") = x_UserName_SuperUser
        HttpContext.Current.Session("ASG_SuperUser_Password") = x_Password_SuperUser

        objParametri_Server.SuperUserUsername = x_UserName_SuperUser
        objParametri_Utenti.SuperUserUsername = x_UserName_SuperUser

        objParametri_Server.UtenteUsername = x_UserName_Utente
        objParametri_Utenti.UtenteUsername = x_UserName_Utente

        objParametri_Server.LogDescrizioneUtente = x_UserName_Utente
        objParametri_Utenti.LogDescrizioneUtente = x_UserName_Utente
        '---------------------------------------------------------------------------------
        '----- Memorizzo la USERNAME e PASSWORD CRIPTATE di UTENTE e SUPERUSER -----------
        '---------------------------------------------------------------------------------

        'Amo: In queste due variabili di sessione dovrebbero andare le credenziali dell'utente?
        HttpContext.Current.Session("ASG_Utente_Username_Crypt") = Stringa_Codifica_LANCompatibile(x_UserName_SuperUser, AgroKey_EncoderDecoder)
        HttpContext.Current.Session("ASG_Utente_Password_Crypt") = Stringa_Codifica_LANCompatibile(x_Password_SuperUser, AgroKey_EncoderDecoder)

        HttpContext.Current.Session("ASG_SuperUser_Username_Crypt") = Stringa_Codifica_LANCompatibile(x_UserName_SuperUser, AgroKey_EncoderDecoder)
        HttpContext.Current.Session("ASG_SuperUser_Password_Crypt") = Stringa_Codifica_LANCompatibile(x_Password_SuperUser, AgroKey_EncoderDecoder)

        '-----------------------------------------------------------------------
        '--------- Memorizzo il CODICE FISCALE di UTENTE e SUPERUSER -----------
        '-----------------------------------------------------------------------

        Dim x_Utente_CodFiscale As String = DT.Rows(0).Item("CodFisc")
        Dim x_SuperUser_CodFiscale As String = DT.Rows(0).Item("CodFisc_SuperUser")

        HttpContext.Current.Session("ASG_Utente_CodFiscale") = x_Utente_CodFiscale
        HttpContext.Current.Session("ASG_SuperUser_CodFiscale") = x_SuperUser_CodFiscale

        objParametri_Server.UtenteCodFiscale = x_Utente_CodFiscale
        objParametri_Utenti.UtenteCodFiscale = x_Utente_CodFiscale
        objParametri_Server.UsernameOperazione = x_Utente_CodFiscale
        objParametri_Utenti.UsernameOperazione = x_Utente_CodFiscale

        objParametri_Server.PivaSuperUser = x_SuperUser_CodFiscale
        objParametri_Utenti.PivaSuperUser = x_SuperUser_CodFiscale

        '-----------------------------------------------------------------------
        '------------ Memorizzo il Codice Progressivo GIAS ---------------------
        '-----------------------------------------------------------------------

        Dim x_ProgressivoGIAS As Integer = DT.Rows(0).Item("ProgressivoGIAS")
        Dim x_CD_Key As String = DT.Rows(0).Item("CD_Key")

        If x_ProgressivoGIAS = -1 OrElse x_CD_Key = "-1" Then
            '### Manca il record nella tabella Utenti_CodiciGiasPro ###
            ' OR
            '### Chiave Assente ###

            x_ProgressivoGIAS = 0
            HttpContext.Current.Session("ASG_ProgressivoGIAS") = x_ProgressivoGIAS

            'Imposta_MsgErrore(objSession, Flag_Tunnel, Messaggio)
            'objResponse.Redirect(URL_Redirect_Errore)
            'AgroMsgBox(Messaggio, objPage)
            'Exit Function
            Return " Messaggio per l'amministratore !!!  Non e' stata rilevata la chiave di attivazione del prodotto. "

        Else
            '### Chiave Presente ###
            HttpContext.Current.Session("ASG_ProgressivoGIAS") = x_ProgressivoGIAS
            Return ""
        End If
    End Function

    Public Function Verifica_Validita_Permessi_E_Chiave_Licenza(ByRef objParametri_Utenti As AgronicaCoreParametri) As Messaggio_Utente_Permessi


        Dim risultato As New Messaggio_Utente_Permessi
        Dim UtenteUsername As String = String.Empty

        If IsNothing(objParametri_Utenti) OrElse String.IsNullOrEmpty(objParametri_Utenti.UtenteUsername) Then
            Return Nothing
        End If
        UtenteUsername = objParametri_Utenti.UtenteUsername

        Dim Scadenza_Permesso As Date
        Dim Num_Giorni As Integer
        Dim DT As DataTable

        'Me.Lbl_ScadenzaPermessi.Visible = False

        Dim xLetturaPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        DT = xLetturaPermessi.LeggiCached(UtenteUsername,
                                    enum_Id_Servizio.GiasOnline,
                                    enum_Security_Attivita.Gest_Menu,
                                    enum_Security_Operazione.Lettura,
                                    0,
                                    "",
                                    "",
                                    objParametri_Utenti)




        If Not IsNothing(DT) Then

            If DT.Rows.Count <> 0 Then

                '-----------------------------------------------------------
                '----------- Controllo Scadenza Validità Permessi ----------
                '-----------------------------------------------------------

                Scadenza_Permesso = CDate(DT.Rows(0).Item("Validita_Fine"))

                Num_Giorni = DateDiff(DateInterval.Day, Date.Today, Scadenza_Permesso)


                Select Case Num_Giorni

                    Case Is < 0


                        ''Aggiornamento Chiave
                        'Dim auS As New AgronicaCoreUtentiDAL.AutenticaUtente
                        'auS.ASG_AggiornaChiave_PPT(objParametri_Utenti)


                        '-------  permessi scaduti  --------
                        '--- in teoria qui non ci arriva ---

                        risultato.Permessi_Scaduti_O_In_Scadenza = True
                        risultato.Messaggio = "ATTENZIONE! I permessi del suo utente sono scaduti!  Contattare l'amministratore per prolungare la validità."

                        'Me.Lbl_ScadenzaPermessi.Visible = True
                        'Me.Lbl_ScadenzaPermessi.Text = "ATTENZIONE! I permessi del suo utente sono scaduti!  Contattare l'amministratore per prolungare la validità."

                    Case Is = 0

                        risultato.Permessi_Scaduti_O_In_Scadenza = True
                        risultato.Messaggio = "ATTENZIONE! Oggi è l'ultimo giorno di validità del suo utente. Contattare l'amministratore."

                        'Me.Lbl_ScadenzaPermessi.Visible = True
                        'Me.Lbl_ScadenzaPermessi.Text = "ATTENZIONE! Oggi è l'ultimo giorno di validità del suo utente. Contattare l'amministratore."

                    Case Is = 1

                        risultato.Permessi_Scaduti_O_In_Scadenza = True
                        risultato.Messaggio = "ATTENZIONE! Domani è l'ultimo giorno di validità del suo utente. Contattare l'amministratore."

                        'Me.Lbl_ScadenzaPermessi.Visible = True
                        'Me.Lbl_ScadenzaPermessi.Text = "ATTENZIONE! Domani è l'ultimo giorno di validità del suo utente. Contattare l'amministratore."

                    Case Is < 30

                        risultato.Permessi_Scaduti_O_In_Scadenza = True
                        risultato.Messaggio = "ATTENZIONE! Stanno per scadere i permessi del suo utente. Ha " & CStr(Num_Giorni) & " giorni per prolungare la validità."

                        'Me.Lbl_ScadenzaPermessi.Visible = True
                        'Me.Lbl_ScadenzaPermessi.Text = "ATTENZIONE! Stanno per scadere i permessi del suo utente. Ha " & CStr(Num_Giorni) & " giorni per prolungare la validità."

                End Select

                '###############################################################################################################
                '##################################### VERIFICA CHIAVE LICENZA #################################################
                '###############################################################################################################


                Dim ObjUtenti_Codici As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim dtCodice As DataTable = ObjUtenti_Codici.Leggi(enum_Impostazioni_Utenti.SUPERUSER_Licenza_Giorni_Franchigia, 2, 1, "", "", objParametri_Utenti)
                Dim auK As New AgronicaCoreUtentiDAL.AutenticaUtente
                'Dim Data_Scadenza As Date
                'Dim bFranchigia As Boolean = False
                Dim sms As String = ""
                Dim GiorniFranchigia_Rimanenti As Integer = 0
                Dim bBlocco_Precedente As Boolean

                If dtCodice.Rows.Count > 0 Then

                    'In caso di 
                    If IsNumeric(dtCodice.Rows(0)("Impostazione_Valore_2")) Then
                        bBlocco_Precedente = CBool(dtCodice.Rows(0)("Impostazione_Valore_2"))
                    Else
                        bBlocco_Precedente = True
                    End If

                    'Azioni
                    Select Case auK.ASG_VerificaChiave("", False, bBlocco_Precedente, GiorniFranchigia_Rimanenti, sms, objParametri_Utenti)

                        Case True

                            'Controllo Giorni Rimanenti e messaggio
                            Select Case GiorniFranchigia_Rimanenti

                                Case -1

                                    'Chiave Ok

                                Case Else

                                    risultato.Licenza_Scaduta_O_In_Scadenza = True
                                    risultato.Messaggio = "ATTENZIONE! La Chiave di Licenza è scaduta. Sono rimasti " & GiorniFranchigia_Rimanenti & " giorni di franchigia."

                                    'Me.Lbl_ScadenzaPermessi.Visible = True
                                    'Me.Lbl_ScadenzaPermessi.Text = "ATTENZIONE! La Chiave di Licenza è scaduta. Sono rimasti " & GiorniFranchigia_Rimanenti & " giorni di franchigia."

                            End Select

                        Case False

                            risultato.Licenza_Scaduta_O_In_Scadenza = True
                            risultato.Messaggio = sms

                            'Me.Lbl_ScadenzaPermessi.Visible = True
                            'Me.Lbl_ScadenzaPermessi.Text = sms

                    End Select

                End If

            End If

        End If

        'risultato.Licenza_Scaduta_O_In_Scadenza = True
        'risultato.Messaggio = "scaduta test"

        Return risultato

    End Function



End Class

Public Class AutenticaUtente_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function ImpostaLockUtente(
                            ByVal UserName As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine(" update Utenti ")
            Stb.AppendLine(" Set Flag = -2 ")
            Stb.AppendLine(" where UserName = '" & Agro_SQL_SaveText(UserName) & "'")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function








End Class
