Imports System.Configuration
Imports System.Data.Entity.Migrations.Model
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Sicurezza

    Private Key As Byte() = {12, 52, 74, 32, 33, 36, 23, 48, 14, 50,
                             52, 112, 60, 14, 135, 116, 84, 149,
                             81, 200, 211, 29, 65, 35}

    Private Iv As Byte() = {12, 36, 37, 97, 106, 56, 76, 18, 99, 107,
                            21, 123, 65, 114, 159, 196, 179,
                            198, 192, 241, 212, 123, 0, 54}

    Private Const PBKDF2iterCount = 1000
    Private Const PBKDF2SubkeyLength = 256 / 8 '256 bits
    Private Const SaltSize = 128 / 8 '128 bits

    ' indentifica la stringa di connessione al super server
    Public Const ID_DB_Super_Server = "-1"
    Public Const ENCRYPTION_KEY = "13422245-16FA-4712-8BEF-26A32DC979BB"

#Region "SHA-256"

    Public Shared Function CalculateSHA256_ToString(ByVal str As String) As String
        Dim hashed As Byte() = CalculateSHA256(str)
        Return BitConverter.ToString(hashed).Replace("-", "")
    End Function

    Public Shared Function CalculateSHA256(ByVal str As String) As Byte()

        Dim sha256 As SHA256 = SHA256Managed.Create()
        Dim objUtf8 As New Text.UTF8Encoding()
        Dim hashValue As Byte() = sha256.ComputeHash(objUtf8.GetBytes(If(str, "")))

        Return hashValue
    End Function

