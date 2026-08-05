Public Class Sicurezza






    '    '#####################################################################
    'Public Shared Function Controlla_Permessi_Utente_2( _
    '                                ByVal UserName As String, _
    '                                ByVal Servizio As Integer, _
    '                                ByVal Attivita As Integer, _
    '                                ByVal Operazione As Integer, _
    '                                ByRef MsgEsito As String) _
    '                                As Boolean

    '    'NOTA
    '    '   Il parametro di uscita MsgEsito, assume i seguenti valori
    '    '   a seconda dell'esito della verifica e delle motivazioni
    '    '
    '    '       "0.Permesso concesso"
    '    '       "1.Permesso negato"
    '    '       "2.Permesso scaduto il 25/05/2002"
    '    '       "3.Permesso attivo a partire dal 25/05/2003"
    '    '       "4.Permesso attivo dalle 8.30 alle 12.30"
    '    '
    '    '   Prima della visualizzazione troncare i primi due caratteri
    '    '   i quali mi possono invece essere utili per una verifica da codice


    '    '----- Dimensiono le variabili

    '    Dim xAttivita As Integer
    '    Dim xOperazione As Integer

    '    Dim UtenteAbilitato As Boolean
    '    Dim Testo As String
    '    Dim FunzionalitaAttivata As Boolean

    '    Dim objEnum As AgronicaCoreDataProvider.TipiEnumerativi

    '    objEnum = New AgronicaCoreDataProvider.TipiEnumerativi

    '    '----- Recupero i valori

    '    xAttivita = Attivita
    '    xOperazione = Operazione


    '    '----- Verifico le eccezioni ai controlli

    '    'NOTA
    '    'Un giorno dovra' essere previsto il controllo sulla chiave ONLINE_KEY
    '    'Per ora lavoro con il web.config ...


    '    ''### Gestione Analisi ###

    '    'If (Attivita = objEnum.enum_Security_Attivita.Gest_Analisi_AccessoMenu) Or _
    '    '   (Attivita = objEnum.enum_Security_Attivita.Gest_Analisi_Cartografia) Then

    '    '    If Not IsNothing(ConfigurationSettings.AppSettings("Flag_GestioneAnalisiAttivo")) Then
    '    '        Testo = ConfigurationSettings.AppSettings("Flag_GestioneAnalisiAttivo").ToString
    '    '        FunzionalitaAttivata = CBool(Testo)
    '    '    Else
    '    '        FunzionalitaAttivata = False
    '    '    End If

    '    '    'Se la funzione non e' attivata ...
    '    '    If FunzionalitaAttivata = False Then

    '    '        Return False
    '    '        Exit Function

    '    '    End If

    '    'End If




    '    '----- Verifico se l'utente dispone del permesso richiesto

    '    Dim objUtenti As AgronicaCoreAnagrafeDAL.Utenti_Permessi_Read

    '    'Creo gli oggetti COM+
    '    Dim objPermessi As Object       'Agro_Utenti_AD.Utenti_Permessi_Read 'Object  
    '    objPermessi = objServer.CreateObject("Agro_Utenti_AD.Utenti_Permessi_Read")

    '    'Verifico i permessi
    '    UtenteAbilitato = objPermessi.Verifica( _
    '                                        CStr(UserName), _
    '                                        CInt(Servizio), _
    '                                        CInt(xAttivita), _
    '                                        CInt(xOperazione), _
    '                                        CDate(Now.Date), _
    '                                        CType(Now.Hour, Short), _
    '                                        CType(Now.Minute, Short), _
    '                                        CStr(MsgEsito), _
    '                                        , _
    '                                        CStr(objSession("ASG_Connessione_Utenti").ToString))

    '    'Distruggo gli oggetti COM+
    '    objPermessi = Nothing

    '    'Restituisco il risultato
    '    Return UtenteAbilitato

    'End Function





    ''#####################################################################
    'Public Function Stringa_Codifica(ByVal Testo As String, _
    '                                 ByVal Chiave As String, _
    '                                 ByRef objServer As Object) _
    '                                 As String

    '    If Testo = "" Then

    '        Return ""
    '        Exit Function

    '    End If

    '    Testo = Replace(Testo, "\", "@")

    '    Dim Cript As Boolean = True

    '    Dim strEncrypted As String
    '    Dim i As Integer
    '    Dim A1 As Integer
    '    Dim A2 As Integer
    '    Dim KeyPos As Byte

    '    KeyPos = 1

    '    For i = 1 To Len(Testo)
    '        A1 = Asc(Mid(Testo, i, 1))
    '        A2 = Asc(Mid(Chiave, KeyPos, 1))
    '        If Cript Then
    '            strEncrypted = strEncrypted & Chr(A2 + A1)
    '        Else
    '            strEncrypted = strEncrypted & Chr(A1 - A2)
    '        End If
    '        KeyPos = KeyPos + 1
    '        If KeyPos > Len(Chiave) Then KeyPos = 1
    '    Next


    '    If Not IsNothing(ConfigurationSettings.AppSettings("VersioneCodifica")) Then

    '        If ConfigurationSettings.AppSettings("VersioneCodifica").ToString <> "" Then

    '            Select Case ConfigurationSettings.AppSettings("VersioneCodifica").ToString.ToUpper

    '                Case "NEW"

    '                    Return Agronica_Url_Encode(strEncrypted)

    '                Case "OLD"

    '                    Return objServer.UrlEncode(strEncrypted)

    '            End Select

    '        Else

    '            Return Agronica_Url_Encode(strEncrypted)

    '        End If

    '    Else

    '        Return Agronica_Url_Encode(strEncrypted)

    '    End If

    '    'Return objServer.UrlEncode(strEncrypted)
    '    'Return Agronica_Url_Encode(strEncrypted)

    'End Function



    ''#####################################################################
    'Public Function Stringa_Decodifica(ByVal Testo As String, _
    '                                   ByVal Chiave As String, _
    '                                   ByRef objServer As Object) _
    '                                   As String

    '    If Testo = "" Then

    '        Return ""
    '        Exit Function

    '    End If

    '    'Testo = objServer.UrlDecode(Testo)
    '    'Testo = Agronica_Url_Decode(Testo)


    '    If Not IsNothing(ConfigurationSettings.AppSettings("VersioneCodifica")) Then

    '        If ConfigurationSettings.AppSettings("VersioneCodifica").ToString <> "" Then

    '            Select Case ConfigurationSettings.AppSettings("VersioneCodifica").ToString.ToUpper

    '                Case "NEW"

    '                    Testo = Agronica_Url_Decode(Testo)

    '                Case "OLD"

    '                    Testo = objServer.UrlDecode(Testo)

    '            End Select

    '        Else

    '            Testo = Agronica_Url_Decode(Testo)

    '        End If

    '    Else

    '        Testo = Agronica_Url_Decode(Testo)

    '    End If

    '    Dim Cript As Boolean = False

    '    Dim strEncrypted As String
    '    Dim i As Integer
    '    Dim A1 As Integer
    '    Dim A2 As Integer
    '    Dim KeyPos As Byte

    '    KeyPos = 1

    '    For i = 1 To Len(Testo)
    '        A1 = Asc(Mid(Testo, i, 1))
    '        A2 = Asc(Mid(Chiave, KeyPos, 1))
    '        If Cript Then
    '            strEncrypted = strEncrypted & Chr(A2 + A1)
    '        Else
    '            strEncrypted = strEncrypted & Chr(A1 - A2)
    '        End If
    '        KeyPos = KeyPos + 1
    '        If KeyPos > Len(Chiave) Then KeyPos = 1
    '    Next


    '    strEncrypted = Replace(strEncrypted, "@", "\")

    '    Return strEncrypted


    'End Function







    ''#####################################################################
    'Public Function Stringa_Codifica_Nuova(ByVal Testo As String, _
    '                                 ByVal Chiave As String) _
    '                                 As String

    '    Testo = Replace(Testo, "\", "@")

    '    Dim Cript As Boolean
    '    Dim strEncrypted As String
    '    Dim i As Long
    '    Dim A1 As Long
    '    Dim A2 As Long
    '    Dim KeyPos As Byte

    '    Cript = True

    '    KeyPos = 1

    '    '        For i = 1 To Len(Testo)
    '    '            A1 = Asc(Mid(Testo, i, 1))
    '    '            A2 = Asc(Mid(Chiave, KeyPos, 1))
    '    '            If Cript Then
    '    '                strEncrypted = strEncrypted & Chr(A2 + A1)
    '    '            Else
    '    '                strEncrypted = strEncrypted & Chr(A1 - A2)
    '    '            End If
    '    '            KeyPos = KeyPos + 1
    '    '            If KeyPos > Len(Chiave) Then KeyPos = 1
    '    '        Next
    '    '
    '    '        Stringa_Codifica = URLEncode(strEncrypted)

    '    For i = 1 To Len(Testo)

    '        strEncrypted = strEncrypted & Right("00" & Hex(Asc(Mid(Testo, i, 1))), 2)

    '    Next

    '    Stringa_Codifica_Nuova = strEncrypted


    'End Function





    ''#####################################################################
    'Public Function Stringa_Decodifica_Nuova(ByVal Testo As String, _
    '                                   ByVal Chiave As String, _
    '                                   ByRef objServer As Object) _
    '                                   As String


    '    'Testo = Simple_UrlDecode(Testo)

    '    'Dim Cript As Boolean = False

    '    Dim strEncrypted As String
    '    Dim i As Integer
    '    Dim A1 As Integer
    '    'Dim A2 As Integer
    '    'Dim KeyPos As Byte

    '    'KeyPos = 1

    '    'For i = 1 To Len(Testo)
    '    '    A1 = Asc(Mid(Testo, i, 1))
    '    '    A2 = Asc(Mid(Chiave, KeyPos, 1))
    '    '    If Cript Then
    '    '        strEncrypted = strEncrypted & Chr(A2 + A1)
    '    '    Else
    '    '        strEncrypted = strEncrypted & Chr(A1 - A2)
    '    '    End If
    '    '    KeyPos = KeyPos + 1
    '    '    If KeyPos > Len(Chiave) Then KeyPos = 1
    '    'Next


    '    'strEncrypted = Replace(strEncrypted, "@", "\")

    '    Testo = Replace(Testo, "@", "\")


    '    Dim strProva As String

    '    For i = 1 To Len(Testo) Step 2

    '        strProva = Mid(Testo, i, 2)

    '        A1 = Val("&H" & strProva)

    '        strProva = Chr(A1)
    '        strEncrypted += strProva


    '    Next

    '    strEncrypted = Replace(strEncrypted, "@", "\")

    '    Return strEncrypted


    'End Function




    ''###############################################################################################
    ''###############################################################################################
    ''###############################################################################################
    ''###############################################################################################
    ''###############################################################################################



    ''###############################################################################################
    'Public Enum enum_DataLock_Operazione
    '    DataLock_CODIFICA = 1
    '    Datalock_DECODIFICA = 2
    'End Enum

    ''###############################################################################################
    'Public Enum enum_DataLock_TipoDato
    '    DataLock_NUMERO = 1
    '    Datalock_STRINGA = 2
    'End Enum




    ''###############################################################################################
    'Public Sub DataLocker( _
    '                ByVal Flag_Operazione As enum_DataLock_Operazione, _
    '                ByVal Flag_TipoDato As enum_DataLock_TipoDato, _
    '                ByRef Dato_Normale As String, _
    '                ByRef Dato_Criptato As String, _
    '                ByVal LunghezzaStringaCriptata As Integer, _
    '                ByRef MessaggioErrore As String, _
    '                Optional ByVal VersioneAlgoritmo As Integer = 1)

    '    '----- Variabili

    '    Dim Chiavi() As String = {"00000000", _
    '                              "04785186", "09571865", "06989254", _
    '                              "04248697", "06735462", "02947283", _
    '                              "08734635", "02284775", "04267427"}

    '    Dim Chiave As String
    '    Dim i As Integer

    '    Dim intIN As Integer
    '    Dim intOUT As Integer
    '    Dim intKEY As Integer

    '    Dim strIN As String
    '    Dim strOUT As String
    '    Dim sI As String
    '    Dim sO As String
    '    Dim sK As String

    '    Dim Somma As Integer
    '    Dim Cifra As Integer


    '    '----- Verifica Dati in ingresso

    '    '
    '    '
    '    '
    '    '
    '    '
    '    '
    '    '

    '    '----- Normalizzazione

    '    If (VersioneAlgoritmo <= 0) Or (VersioneAlgoritmo > 9) Then
    '        VersioneAlgoritmo = 1
    '    End If


    '    '----- Elaborazione

    '    Select Case Flag_TipoDato


    '        Case enum_DataLock_TipoDato.DataLock_NUMERO
    '            '///////////////////////////////////////////////////////////////
    '            '///////////////////////////////////////////////////////////////
    '            '///// NUMERO //////////////////////////////////////////////////
    '            '///////////////////////////////////////////////////////////////
    '            '///////////////////////////////////////////////////////////////

    '            Select Case Flag_Operazione

    '                Case enum_DataLock_Operazione.DataLock_CODIFICA
    '                    '///////////////////////////////////////////////////////////////
    '                    '///// CODIFICA NUMERO /////////////////////////////////////////
    '                    '///////////////////////////////////////////////////////////////

    '                    'Recupero il numero
    '                    strIN = Dato_Normale

    '                    'Lo converto in intero
    '                    intIN = CInt(strIN)

    '                    'Sommo una costante per normalizzare la lunghezza
    '                    intIN = intIN + (10000000 * VersioneAlgoritmo)

    '                    'Converto in stringa
    '                    strIN = CStr(intIN)

    '                    'Imposto la chiave per la versione attuale dell'algoritmo
    '                    Chiave = Chiavi(VersioneAlgoritmo)

    '                    'Inizializzzo
    '                    strOUT = ""

    '                    'Ciclo sui caratteri ...
    '                    For i = 1 To Len(strIN)

    '                        sI = Mid(strIN, i, 1)
    '                        sK = Mid(Chiave, i, 1)

    '                        intIN = CInt(sI)
    '                        intKEY = CInt(sK)

    '                        Somma = intIN + intKEY

    '                        If Somma > 9 Then
    '                            Somma = Somma - 10
    '                        End If

    '                        strOUT = strOUT & CStr(Somma).Trim

    '                    Next

    '                    'Fine lavoro
    '                    Dato_Criptato = strOUT

    '                    '///////////////////////////////////////////////////////////////
    '                    '///// CODIFICA NUMERO fine ////////////////////////////////////
    '                    '///////////////////////////////////////////////////////////////





    '                Case enum_DataLock_Operazione.Datalock_DECODIFICA
    '                    '///////////////////////////////////////////////////////////////
    '                    '///// DECODIFICA NUMERO ///////////////////////////////////////
    '                    '///////////////////////////////////////////////////////////////

    '                    'Imposto la chiave per la versione attuale dell'algoritmo
    '                    VersioneAlgoritmo = CInt(Mid(Dato_Criptato, 1, 1))
    '                    Chiave = Chiavi(VersioneAlgoritmo)

    '                    'Recupero il numero
    '                    strIN = Dato_Criptato

    '                    'Inizializzzo
    '                    strOUT = ""

    '                    'Ciclo sui caratteri ...
    '                    For i = 1 To Len(strIN)

    '                        sI = Mid(strIN, i, 1)
    '                        sK = Mid(Chiave, i, 1)

    '                        intIN = CInt(sI)
    '                        intKEY = CInt(sK)

    '                        Cifra = intIN - intKEY

    '                        If (Cifra < 0) Then
    '                            Cifra = Cifra + 10
    '                        End If

    '                        strOUT = strOUT & CStr(Cifra).Trim

    '                    Next

    '                    'Tolgo la costante di normalizzazione
    '                    intOUT = CInt(strOUT)
    '                    intOUT = intOUT - (10000000 * VersioneAlgoritmo)
    '                    strOUT = CStr(intOUT)

    '                    'Fine lavoro
    '                    Dato_Normale = strOUT

    '                    '///////////////////////////////////////////////////////////////
    '                    '///// DECODIFICA NUMERO fine //////////////////////////////////
    '                    '///////////////////////////////////////////////////////////////


    '            End Select
    '            '///////////////////////////////////////////////////////////////
    '            '///////////////////////////////////////////////////////////////
    '            '///// NUMERO fine /////////////////////////////////////////////
    '            '///////////////////////////////////////////////////////////////
    '            '///////////////////////////////////////////////////////////////





    '        Case enum_DataLock_TipoDato.Datalock_STRINGA
    '            '///////////////////////////////////////////////////////////////
    '            '///////////////////////////////////////////////////////////////
    '            '///// STRINGA /////////////////////////////////////////////////
    '            '///////////////////////////////////////////////////////////////
    '            '///////////////////////////////////////////////////////////////
    '            Select Case Flag_Operazione

    '                Case enum_DataLock_Operazione.DataLock_CODIFICA
    '                    '///////////////////////////////////////////////////////////////
    '                    '///// CODIFICA STRINGA ////////////////////////////////////////
    '                    '///////////////////////////////////////////////////////////////

    '                    '
    '                    '
    '                    '
    '                    '

    '                    'Fine lavoro
    '                    Dato_Criptato = strOUT

    '                    '///////////////////////////////////////////////////////////////
    '                    '///// CODIFICA STRINGA fine ///////////////////////////////////
    '                    '///////////////////////////////////////////////////////////////





    '                Case enum_DataLock_Operazione.Datalock_DECODIFICA
    '                    '///////////////////////////////////////////////////////////////
    '                    '///// DECODIFICA STRINGA //////////////////////////////////////
    '                    '///////////////////////////////////////////////////////////////

    '                    '
    '                    '
    '                    '
    '                    '

    '                    'Fine lavoro
    '                    Dato_Normale = strOUT

    '                    '///////////////////////////////////////////////////////////////
    '                    '///// DECODIFICA STRINGA fine /////////////////////////////////
    '                    '///////////////////////////////////////////////////////////////


    '            End Select
    '            '///////////////////////////////////////////////////////////////
    '            '///////////////////////////////////////////////////////////////
    '            '///// STRINGA fine ////////////////////////////////////////////
    '            '///////////////////////////////////////////////////////////////
    '            '///////////////////////////////////////////////////////////////


    '    End Select









    '    ''Testo = Replace(Testo, "\", "@")

    '    'Dim Cript As Boolean = True

    '    'Dim strEncrypted As String
    '    'Dim i As Integer
    '    'Dim A1 As Integer
    '    'Dim A2 As Integer
    '    'Dim KeyPos As Byte

    '    'KeyPos = 1

    '    'For i = 1 To Len(Testo)
    '    '    A1 = Asc(Mid(Testo, i, 1))
    '    '    A2 = Asc(Mid(Chiave, KeyPos, 1))
    '    '    If Cript Then
    '    '        strEncrypted = strEncrypted & Chr(A2 + A1)
    '    '    Else
    '    '        strEncrypted = strEncrypted & Chr(A1 - A2)
    '    '    End If
    '    '    KeyPos = KeyPos + 1
    '    '    If KeyPos > Len(Chiave) Then KeyPos = 1
    '    'Next






    'End Sub










    ''###############################################################################################
    ''###############################################################################################
    ''###############################################################################################
    ''###############################################################################################
    ''###############################################################################################


    ''#####################################################################
    'Public Function Stringa_Codifica_LANCompatibile( _
    '                                 ByVal Testo As String, _
    '                                 ByVal Chiave As String) _
    '                                 As String


    '    'Testo = Replace(Testo, "\", "@")

    '    Dim Cript As Boolean = True

    '    Dim strEncrypted As String
    '    Dim i As Integer
    '    Dim A1 As Integer
    '    Dim A2 As Integer
    '    Dim KeyPos As Byte

    '    KeyPos = 1

    '    For i = 1 To Len(Testo)
    '        A1 = Asc(Mid(Testo, i, 1))
    '        A2 = Asc(Mid(Chiave, KeyPos, 1))
    '        If Cript Then
    '            strEncrypted = strEncrypted & Chr(A2 + A1)
    '        Else
    '            strEncrypted = strEncrypted & Chr(A1 - A2)
    '        End If
    '        KeyPos = KeyPos + 1
    '        If KeyPos > Len(Chiave) Then KeyPos = 1
    '    Next

    '    Return Agronica_Url_Encode(strEncrypted)

    'End Function




    ''#####################################################################
    'Public Function Stringa_Decodifica_LANCompatibile( _
    '                                   ByVal Testo As String, _
    '                                   ByVal Chiave As String) _
    '                                   As String

    '    Testo = Agronica_Url_Decode(Testo)

    '    Dim Cript As Boolean = False

    '    Dim strEncrypted As String
    '    Dim i As Integer
    '    Dim A1 As Integer
    '    Dim A2 As Integer
    '    Dim KeyPos As Byte

    '    KeyPos = 1

    '    For i = 1 To Len(Testo)
    '        A1 = Asc(Mid(Testo, i, 1))
    '        A2 = Asc(Mid(Chiave, KeyPos, 1))
    '        If Cript Then
    '            strEncrypted = strEncrypted & Chr(A2 + A1)
    '        Else
    '            strEncrypted = strEncrypted & Chr(A1 - A2)
    '        End If
    '        KeyPos = KeyPos + 1
    '        If KeyPos > Len(Chiave) Then KeyPos = 1
    '    Next


    '    'strEncrypted = Replace(strEncrypted, "@", "\")

    '    Return strEncrypted


    'End Function




    ''##############################################################
    'Function Agronica_Url_Encode( _
    '                        ByVal StrInput As String, _
    '                        Optional ByVal Separatore As String = "G") _
    '                        As String

    '    Dim i As Integer
    '    Dim Carattere As String
    '    Dim Cod_Ascii_16 As Integer
    '    Dim Cod_Hex As String
    '    Dim StrOutput As String


    '    StrOutput = ""

    '    For i = 1 To Len(StrInput)

    '        Carattere = Mid(StrInput, i, 1)

    '        Cod_Ascii_16 = AscW(Carattere)

    '        Cod_Hex = Hex(Cod_Ascii_16)

    '        StrOutput = StrOutput & Cod_Hex & Separatore

    '    Next i

    '    'Tolgo il separatore finale
    '    StrOutput = Left(StrOutput, Len(StrOutput) - 1)

    '    'Return
    '    Return StrOutput

    'End Function




    ''##############################################################
    'Function Agronica_Url_Decode( _
    '                        ByVal StrInput As String, _
    '                        Optional ByVal Separatore As String = "G") _
    '                        As String

    '    Dim i As Integer
    '    Dim Carattere As String
    '    Dim Cod_Ascii_16 As Integer
    '    Dim Cod_Hex As String
    '    Dim StrOutput As String
    '    Dim Vettore() As String

    '    'Inizializzo
    '    StrOutput = ""

    '    'Recupero gli elementi
    '    Vettore = Split(StrInput, Separatore)

    '    For i = LBound(Vettore) To UBound(Vettore)

    '        Cod_Hex = Vettore(i)

    '        Cod_Ascii_16 = Val("&H" & Cod_Hex)

    '        Carattere = ChrW(Cod_Ascii_16)

    '        StrOutput = StrOutput & Carattere

    '    Next i

    '    Return StrOutput

    'End Function




    ''###############################################################################################
    'Public Function Verifica_Mirroring()

    '    Dim Flag_Mirror As Integer

    '    '-------------------------------------------------
    '    '---- verifica l'attivazione del MIRRORING -------
    '    '-------------------------------------------------
    '    If Not IsNothing(ConfigurationSettings.AppSettings("Flag_Mirror")) Then


    '        If ConfigurationSettings.AppSettings("Flag_Mirror") = "SYSTEM_FRAMEWORK" Then

    '            Flag_Mirror = 0

    '        Else

    '            Flag_Mirror = 1

    '        End If

    '    End If

    '    Return Flag_Mirror

    'End Function




    ''###############################################################################################
    ''###############################################################################################
    ''###############################################################################################
    ''###############################################################################################





End Class
