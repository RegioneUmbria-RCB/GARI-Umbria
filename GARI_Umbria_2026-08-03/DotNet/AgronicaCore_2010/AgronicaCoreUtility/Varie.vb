Imports System.Text.RegularExpressions
Imports System.Web
Imports AgronicaCoreDataProvider.LogProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Varie

    Public Shared Function aggiungiAQueryString(queryString As String, chiave As String, valore As String) As String
        If queryString.Contains(chiave & "=" & valore) Then
            Return queryString
        End If

        If queryString.Contains("?") Then
            queryString &= String.Format("&{0}=", chiave)
        Else
            queryString &= String.Format("?{0}=", chiave)
        End If

        queryString &= valore

        Return queryString
    End Function


    Public Shared Function ObjectToByteArray(ByVal _Object As Object) As Byte()
        Try
            ' create new memory stream
            Dim _MemoryStream As New System.IO.MemoryStream()

            ' create new BinaryFormatter
            Dim _BinaryFormatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()

            ' Serializes an object, or graph of connected objects, to the given stream.
            _BinaryFormatter.Serialize(_MemoryStream, _Object)


            ' convert stream to byte array and return
            Return _MemoryStream.ToArray()
        Catch _Exception As Exception
            ' Error
            Console.WriteLine("Exception caught in process: {0}", _Exception.ToString())
        End Try

        ' Error occured, return null
        Return Nothing
    End Function

    Public Shared Function ByteArrayToObject(ByVal _byteArray As Byte()) As Object
        Try
            ' create new memory stream
            Dim _MemoryStream As New System.IO.MemoryStream()

            ' create new BinaryFormatter
            Dim _BinaryFormatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()


            _MemoryStream.Read(_byteArray, 0, _byteArray.Length - 1)
            ' Serializes an object, or graph of connected objects, to the given stream.
            Return _BinaryFormatter.Deserialize(_MemoryStream)

        Catch _Exception As Exception
            ' Error
            Console.WriteLine("Exception caught in process: {0}", _Exception.ToString())
        End Try

        ' Error occured, return null
        Return Nothing
    End Function

    Public Sub Log_X_Segnalazioni_Speciali(ByVal Oggetto As String, ByVal TestoMail As String, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional ByVal Includi_Stack_Chiamate As Boolean = True)

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        Try

            Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim DT_Config = objConfigurazione_Siti.Leggi(0, "", "", "", objParametri_Server)

            If Not IsNothing(DT_Config) AndAlso DT_Config.Rows.Count > 0 Then

                Dim Valore As enum_Livello_Log_Applicazioni = enum_Livello_Log_Applicazioni.Nessuno

                Dim Mittente As String = ""

                Dim Destinatario As String = ""

                Dim Dr = DT_Config.Select("Chiave = 'Log_Segnalazioni_Speciali'")

                If Not IsNothing(Dr) AndAlso Dr.Length = 1 AndAlso Not String.IsNullOrEmpty(Dr(0)("Valore")) AndAlso IsNumeric(Dr(0)("Valore")) Then
                    Valore = CInt(Dr(0)("Valore"))
                End If

                If Valore <> enum_Livello_Log_Applicazioni.Nessuno AndAlso Not String.IsNullOrEmpty(TestoMail) Then
                    If Includi_Stack_Chiamate Then
                        TestoMail &= Environment.StackTrace
                    End If

                    TestoMail &= "<br><br> Stringa di Connessione al DB: " & objParametri_Server.StringaConnessione & " <br> Utente: " & objParametri_Server.UtenteUsername
                End If

                If Valore = enum_Livello_Log_Applicazioni.LogSoloSuFile OrElse Valore = enum_Livello_Log_Applicazioni.LogFileEmailEDB OrElse
                    Valore = enum_Livello_Log_Applicazioni.LogFileEDB OrElse Valore = enum_Livello_Log_Applicazioni.LogFileEmail Then
                    objLog.Scrivi_LOG(objParametri_Server, Oggetto, TestoMail)
                End If

                If Valore = enum_Livello_Log_Applicazioni.LogSoloSuEmailEDB OrElse Valore = enum_Livello_Log_Applicazioni.LogFileEmailEDB OrElse
                    Valore = enum_Livello_Log_Applicazioni.LogSoloEmail OrElse Valore = enum_Livello_Log_Applicazioni.LogFileEmail Then

                    Dr = DT_Config.Select("Chiave = 'MailFrom_smtp'")

                    If Not IsNothing(Dr) AndAlso Dr.Length = 1 AndAlso Not String.IsNullOrEmpty(Dr(0)("Valore")) Then
                        Mittente = Dr(0)("Valore")
                    End If

                    Dr = DT_Config.Select("Chiave = 'Log_Segnalazioni_Speciali_Mail_A'")

                    If Not IsNothing(Dr) AndAlso Dr.Length = 1 AndAlso Not String.IsNullOrEmpty(Dr(0)("Valore")) Then
                        Destinatario = Dr(0)("Valore")
                    End If

                    If Not String.IsNullOrEmpty(Mittente) AndAlso Not String.IsNullOrEmpty(Destinatario) Then

                        Dim objUtility As New AgronicaCoreUtility.Mail

                        Dim erroreinvioMail = objUtility.invia(objParametri_Server, Mittente, Destinatario, "", "", Oggetto, TestoMail, True, {})

                        If Not String.IsNullOrEmpty(erroreinvioMail) Then
                            Throw New Exception("Errore Invio Mail")
                        End If
                    End If

                End If

            End If
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametri_Server, Oggetto, TestoMail)
        End Try


    End Sub

    Public Shared Sub SanitizeTesto_MantieniVirgoletteECaratteriAccentati(ByRef testo As String)
        '1. Caratteri pericolosi in HTML e JavaScript Injection:
        '   Tag HTML: <, >.
        '   Caratteri speciali HTML: & (entità HTML come & lt;), ' e " (delimitatori di attributi).
        '   Caratteri JavaScript: ()(funzioni), {}, [](strutture di codice), ;, = (assegnazioni).
        '2. Caratteri pericolosi in SQL Injection:
        '   Separatori e commenti ;, --, /, *.
        '   Delimitatori stringhe: ', ".
        '3. Caratteri di escape
        '    Backslash \ (utilizzato per fare escaping).


        'Definizione della regex: consenti lettere (anche accentate), numeri, spazi, apici e doppi apici
        Dim pattern As String = "[^a-zA-Z0-9àèéìòùÀÈÉÌÒÙçÇ .,?!\-_]"

        'Sostituzione dei caratteri non consentiti con stringa vuota
        testo = Regex.Replace(testo, pattern, "", RegexOptions.None, TimeSpan.FromSeconds(3))
    End Sub

    Public Class objAllegato
        Public Property NomeFile As String
        Public Property Estensione As String
        Public Property File As Byte()
    End Class

End Class