#End Region


    Public Function EncryptString(plainText As String, cr As String) As String

        If String.IsNullOrEmpty(plainText) Then
            Throw New ArgumentNullException("plainText")
        End If

        Dim key As Byte() = Nothing
        Dim iv As Byte() = Nothing

        GetAesEncryptionKeyAndIV(cr, key, iv)

        Dim encrypted As String = EncryptStringWithAes(plainText, key, iv)
        Return encrypted
    End Function

    Public Function DecryptString(encryptedText As String, cr As String) As String

        If String.IsNullOrEmpty(encryptedText) Then
            Throw New ArgumentNullException("encryptedText")
        End If

        Dim key As Byte() = Nothing
        Dim iv As Byte() = Nothing

        GetAesEncryptionKeyAndIV(cr, key, iv)

        Dim decrypted As String = DecryptStringWithAes(encryptedText, key, iv)
        Return decrypted

    End Function

    Private Sub GetAesEncryptionKeyAndIV(cr As String, ByRef key As Byte(), ByRef iv As Byte())

        If String.IsNullOrEmpty(cr) Then
            Throw New ArgumentNullException("cr")
        End If

        Dim cr2 As String = ""
        If Not IsNothing(ConfigurationManager.AppSettings("cr2")) Then
            cr2 = ConfigurationManager.AppSettings("cr2")
        ElseIf Not IsNothing(ConfigurationManager.GetSection("globalSettings")) Then
            cr2 = ConfigurationManager.GetSection("globalSettings")("cr2")
        End If

        If String.IsNullOrEmpty(cr2) Then
            Throw New Exception("Key missing, can't decrypt")
        End If

        Dim keyStr = cr & cr2
        key = CalculateSHA256(keyStr)
        Dim iv_32 = CalculateSHA256(cr)
        iv = iv_32.Take(16).ToArray()

    End Sub

    Private Function EncryptStringWithAes(plainText As String, key As Byte(), iv As Byte()) As String

        If String.IsNullOrEmpty(plainText) Then
            Throw New ArgumentNullException("plainText")
        End If

        If IsNothing(key) OrElse key.Length <= 0 Then
            Throw New ArgumentNullException("key")
        End If

        If IsNothing(iv) OrElse iv.Length <= 0 Then
            Throw New ArgumentNullException("iv")
        End If

        Dim encrypted As Byte()

        Using aes As Aes = Aes.Create()

            aes.Key = key
            aes.IV = iv

            ' Create an encryptor
            Dim encryptor As ICryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV)

            ' Create a memory stream to hold the encrypted data
            Using msEncrypt As New MemoryStream()

                Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)

                    Using swEncrypt As New StreamWriter(csEncrypt)
                        ' Write the data to be encrypted into the stream
                        swEncrypt.Write(plainText)
                    End Using

                End Using

                encrypted = msEncrypt.ToArray()

            End Using

        End Using

        Return Convert.ToBase64String(encrypted)

    End Function

    Private Function DecryptStringWithAes(cipherText As String, key As Byte(), iv As Byte()) As String

        If String.IsNullOrEmpty(cipherText) Then
            Throw New ArgumentNullException("cipherText")
        End If

        If IsNothing(key) OrElse key.Length <= 0 Then
            Throw New ArgumentNullException("key")
        End If

        If IsNothing(iv) OrElse iv.Length <= 0 Then
            Throw New ArgumentNullException("iv")
        End If

        Dim cipherByteArray = Convert.FromBase64String(cipherText)
        Dim plaintext As String = Nothing

        Using aes As Aes = Aes.Create()
            aes.Key = key
            aes.IV = iv

            ' Create a decryptor
            Dim decryptor As ICryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV)

            ' Create a memory stream with the encrypted data
            Using msDecrypt As New MemoryStream(cipherByteArray)

                Using csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)

                    Using srDecrypt As New StreamReader(csDecrypt)
                        ' Read the decrypted data
                        plaintext = srDecrypt.ReadToEnd()
                    End Using

                End Using

            End Using

        End Using

        Return plaintext
    End Function

    '#####################################################################
    Public Function Stringa_Codifica_WebService(ByVal Testo As String) As String

        Dim strEncrypted As String = ""

        'va deciso l'algoritmo di crypt
        strEncrypted = Testo

        Return strEncrypted

    End Function

    '#####################################################################
    Public Function Stringa_Decodifica_WebService(ByVal strEncrypted As String) As String

        Dim Testo As String = ""

        'va deciso l'algoritmo di crypt
        Testo = strEncrypted

        Return Testo

    End Function


    '#####################################################################
    Public Shared Function Stringa_Codifica_Tunnel(ByVal Testo As String) As String

        If Testo = "" Then
            Return ""
        End If

        Testo = Replace(Testo, "\", "@")

        If UsaEncodingSemplice() Then
            Return System.Web.HttpUtility.HtmlEncode(Testo)
        End If

        Dim Cript As Boolean = True
        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte
        KeyPos = 1

        For i = 1 To Len(Testo)
            A1 = Asc(Mid(Testo, i, 1))
            A2 = Asc(Mid(AgroKeyTunnel_EncoderDecoder, KeyPos, 1))
            If Cript Then
                strEncrypted = strEncrypted & Chr(A2 + A1)
            Else
                strEncrypted = strEncrypted & Chr(A1 - A2)
            End If
            KeyPos = KeyPos + 1
            If KeyPos > Len(AgroKeyTunnel_EncoderDecoder) Then KeyPos = 1
        Next


        Return Agronica_Url_Encode(strEncrypted)


    End Function






    '#####################################################################
    Public Shared Function Stringa_Decodifica_Tunnel(ByVal Testo As String) As String

        If Testo = "" Then
            Return ""
        End If

        If UsaEncodingSemplice() Then
            Testo = System.Web.HttpUtility.HtmlDecode(Testo)
            Testo = Replace(Testo, "@", "\")
            Return Testo
        End If

        Testo = Agronica_Url_Decode(Testo)

        Dim Cript As Boolean = False

        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte

        KeyPos = 1

        For i = 1 To Len(Testo)
            A1 = Asc(Mid(Testo, i, 1))
            A2 = Asc(Mid(AgroKeyTunnel_EncoderDecoder, KeyPos, 1))
            If Cript Then
                strEncrypted = strEncrypted & Chr(A2 + A1)
            Else
                strEncrypted = strEncrypted & Chr(A1 - A2)
            End If
            KeyPos = KeyPos + 1
            If KeyPos > Len(AgroKeyTunnel_EncoderDecoder) Then KeyPos = 1
        Next


        strEncrypted = Replace(strEncrypted, "@", "\")

        Return strEncrypted


    End Function


    '##############################################################
    Private Shared Function Agronica_Url_Encode(
                            ByVal StrInput As String,
                            Optional ByVal Separatore As String = "G") _
                            As String

        Dim i As Integer
        Dim Carattere As String
        Dim Cod_Ascii_16 As Integer
        Dim Cod_Hex As String
        Dim StrOutput As String


        StrOutput = ""

        For i = 1 To Len(StrInput)

            Carattere = Mid(StrInput, i, 1)

            Cod_Ascii_16 = AscW(Carattere)

            Cod_Hex = Hex(Cod_Ascii_16)

            StrOutput = StrOutput & Cod_Hex & Separatore

        Next i

        'Tolgo il separatore finale
        StrOutput = Left(StrOutput, Len(StrOutput) - 1)

        'Return
        Return StrOutput

    End Function

    '##############################################################
    Private Shared Function Agronica_Url_Decode(ByVal StrInput As String,
                                                Optional ByVal Separatore As String = "G"
                                                ) As String

        Dim i As Integer
        Dim Carattere As String
        Dim Cod_Ascii_16 As Integer
        Dim Cod_Hex As String
        Dim StrOutput As String
        Dim Vettore As String()

        'Inizializzo
        StrOutput = ""

        'Recupero gli elementi
        Vettore = Split(StrInput, Separatore)

        For i = LBound(Vettore) To UBound(Vettore)

            Cod_Hex = Vettore(i)

            Cod_Ascii_16 = Val("&H" & Cod_Hex)

            Carattere = ChrW(Cod_Ascii_16)

            StrOutput = StrOutput & Carattere

        Next i

        Return StrOutput

    End Function


    '#####################################################################
    Public Shared Function Stringa_Codifica(ByVal Testo As String,
                                            ByVal Chiave As String,
                                            ByRef objServer As Object
                                            ) As String

        If Testo = "" Then
            Return ""
        End If

        Testo = Replace(Testo, "\", "@")

        If UsaEncodingSemplice() Then
            Return System.Web.HttpUtility.HtmlEncode(Testo)
        End If


        Dim Cript As Boolean = True

        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte

        KeyPos = 1

        For i = 1 To Len(Testo)
            A1 = Asc(ControllaAccenti(CChar(Mid(Testo, i, 1))))
            A2 = Asc(Mid(Chiave, KeyPos, 1))
            If Cript Then
                strEncrypted = strEncrypted & Chr(A2 + A1)
            Else
                strEncrypted = strEncrypted & Chr(A1 - A2)
            End If
            KeyPos = KeyPos + 1
            If KeyPos > Len(Chiave) Then KeyPos = 1
        Next

        'Return objServer.UrlEncode(strEncrypted)
        Return Agronica_Url_Encode(strEncrypted)

    End Function


    '#####################################################################
    Public Shared Function Stringa_Codifica(ByVal Testo As String, ByVal Chiave As String) As String

        If Testo = "" Then
            Return ""
        End If

        Testo = Replace(Testo, "\", "@")

        If UsaEncodingSemplice() Then
            Return System.Web.HttpUtility.HtmlEncode(Testo)
        End If


        Dim Cript As Boolean = True

        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte

        KeyPos = 1

        For i = 1 To Len(Testo)
            A1 = Asc(ControllaAccenti(CChar(Mid(Testo, i, 1))))
            A2 = Asc(Mid(Chiave, KeyPos, 1))
            If Cript Then
                strEncrypted = strEncrypted & Chr(A2 + A1)
            Else
                strEncrypted = strEncrypted & Chr(A1 - A2)
            End If
            KeyPos = KeyPos + 1
            If KeyPos > Len(Chiave) Then KeyPos = 1
        Next


        Return Agronica_Url_Encode(strEncrypted)

    End Function






    'PATCH SULLA CODIFICA DEI CARATTERI ACCENTATI, NON HO TROVATO NIENTE DI MEGLIO, è una schifezza e ne sono conscio
    Private Shared Function ControllaAccenti(ByVal chr_2_check As Char) As Char
        Select Case chr_2_check
            Case CChar("à"), CChar("á")
                Return CChar("a")
            Case CChar("é"), CChar("è")
                Return CChar("e")
            Case CChar("ì"), CChar("í")
                Return CChar("i")
            Case CChar("ò"), CChar("ó")
                Return CChar("o")
            Case CChar("ù"), CChar("ú")
                Return CChar("u")
            Case CChar("À"), CChar("Á")
                Return CChar("A")
            Case CChar("È"), CChar("É")
                Return CChar("E")
            Case CChar("Ì"), CChar("Í")
                Return CChar("I")
            Case CChar("Ó"), CChar("Ò")
                Return CChar("O")
            Case CChar("Ú"), CChar("Ù")
                Return CChar("U")
            Case CChar("°")
                Return CChar(" ")

                '  Giulia, 03/04/2017 12:10:05: Necessari per sostituire i caratteri di altre lingue (tedesco, spagnolo, ecc..)
                '       Per questo è cambiata anche la codifica del file
            Case CChar("ä"), CChar("ã"), CChar("â"), CChar("å"), CChar("ā"), CChar("ă")
                Return CChar("a")
            Case CChar("À"), CChar("Á"), CChar("Â"), CChar("Ã"), CChar("Ä"), CChar("Å"), CChar("Ā"), CChar("Ă")
                Return CChar("A")
            Case CChar("ê"), CChar("ë"), CChar("ē"), CChar("ĕ"), CChar("ė"), CChar("ě")
                Return CChar("e")
            Case CChar("È"), CChar("É"), CChar("Ê"), CChar("Ë"), CChar("Ē"), CChar("Ĕ"), CChar("Ė"), CChar("Ě")
                Return CChar("E")
            Case CChar("î"), CChar("ï")
                Return CChar("i")
            Case CChar("Î"), CChar("Ï")
                Return CChar("I")
            Case CChar("ò"), CChar("ó"), CChar("ô"), CChar("ô"), CChar("õ"), CChar("ö"), CChar("ō"), CChar("ŏ")
                Return CChar("o")
            Case CChar("Ò"), CChar("Ó"), CChar("Ô"), CChar("Õ"), CChar("Ö"), CChar("Ō"), CChar("Ŏ")
                Return CChar("O")
            Case CChar("ù"), CChar("ú"), CChar("û"), CChar("ü"), CChar("ů"), CChar("ũ")
                Return CChar("u")
            Case CChar("Ù"), CChar("Ú"), CChar("Û"), CChar("Ü"), CChar("Ů"), CChar("Ũ")
                Return CChar("U")
            Case CChar("ñ"), CChar("ń"), CChar("ň")
                Return CChar("n")
            Case CChar("Ñ"), CChar("Ń"), CChar("Ň")
                Return CChar("N")

            Case CChar("ß")
                Return CChar("S")

            Case Else
                Return chr_2_check
        End Select
    End Function


    '#####################################################################
    Public Function Stringa_DecodificaKey(ByVal StringaCriptata As String,
                                          ByVal FormattaURL As Boolean
                                          ) As String

        Dim StringaCriptataBis As String
        Dim cryptoProvider As New TripleDESCryptoServiceProvider

        '----- Decrypt di una stringa
        If FormattaURL Then
            StringaCriptataBis = Replace(StringaCriptata, "^", "+")
        Else
            StringaCriptataBis = StringaCriptata
        End If

        'Converto la stringa con formato XML in un array di byte
        Dim buffer As Byte() = Convert.FromBase64String(StringaCriptataBis)
        Dim ms As New MemoryStream(buffer)
        Dim cs As New CryptoStream(ms,
                                   cryptoProvider.CreateDecryptor(Key, Iv),
                                   CryptoStreamMode.Read)

        Dim sr As New StreamReader(cs)
        Return sr.ReadToEnd()

    End Function



    '#####################################################################
    Public Shared Function Stringa_Decodifica(ByVal Testo As String,
                                              ByVal Chiave As String,
                                              Optional ByRef objServer As Object = Nothing
                                              ) As String

        If Testo = "" Then
            Return ""
        End If

        If UsaEncodingSemplice() Then
            Testo = System.Web.HttpUtility.HtmlDecode(Testo)
            Testo = Replace(Testo, "@", "\")
            Return Testo
        End If

        'Testo = objServer.UrlDecode(Testo)
        Testo = Agronica_Url_Decode(Testo)

        Dim Cript As Boolean = False

        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte

        KeyPos = 1

        For i = 1 To Len(Testo)
            A1 = Asc(Mid(Testo, i, 1))
            A2 = Asc(Mid(Chiave, KeyPos, 1))
            If Cript Then
                strEncrypted = strEncrypted & Chr(A2 + A1)
            Else
                strEncrypted = strEncrypted & Chr(A1 - A2)
            End If
            KeyPos = KeyPos + 1
            If KeyPos > Len(Chiave) Then KeyPos = 1
        Next


        strEncrypted = Replace(strEncrypted, "@", "\")

        Return strEncrypted


    End Function

    '#####################################################################
    Public Shared Function Stringa_Codifica_LANCompatibile(ByVal Testo As String,
                                                           ByVal Chiave As String,
                                                           Optional Esci_Se_Vuoto As Boolean = False
                                                           ) As String

        If Esci_Se_Vuoto AndAlso Testo = "" Then
            Return ""
        End If
        'Testo = Replace(Testo, "\", "@")

        If UsaEncodingSemplice() Then
            Testo = Replace(Testo, "\", "@")
            Return System.Web.HttpUtility.HtmlEncode(Testo)
        End If

        Dim Cript As Boolean = True

        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte

        KeyPos = 1

        For i = 1 To Len(Testo)
            A1 = Asc(Mid(Testo, i, 1))
            A2 = Asc(Mid(Chiave, KeyPos, 1))
            If Cript Then
                strEncrypted = strEncrypted & Chr(A2 + A1)
            Else
                strEncrypted = strEncrypted & Chr(A1 - A2)
            End If
            KeyPos = KeyPos + 1
            If KeyPos > Len(Chiave) Then KeyPos = 1
        Next

        Return Agronica_Url_Encode(strEncrypted)

    End Function

    '#####################################################################
    Public Shared Function Stringa_Decodifica_LANCompatibile(ByVal Testo As String,
                                                             ByVal Chiave As String,
                                                             Optional Esci_Se_Vuoto As Boolean = False
                                                             ) As String

        If Esci_Se_Vuoto AndAlso Testo = "" Then
            Return ""
        End If

        If UsaEncodingSemplice() Then
            Testo = System.Web.HttpUtility.HtmlDecode(Testo)
            Testo = Replace(Testo, "@", "\")
            Return Testo
        End If

        Testo = Agronica_Url_Decode(Testo)

        Dim Cript As Boolean = False

        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte

        KeyPos = 1

        For i = 1 To Len(Testo)
            A1 = Asc(Mid(Testo, i, 1))
            A2 = Asc(Mid(Chiave, KeyPos, 1))
            If Cript Then
                strEncrypted = strEncrypted & Chr(A2 + A1)
            Else
                strEncrypted = strEncrypted & Chr(A1 - A2)
            End If
            KeyPos = KeyPos + 1
            If KeyPos > Len(Chiave) Then KeyPos = 1
        Next

        'strEncrypted = Replace(strEncrypted, "@", "\")

        Return strEncrypted

    End Function


    '######################################################################################################
    Private Sub G2G_Stringa_Decodifica(ByVal TestoCrypt As String,
                                       ByRef TestoChiaro As String,
                                       ByRef ErroreFlag As Boolean,
                                       ByRef ErroreMessaggio As String)

        Try

            ErroreMessaggio = ""
            ErroreFlag = False

            '   §§§ DA IMPLEMENTARE §§§

            TestoChiaro = TestoCrypt

        Catch ex As Exception
            ErroreMessaggio = ex.Message
            ErroreFlag = True
        End Try

    End Sub

    '#####################################################################
    Public Shared Function Stringa_Codifica_Nuova(ByVal Testo As String,
                                                  ByVal Chiave As String
                                                  ) As String

        Testo = Replace(Testo, "\", "@")

        If UsaEncodingSemplice() Then
            Return System.Web.HttpUtility.HtmlEncode(Testo)
        End If

        Dim Cript As Boolean
        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim KeyPos As Byte

        Cript = True

        KeyPos = 1

        '        For i = 1 To Len(Testo)
        '            A1 = Asc(Mid(Testo, i, 1))
        '            A2 = Asc(Mid(Chiave, KeyPos, 1))
        '            If Cript Then
        '                strEncrypted = strEncrypted & Chr(A2 + A1)
        '            Else
        '                strEncrypted = strEncrypted & Chr(A1 - A2)
        '            End If
        '            KeyPos = KeyPos + 1
        '            If KeyPos > Len(Chiave) Then KeyPos = 1
        '        Next
        '
        '        Stringa_Codifica = URLEncode(strEncrypted)

        For i = 1 To Len(Testo)

            strEncrypted = strEncrypted & Right("00" & Hex(Asc(Mid(Testo, i, 1))), 2)

        Next

        Return strEncrypted

    End Function





    '#####################################################################
    'usata per decodificare l'xml proveniente dal giasonline
    Public Shared Function Stringa_Decodifica_Nuova(ByVal Testo As String,
                                                    ByVal Chiave As String,
                                                    Optional ByRef objServer As Object = Nothing
                                                    ) As String

        If UsaEncodingSemplice() Then
            Testo = System.Web.HttpUtility.HtmlDecode(Testo)
            Testo = Replace(Testo, "@", "\")
            Return Testo
        End If

        'Testo = Simple_UrlDecode(Testo)

        'Dim Cript As Boolean = False

        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim A1 As Integer
        'Dim A2 As Integer
        'Dim KeyPos As Byte

        'KeyPos = 1

        'For i = 1 To Len(Testo)
        '    A1 = Asc(Mid(Testo, i, 1))
        '    A2 = Asc(Mid(Chiave, KeyPos, 1))
        '    If Cript Then
        '        strEncrypted = strEncrypted & Chr(A2 + A1)
        '    Else
        '        strEncrypted = strEncrypted & Chr(A1 - A2)
        '    End If
        '    KeyPos = KeyPos + 1
        '    If KeyPos > Len(Chiave) Then KeyPos = 1
        'Next


        'strEncrypted = Replace(strEncrypted, "@", "\")

        Testo = Replace(Testo, "@", "\")


        Dim strProva As String

        For i = 1 To Len(Testo) Step 2

            strProva = Mid(Testo, i, 2)

            A1 = Val("&H" & strProva)

            strProva = Chr(A1)
            strEncrypted += strProva

        Next

        strEncrypted = Replace(strEncrypted, "@", "\")

        Return strEncrypted

    End Function


    Private Function Simple_URLEncode(ByVal strURL As String) As String

        Dim result As String = ""

        For intN As Integer = 1 To Len(strURL)
            Select Case Asc(UCase(Mid(strURL, intN, 1)))
                Case Asc("A") To Asc("Z"), Asc("0") To Asc("9"), Asc("+"), Asc("."), Asc("-"), Asc("/"), Asc(":"), Asc("?")
                    result &= Mid(strURL, intN, 1)
                Case Else

                    result &= "%" & Right("00" & Hex(Asc(Mid(strURL, intN, 1))), 2)

            End Select
        Next

        Return result

    End Function

    Private Function Simple_URLDecode(ByVal strCode As String) As String

        Dim i As Integer
        Dim strCodice As String
        Dim risultato As String = ""

        For i = 1 To Len(strCode)

            If Mid(strCode, i, 1) = "%" Then

                strCodice = Mid(strCode, i + 1, 2)
                risultato = risultato & Chr(CInt("&H" & strCodice))
                i = i + 2

            Else

                risultato = risultato & Mid(strCode, i, 1)

            End If

        Next

        Return risultato

    End Function

    Private Shared Function UsaEncodingSemplice() As Boolean

        Dim encodingSemplice As Boolean = False
        If Not IsNothing(ConfigurationManager.AppSettings("UsaEncodingSemplice")) Then
            encodingSemplice = CBool(ConfigurationManager.AppSettings("UsaEncodingSemplice"))
        End If

        Return encodingSemplice
    End Function

    '###############################################################################################
    Public Enum enum_DataLock_Operazione
        DataLock_CODIFICA = 1
        Datalock_DECODIFICA = 2
    End Enum

    '###############################################################################################
    Public Enum enum_DataLock_TipoDato
        DataLock_NUMERO = 1
        Datalock_STRINGA = 2
    End Enum


    '###############################################################################################
    Private Sub DataLocker(ByVal Flag_Operazione As enum_DataLock_Operazione,
                           ByVal Flag_TipoDato As enum_DataLock_TipoDato,
                           ByRef Dato_Normale As String,
                           ByRef Dato_Criptato As String,
                           ByVal LunghezzaStringaCriptata As Integer,
                           ByRef MessaggioErrore As String,
                           Optional ByVal VersioneAlgoritmo As Integer = 1)

        '----- Variabili

        Dim Chiavi As String() = {"00000000",
                                  "04785186", "09571865", "06989254",
                                  "04248697", "06735462", "02947283",
                                  "08734635", "02284775", "04267427"}

        Dim Chiave As String
        Dim i As Integer

        Dim intIN As Integer
        Dim intOUT As Integer
        Dim intKEY As Integer

        Dim strIN As String
        Dim strOUT As String = ""
        Dim sI As String
        Dim sK As String

        Dim Somma As Integer
        Dim Cifra As Integer


        '----- Verifica Dati in ingresso




        '----- Normalizzazione

        If (VersioneAlgoritmo <= 0) OrElse (VersioneAlgoritmo > 9) Then
            VersioneAlgoritmo = 1
        End If


        '----- Elaborazione

        Select Case Flag_TipoDato


            Case enum_DataLock_TipoDato.DataLock_NUMERO
                '///////////////////////////////////////////////////////////////
                '///////////////////////////////////////////////////////////////
                '///// NUMERO //////////////////////////////////////////////////
                '///////////////////////////////////////////////////////////////
                '///////////////////////////////////////////////////////////////

                Select Case Flag_Operazione

                    Case enum_DataLock_Operazione.DataLock_CODIFICA
                        '///////////////////////////////////////////////////////////////
                        '///// CODIFICA NUMERO /////////////////////////////////////////
                        '///////////////////////////////////////////////////////////////

                        'Recupero il numero
                        strIN = Dato_Normale

                        'Lo converto in intero
                        intIN = CInt(strIN)

                        'Sommo una costante per normalizzare la lunghezza
                        intIN = intIN + (10000000 * VersioneAlgoritmo)

                        'Converto in stringa
                        strIN = CStr(intIN)

                        'Imposto la chiave per la versione attuale dell'algoritmo
                        Chiave = Chiavi(VersioneAlgoritmo)

                        'Inizializzo
                        strOUT = ""

                        'Ciclo sui caratteri ...
                        For i = 1 To Len(strIN)

                            sI = Mid(strIN, i, 1)
                            sK = Mid(Chiave, i, 1)

                            intIN = CInt(sI)
                            intKEY = CInt(sK)

                            Somma = intIN + intKEY

                            If Somma > 9 Then
                                Somma = Somma - 10
                            End If

                            strOUT = strOUT & CStr(Somma).Trim

                        Next

                        'Fine lavoro
                        Dato_Criptato = strOUT

                        '///////////////////////////////////////////////////////////////
                        '///// CODIFICA NUMERO fine ////////////////////////////////////
                        '///////////////////////////////////////////////////////////////





                    Case enum_DataLock_Operazione.Datalock_DECODIFICA
                        '///////////////////////////////////////////////////////////////
                        '///// DECODIFICA NUMERO ///////////////////////////////////////
                        '///////////////////////////////////////////////////////////////

                        'Imposto la chiave per la versione attuale dell'algoritmo
                        VersioneAlgoritmo = CInt(Mid(Dato_Criptato, 1, 1))
                        Chiave = Chiavi(VersioneAlgoritmo)

                        'Recupero il numero
                        strIN = Dato_Criptato

                        'Inizializzo
                        strOUT = ""

                        'Ciclo sui caratteri ...
                        For i = 1 To Len(strIN)

                            sI = Mid(strIN, i, 1)
                            sK = Mid(Chiave, i, 1)

                            intIN = CInt(sI)
                            intKEY = CInt(sK)

                            Cifra = intIN - intKEY

                            If (Cifra < 0) Then
                                Cifra = Cifra + 10
                            End If

                            strOUT = strOUT & CStr(Cifra).Trim

                        Next

                        'Tolgo la costante di normalizzazione
                        intOUT = CInt(strOUT)
                        intOUT = intOUT - (10000000 * VersioneAlgoritmo)
                        strOUT = CStr(intOUT)

                        'Fine lavoro
                        Dato_Normale = strOUT

                        '///////////////////////////////////////////////////////////////
                        '///// DECODIFICA NUMERO fine //////////////////////////////////
                        '///////////////////////////////////////////////////////////////


                End Select
                '///////////////////////////////////////////////////////////////
                '///////////////////////////////////////////////////////////////
                '///// NUMERO fine /////////////////////////////////////////////
                '///////////////////////////////////////////////////////////////
                '///////////////////////////////////////////////////////////////





            Case enum_DataLock_TipoDato.Datalock_STRINGA
                '///////////////////////////////////////////////////////////////
                '///////////////////////////////////////////////////////////////
                '///// STRINGA /////////////////////////////////////////////////
                '///////////////////////////////////////////////////////////////
                '///////////////////////////////////////////////////////////////
                Select Case Flag_Operazione

                    Case enum_DataLock_Operazione.DataLock_CODIFICA
                        '///////////////////////////////////////////////////////////////
                        '///// CODIFICA STRINGA ////////////////////////////////////////
                        '///////////////////////////////////////////////////////////////




                        'Fine lavoro
                        Dato_Criptato = strOUT

                        '///////////////////////////////////////////////////////////////
                        '///// CODIFICA STRINGA fine ///////////////////////////////////
                        '///////////////////////////////////////////////////////////////





                    Case enum_DataLock_Operazione.Datalock_DECODIFICA
                        '///////////////////////////////////////////////////////////////
                        '///// DECODIFICA STRINGA //////////////////////////////////////
                        '///////////////////////////////////////////////////////////////




                        'Fine lavoro
                        Dato_Normale = strOUT

                        '///////////////////////////////////////////////////////////////
                        '///// DECODIFICA STRINGA fine /////////////////////////////////
                        '///////////////////////////////////////////////////////////////


                End Select
                '///////////////////////////////////////////////////////////////
                '///////////////////////////////////////////////////////////////
                '///// STRINGA fine ////////////////////////////////////////////
                '///////////////////////////////////////////////////////////////
                '///////////////////////////////////////////////////////////////


        End Select

        ''Testo = Replace(Testo, "\", "@")

        'Dim Cript As Boolean = True

        'Dim strEncrypted As String
        'Dim i As Integer
        'Dim A1 As Integer
        'Dim A2 As Integer
        'Dim KeyPos As Byte

        'KeyPos = 1

        'For i = 1 To Len(Testo)
        '    A1 = Asc(Mid(Testo, i, 1))
        '    A2 = Asc(Mid(Chiave, KeyPos, 1))
        '    If Cript Then
        '        strEncrypted = strEncrypted & Chr(A2 + A1)
        '    Else
        '        strEncrypted = strEncrypted & Chr(A1 - A2)
        '    End If
        '    KeyPos = KeyPos + 1
        '    If KeyPos > Len(Chiave) Then KeyPos = 1
        'Next

    End Sub



    '#####################################################################
    Public Function ChiaveGiasOnline_Decodifica(ByVal Chiave As String,
                                                ByVal UsernamePresunta As String,
                                                ByRef Username As String,
                                                ByRef CodiceProgressivoGIAS As Integer,
                                                ByRef DataAttivazione As Date,
                                                ByRef DataScadenza As Date,
                                                ByRef NumeroAziende As Integer,
                                                ByRef NumeroUtenti As Integer,
                                                ByRef SuperficieTotale As Integer,
                                                ByRef NumeroAccessi As Integer,
                                                ByRef NumeroUtilizzi As Integer,
                                                ByRef DataGenerazioneChiave As Date,
                                                ByRef VersioneCodifica As Integer,
                                                ByRef VersioneChiave As Integer,
                                                ByRef Modulo_Flag() As Integer,
                                                ByRef Modulo_Inizio() As Date,
                                                ByRef Modulo_Fine() As Date,
                                                ByRef Checksum As String,
                                                ByRef MessaggioErrore As String
                                                ) As Boolean

        '---------------------------------------------------------------------
        '----- Definisco le variabili
        '---------------------------------------------------------------------

        Dim ChiaveLunghezza As Integer
        Dim NumeroModuli As Integer
        Dim Chiave3 As String

        Dim i As Integer
        Dim Testo As String

        Dim Pos_Username As Integer
        Dim Pos_CodiceProgressivoGIAS As Integer
        Dim Pos_DataAttivazione As Integer
        Dim Pos_DataScadenza As Integer
        Dim Pos_NumeroAziende As Integer
        Dim Pos_NumeroUtenti As Integer
        Dim Pos_SuperficieTotale As Integer
        Dim Pos_NumeroAccessi As Integer
        Dim Pos_NumeroUtilizzi As Integer
        Dim Pos_DataGenerazioneChiave As Integer
        Dim Pos_VersioneCodifica As Integer
        Dim Pos_VersioneChiave As Integer
        Dim Pos_Modulo As String()
        Dim Pos_Checksum As Integer

        Dim Num_Username As Integer
        Dim Num_CodiceProgressivoGIAS As Integer
        Dim Num_DataAttivazione As Integer
        Dim Num_DataScadenza As Integer
        Dim Num_NumeroAziende As Integer
        Dim Num_NumeroUtenti As Integer
        Dim Num_SuperficieTotale As Integer
        Dim Num_NumeroAccessi As Integer
        Dim Num_NumeroUtilizzi As Integer
        Dim Num_DataGenerazioneChiave As Integer
        Dim Num_VersioneCodifica As Integer
        Dim Num_VersioneChiave As Integer
        Dim Num_Modulo As Integer
        Dim Num_Checksum As Integer

        Dim Carattere As String
        Dim Codice As Integer

        MessaggioErrore = ""


        '---------------------------------------------------------------------
        '----- Recupero la VERSIONE dell'ALGORITMO di CODIFICA ... e decodifico
        '---------------------------------------------------------------------

        'Recupero la versione
        Pos_VersioneCodifica = 1
        Num_VersioneCodifica = 2

        VersioneCodifica = CInt(Mid(Chiave, Pos_VersioneCodifica, Num_VersioneCodifica))


        Select Case VersioneCodifica

            Case 1

                Chiave3 = Mid(Chiave, 3)
                Chiave = Mid(Chiave, 1, 2) & Stringa_DecodificaKey(Chiave3, True)

            Case Else

                'Creo una stringa vuota (riempio di caratteri ".")
                Chiave = Space(ChiaveLunghezza).Replace(" ", ".")

        End Select


        'Debug
        Testo = Chiave



        '---------------------------------------------------------------------
        '----- Recupero la VERSIONE della CHIAVE ...
        '---------------------------------------------------------------------

        Pos_VersioneChiave = 26
        Num_VersioneChiave = 3

        'Recupero la versione
        VersioneChiave = CInt(Mid(Chiave, Pos_VersioneChiave, Num_VersioneChiave))

        Select Case VersioneChiave


            Case 1

                'Definisco la lunghezza della stringa
                ChiaveLunghezza = 1000

                'Definisco il numero di moduli
                NumeroModuli = 16

                'Definisco la posizione degli elementi nella stringa
                Pos_Username = 181
                Pos_CodiceProgressivoGIAS = 15
                Pos_DataAttivazione = 49
                Pos_DataScadenza = 101
                Pos_NumeroAziende = 141
                Pos_NumeroUtenti = 89
                Pos_SuperficieTotale = 134
                Pos_NumeroAccessi = 148
                Pos_NumeroUtilizzi = 41
                Pos_DataGenerazioneChiave = 4
                Pos_VersioneCodifica = 1
                Pos_VersioneChiave = 26
                Pos_Checksum = 98

                Num_Username = 50
                Num_CodiceProgressivoGIAS = 6
                Num_DataAttivazione = 10
                Num_DataScadenza = 10
                Num_NumeroAziende = 6
                Num_NumeroUtenti = 6
                Num_SuperficieTotale = 6
                Num_NumeroAccessi = 6
                Num_NumeroUtilizzi = 6
                Num_DataGenerazioneChiave = 10
                Num_VersioneCodifica = 2
                Num_VersioneChiave = 3
                Num_Checksum = 3
                Num_Modulo = 21

                'Per i moduli uso un trucchetto .... mi devo ricordare di usare la funzione CINT()
                Pos_Modulo = Split("261,284,310,343," &
                                   "374,396,423,449," &
                                   "491,515,559,584," &
                                   "627,693,716,758", ",")

            Case Else



        End Select


        '---------------------------------------------------------------------
        '----- Recupero la USERNAME e confronto con il valore presunto ...
        '---------------------------------------------------------------------

        'Recupero la Username
        Username = Mid(Chiave, Pos_Username, Num_Username).Trim

        If LCase(Username) <> LCase(UsernamePresunta) Then

            'Imposto i valori
            Username = ""
            CodiceProgressivoGIAS = 0
            DataAttivazione = #1/1/1900#
            DataScadenza = #1/1/1900#
            NumeroAziende = 0
            NumeroUtenti = 0
            SuperficieTotale = 0
            NumeroAccessi = 0
            NumeroUtilizzi = 0
            DataGenerazioneChiave = #1/1/1900#
            VersioneCodifica = 0
            VersioneChiave = 0
            Erase Modulo_Flag
            Erase Modulo_Inizio
            Erase Modulo_Fine
            Checksum = "000"
            MessaggioErrore = "Username non corretto !!!"

            'Esito negativo
            Return False

        End If


        '---------------------------------------------------------------------
        '----- Ricalcolo il CHECKSUM della chiave e confronto ...
        '---------------------------------------------------------------------

        'Recupero il valore memorizzato
        Checksum = Mid(Chiave, Pos_Checksum, Num_Checksum)

        'Sostituisco il valore "000" con il quale e' stato fatto il calcolo
        Mid(Chiave, Pos_Checksum, Num_Checksum) = "000"

        '--- Ricalcolo il Checksum

        Dim Somma As Integer
        Dim NewChecksum As String

        'Inizializzo
        Somma = 0

        For i = 1 To ChiaveLunghezza

            'Recupero il carattere
            Carattere = Mid(Chiave, i, 1)

            'Conversione ASCII
            Codice = Asc(Carattere)

            'Sommo
            Somma = Somma + Codice

        Next


        Testo = "0000000" & CStr(Somma)

        'Considero come Checksum i tre caratteri meno significativi
        NewChecksum = Microsoft.VisualBasic.Strings.Right(Testo, 3)


        '--- Verifico il Checksum

        If NewChecksum <> Checksum Then

            'Imposto i valori
            Username = ""
            CodiceProgressivoGIAS = 0
            DataAttivazione = #1/1/1900#
            DataScadenza = #1/1/1900#
            NumeroAziende = 0
            NumeroUtenti = 0
            SuperficieTotale = 0
            NumeroAccessi = 0
            NumeroUtilizzi = 0
            DataGenerazioneChiave = #1/1/1900#
            VersioneCodifica = 0
            VersioneChiave = 0
            Erase Modulo_Flag
            Erase Modulo_Inizio
            Erase Modulo_Fine
            Checksum = "000"
            MessaggioErrore = "Checksum non corretta : possibile manomissione licenza !!!"

            'Esito negativo
            Return False

        End If


        '---------------------------------------------------------------------
        '----- Estraggo le informazioni ...
        '---------------------------------------------------------------------

        '--- Estraggo le informazioni generali

        Username = Mid(Chiave, Pos_Username, Num_Username).Trim
        CodiceProgressivoGIAS = CInt(Mid(Chiave, Pos_CodiceProgressivoGIAS, Num_CodiceProgressivoGIAS))
        DataAttivazione = CDate(Mid(Chiave, Pos_DataAttivazione, Num_DataAttivazione))
        DataScadenza = CDate(Mid(Chiave, Pos_DataScadenza, Num_DataScadenza))
        NumeroAziende = CInt(Mid(Chiave, Pos_NumeroAziende, Num_NumeroAziende))
        NumeroUtenti = CInt(Mid(Chiave, Pos_NumeroUtenti, Num_NumeroUtenti))
        SuperficieTotale = CInt(Mid(Chiave, Pos_SuperficieTotale, Num_SuperficieTotale))
        NumeroAccessi = CInt(Mid(Chiave, Pos_NumeroAccessi, Num_NumeroAccessi))
        NumeroUtilizzi = CInt(Mid(Chiave, Pos_NumeroUtilizzi, Num_NumeroUtilizzi))
        DataGenerazioneChiave = CDate(Mid(Chiave, Pos_DataGenerazioneChiave, Num_DataGenerazioneChiave))
        VersioneCodifica = CInt(Mid(Chiave, Pos_VersioneCodifica, Num_VersioneCodifica))
        VersioneChiave = CInt(Mid(Chiave, Pos_VersioneChiave, Num_VersioneChiave))


        '--- Estraggo le informazioni sui moduli

        'Dimensiono i vettori
        ReDim Modulo_Flag(NumeroModuli - 1)
        ReDim Modulo_Inizio(NumeroModuli - 1)
        ReDim Modulo_Fine(NumeroModuli - 1)

        'Recupero
        For i = 1 To NumeroModuli

            'Recupero le info complessive del modulo
            Testo = Mid(Chiave, CInt(Pos_Modulo(i - 1)), Num_Modulo)

            'Spezzo nei tre elementi
            Modulo_Flag(i - 1) = CInt(Mid(Testo, 1, 1))

            If IsDate(Mid(Testo, 2, 10)) Then
                Modulo_Inizio(i - 1) = CDate(Mid(Testo, 2, 10))
            Else
                Modulo_Inizio(i - 1) = #2/2/1900#
            End If

            If IsDate(Mid(Testo, 12, 10)) Then
                Modulo_Fine(i - 1) = CDate(Mid(Testo, 12, 10))
            Else
                Modulo_Fine(i - 1) = #3/3/1900#
            End If

        Next


        '---------------------------------------------------------------------
        '----- Se mi trovo qui ... tutto OK ...
        '---------------------------------------------------------------------

        Return True



    End Function



    Public Function GeneraNuovoHash(ByVal stringToHash As String) As String
        Dim deriveBytes As New Rfc2898DeriveBytes(stringToHash, SaltSize, PBKDF2iterCount)

        Dim salt As Byte() = deriveBytes.Salt
        Dim subkey As Byte() = deriveBytes.GetBytes(PBKDF2SubkeyLength)

        Dim bytesNewHashedString(SaltSize + PBKDF2SubkeyLength) As Byte
        Buffer.BlockCopy(salt, 0, bytesNewHashedString, 1, SaltSize)
        Buffer.BlockCopy(subkey, 0, bytesNewHashedString, 1 + SaltSize, PBKDF2SubkeyLength)
        Return Convert.ToBase64String(bytesNewHashedString)

        'Dim handleSha512 As New SHA512Managed
        'Dim hashPwd As Byte() = handleSha512.ComputeHash(Encoding.UTF8.GetBytes(Password))
        'Dim pwdToDb = Convert.ToBase64String(hashPwd)
        'Dim sBuilder As New Text.StringBuilder()
        'For i As Integer = 0 To outputBytes.Length - 1
        '    sBuilder.Append(outputBytes(i).ToString("X2"))
        'Next
    End Function

    Public Function GeneraHashConfronto(ByVal hashedString As String, ByVal stringToHash As String) As String
        Dim bytesHashedString As Byte() = OttieniBytesDaHash(hashedString)

        If bytesHashedString Is Nothing Then
            Throw New ArgumentException("L'hash fornito non è in un formato corretto")
        End If

        Dim saltFromHashedString(SaltSize - 1) As Byte
        Buffer.BlockCopy(bytesHashedString, 1, saltFromHashedString, 0, SaltSize)
        'Il codice seguente è necessario in caso in cui occorra effettuare il confronto fra i due array di byte piuttosto che sulla codifica
        'Dim subkeyFromHashedString(PBKDF2SubkeyLength - 1) As Byte
        'Buffer.BlockCopy(bytesHashedString, 1 + SaltSize, subkeyFromHashedString, 0, PBKDF2SubkeyLength)

        Dim verifyDeriveBytes As New Rfc2898DeriveBytes(stringToHash, saltFromHashedString, PBKDF2iterCount)
        Dim newSubKey As Byte() = verifyDeriveBytes.GetBytes(PBKDF2SubkeyLength)

        Dim bytesVerifyHashedString(SaltSize + PBKDF2SubkeyLength) As Byte
        Buffer.BlockCopy(saltFromHashedString, 0, bytesVerifyHashedString, 1, SaltSize)
        Buffer.BlockCopy(newSubKey, 0, bytesVerifyHashedString, 1 + SaltSize, PBKDF2SubkeyLength)
        Return Convert.ToBase64String(bytesVerifyHashedString)
    End Function

    Public Function OttieniBytesDaHash(ByVal hashedString As String) As Byte()
        Dim outBytesHashedString As Byte() = Nothing

        If hashedString.Length Mod 4 = 0 Then
            Dim regexPerBase64 As New Regex("\A[a-zA-Z\d\/+]+={0,2}\z", RegexOptions.None, TimeSpan.FromSeconds(3))

            If regexPerBase64.IsMatch(hashedString) Then
                'La funzione lancia eccezione se le viene fornita una stringa che non è base64
                'pertanto, per evitare ciò i due controlli precedenti sulla lunghezza della stringa come multiplo di 4 e la regex sul set di caratteri
                'sono sufficienti per evitare l'eccezione
                outBytesHashedString = Convert.FromBase64String(hashedString)

                If (Not outBytesHashedString.Length = (1 + SaltSize + PBKDF2SubkeyLength)) OrElse
                   (Not outBytesHashedString(0) = 0) Then
                    outBytesHashedString = Nothing
                End If
            End If
        End If

        Return outBytesHashedString
    End Function

    ' Recupera la stringa di connessione dalle variabili application
    Public Shared Function GetStringaConnessione(ByVal ID_DB As String) As String
        Try
            Dim StringheConnessione As Dictionary(Of String, String)
            StringheConnessione = HttpContext.Current.Application("GiasStringheConnessione")
            Return StringheConnessione(ID_DB)
        Catch ex As Exception
            Throw New Exception("Impossibile ricavare la stringa di connessione: " & ID_DB)
        End Try
    End Function

    ' Verifica esistenza della stringa di connessione in application
    Public Shared Function ExistStringaConnessione(ByVal ID_DB As String) As Boolean
        If Not IsNothing(HttpContext.Current) Then
            If Not IsNothing(HttpContext.Current.Application("GiasStringheConnessione")) Then
                Dim StringheConnessione As Dictionary(Of String, String)
                StringheConnessione = HttpContext.Current.Application("GiasStringheConnessione")
                Return StringheConnessione.ContainsKey(ID_DB)
            End If
        End If
        Return False
    End Function

    ' Restituisce stringa di connessione effettuando eventuale criptazione/decriptazione di user e password
    Public Shared Function ReadConnectionString(ByRef row As DataRow,
                                                 ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                                 Optional ByVal cryptKey As String = "",
                                                 Optional ByVal toCrypt As Boolean = False) As String

        Dim idDB As Integer = CInt(row.Item("ID_DB"))
        Dim provider As String = CStr(row.Item("Provider"))
        Dim server As String = CStr(row.Item("Server"))
        Dim db As String = CStr(row.Item("DB"))
        Dim userId As String = CStr(row.Item("UserId"))
        Dim password As String = CStr(row.Item("Password"))

        If Not String.IsNullOrEmpty(cryptKey) Then

            Dim sicurezza As New Sicurezza
            Dim flagIsEncrypted As Integer = CInt(row.Item("Flag_Encrypted"))

            If flagIsEncrypted = 0 AndAlso toCrypt Then

                Dim encryptedUserId = sicurezza.EncryptString(userId, cryptKey)
                Dim encryptedPassword = sicurezza.EncryptString(password, cryptKey)

                Dim connessioni As New Connessioni
                connessioni.Modifica(idDB, enum_Tipo_DB.TUTTI,
                                     "", "", "", encryptedUserId, encryptedPassword,
                                     "", "", 0, "", "", "", objParametri_Super_Server, "1")

            ElseIf flagIsEncrypted = 1 Then

                userId = sicurezza.DecryptString(userId, cryptKey)
                password = sicurezza.DecryptString(password, cryptKey)

            End If

        End If

        Dim Stringa_Connessione As String =
            "Provider=" & CStr(row.Item("Provider")) &
            ";Server=" & CStr(row.Item("Server")) &
            ";Initial Catalog=" & CStr(row.Item("DB")) &
            ";User Id=" & userId &
            ";Password=" & password & ";"

        Return Stringa_Connessione

    End Function

    Public Shared Function Leggi_Stringa_Connessione(ByVal ID_DB As Integer, ByRef objParametri_Super_Server As AgronicaCoreParametri) As String

        Dim connectionString As String = ""

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DEBUG_LogUMAObj")) Then
            Dim filePath As String = HttpContext.Current.Server.MapPath("~/DebugObjParamUMA.txt")
            Using fstr As IO.FileStream = IO.File.Open(filePath, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.ReadWrite)
                Dim sw As New IO.StreamWriter(fstr)
                sw.WriteLine("Sicurezza - Leggi_Stringa_Connessione" & If(IsNothing(objParametri_Super_Server), "Nope", objParametri_Super_Server.ToString))
                sw.Flush()
                sw.Dispose()
            End Using

        End If
        Dim cryptKey As String = Leggi_Valore_Configurazione("cr", objParametri_Super_Server)

        Dim connessioni As New Connessioni
        Dim dt As DataTable = connessioni.Leggi_Connessione(ID_DB, objParametri_Super_Server)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            connectionString = ReadConnectionString(dt.Rows(0), objParametri_Super_Server, cryptKey)
        End If

        Return connectionString

    End Function

    Private Shared Function Leggi_Valore_Configurazione(ByVal Chiave As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim valore As String = ""

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DEBUG_LogUMAObj")) Then

            Dim filePath As String = HttpContext.Current.Server.MapPath("~/DebugObjParamUMA.txt")
            Using fstr As IO.FileStream = IO.File.Open(filePath, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.ReadWrite)
                Dim sw As New IO.StreamWriter(fstr)
                sw.WriteLine("Sicurezza Leggi_Valore_Configurazione: " & If(IsNothing(objParametri), "nope", objParametri.ToString))
                sw.Flush()
                sw.Dispose()
            End Using
        End If
        Dim provider As IDataProvider = DataProviderFactory.Instance.Provider
        Dim sqlString As String = "select * from configurazione_siti where chiave = '" & Chiave & "'"
        Dim dt As DataTable = provider.EseguiQuery_Lettura(objParametri, sqlString, "")

        If dt.Rows.Count > 1 Then
            Throw New Exception("Troppi record selezionati")
        End If
        If dt.Rows.Count = 1 Then
            valore = dt.Rows(0).Item("Valore")
        End If

        Return valore

    End Function

End Class
