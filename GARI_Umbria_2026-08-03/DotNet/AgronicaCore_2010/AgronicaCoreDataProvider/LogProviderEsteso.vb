Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json

Public Class LogProviderEsteso
    Inherits LogProvider
    Private Shared dbLocker As New Object

    Sub New()

    End Sub

    Public Sub Logga_Solo_DB(
                    ByVal NomeRoutine As String,
                    ByVal subject As String,
                    ByVal MessaggioErrore As String,
                    objParametri As AgronicaCoreParametri)

        Try
            Dim cfg = ConfigurazioneLogProviderFactory.Instance(objParametri)
            If cfg IsNot Nothing Then
                If cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogFileEmailEDB OrElse cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogSoloSuEmailEDB OrElse
                    cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogSoloDB OrElse cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogFileEDB Then

                    Dim sha As HashAlgorithm = SHA256.Create()

                    Dim oggetto As New DbLogClass With {
                            .Errore = MessaggioErrore,
                            .Username = If(IsNothing(objParametri.UtenteUsername), "", objParametri.UtenteUsername),
                            .db = DataProviderFactory.Instance.Provider().NomeDataBase_FromStringaConnessione(objParametri.StringaConnessione),
                            .Data = Date.Now.ToShortDateString,
                            .istanza = DataProviderFactory.Instance.Provider().NomeIstanza_FromStringaConnessione(objParametri.StringaConnessione),
                            .routine = NomeRoutine,
                            .occorrenze = 1,
                            .hash = ""
                        }

                    Dim hash = BitConverter.ToString(sha.ComputeHash(ObjectToByteArray(oggetto)))
                    oggetto.hash = hash

                    If Not Debugger.IsAttached Then
                        ThreadPool.QueueUserWorkItem(Sub()
                                                         Try
                                                             PreparaScritturaSuDb(oggetto, objParametri)
                                                         Catch ex As Exception
                                                             Scrivi_LOG(objParametri, NomeRoutine,
                                                                         "Errore nella scrittura log su DB: " & ex.Message)
                                                         End Try
                                                     End Sub) 'ComponiESpedisciMail(cfg, subject, MessaggioErrore))
                    End If

                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Sub Logga(ByVal NomeRoutine As String,
                     ByVal MessaggioErrore As String,
                        Optional ByVal objParametri As AgronicaCoreParametri = Nothing,
                        Optional verificaInviaElasticSearch As Boolean = False)

        Dim sb As New StringBuilder

        Try
            sb.Append(MessaggioErrore)
            sb.AppendLine()
            If objParametri IsNot Nothing Then
                Dim utente As String = If(IsNothing(objParametri.UtenteUsername), "", objParametri.UtenteUsername)
                Dim nomedb As String = DataProviderFactory.Instance.Provider().NomeDataBase_FromStringaConnessione(objParametri.StringaConnessione)
                sb.AppendFormat("Database: {0}", nomedb)
                sb.AppendLine()
                sb.AppendFormat("Utente: {0}", utente)
            End If

            Dim cfg = ConfigurazioneLogProviderFactory.Instance(objParametri)
            If cfg Is Nothing Then

                ' Loggo solo su file
                If objParametri IsNot Nothing Then
                    'MyBase.Scrivi_LOG(objParametri.LogDirectory,
                    '                  objParametri.LogFileName,
                    '                  objParametri.LogDescrizioneUtente,
                    '                  NomeRoutine, sb.ToString()
                    '                )
                    MyBase.Scrivi_LOG(objParametri, NomeRoutine, sb.ToString(), verificaInviaElasticSearch)
                End If

            Else

                Dim provenienza As String = If(IsNothing(cfg.Ambiente), "", cfg.Ambiente)
                sb.AppendLine()
                sb.AppendFormat("Ambiente: {0}", provenienza.ToUpper)

                If cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogFileEmailEDB OrElse cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogSoloSuFile OrElse
                    cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogFileEDB OrElse cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogFileEmail Then
                    ' Loggo su file
                    If objParametri IsNot Nothing Then
                        'MyBase.Scrivi_LOG(objParametri.LogDirectory,
                        '                  objParametri.LogFileName,
                        '                  objParametri.LogDescrizioneUtente,
                        '                  NomeRoutine, sb.ToString
                        '                )
                        MyBase.Scrivi_LOG(objParametri, NomeRoutine, sb.ToString(), verificaInviaElasticSearch)
                    End If
                End If

                If cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogFileEmailEDB OrElse cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogSoloSuEmailEDB OrElse
                    cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogSoloEmail OrElse cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogFileEmail Then

                    If Not Debugger.IsAttached Then

                        Dim subject = "**** ERRORE DI PARAMETRIZZAZIONE NEL DATAPROVIDER O SQL INJECTION RILEVATA ****"
                        ThreadPool.QueueUserWorkItem(Sub()
                                                         Try
                                                             ComponiESpedisciMail(cfg, subject, sb.ToString)
                                                         Catch ex As Exception
                                                             Scrivi_LOG(objParametri, NomeRoutine,
                                                                        "Errore nell'invio mail di log: " & ex.Message)
                                                         End Try
                                                     End Sub)

                    End If

                End If

                If cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogFileEmailEDB OrElse cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogSoloSuEmailEDB OrElse
                    cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogSoloDB OrElse cfg.LivelloLOG = enum_Livello_Log_Applicazioni.LogFileEDB Then

                    If Not Debugger.IsAttached Then

                        Dim sha As HashAlgorithm = SHA256.Create()

                        Dim oggetto As New DbLogClass With {
                            .Errore = sb.ToString,
                            .Username = If(IsNothing(objParametri.UtenteUsername), "", objParametri.UtenteUsername),
                            .db = DataProviderFactory.Instance.Provider().NomeDataBase_FromStringaConnessione(objParametri.StringaConnessione),
                            .Data = Date.Now.ToShortDateString,
                            .istanza = DataProviderFactory.Instance.Provider().NomeIstanza_FromStringaConnessione(objParametri.StringaConnessione),
                            .routine = NomeRoutine,
                            .occorrenze = 1,
                            .hash = ""
                        }

                        Dim hash = BitConverter.ToString(sha.ComputeHash(ObjectToByteArray(oggetto)))
                        oggetto.hash = hash
                        oggetto.Data = Date.Now.ToString

                        Dim subject = "**** ERRORE DI PARAMETRIZZAZIONE NEL DATAPROVIDER O SQL INJECTION RILEVATA ****"
                        ThreadPool.QueueUserWorkItem(Sub()
                                                         Try
                                                             PreparaScritturaSuDb(oggetto, objParametri)
                                                         Catch ex As Exception
                                                             Scrivi_LOG(objParametri, NomeRoutine,
                                                                         "Errore nella scrittura log su DB: " & ex.Message)
                                                         End Try
                                                     End Sub)

                    End If

                End If

            End If
        Catch ex As Exception

        End Try


    End Sub

    Private Sub PreparaScritturaSuDb(ByVal oggettoLog As DbLogClass, ByRef objParametri As AgronicaCoreParametri)

        Dim oldOggetto = Nothing

        SyncLock (dbLocker)

            Dim hashDict = DataProviderDbLogFactory.Instance.HashDict

            If hashDict.ContainsKey(oggettoLog.hash) Then
                oggettoLog.occorrenze = hashDict.Item(oggettoLog.hash).occorrenze + 1
                hashDict.TryRemove(oggettoLog.hash, oldOggetto)
                hashDict.TryAdd(oggettoLog.hash, oggettoLog)
            Else
                hashDict.TryAdd(oggettoLog.hash, oggettoLog)
            End If

            Dim lepi As Integer = If(IsNothing(ConfigurazioneLogProviderFactory.Instance(objParametri)),
                                    ConfigurazioneLogProviderFactory.LIMITE_ERRORI_PER_INSERIMENTO,
                                    ConfigurazioneLogProviderFactory.Instance(objParametri).Limite_Errori_X_Scrittura_DBLog)

            If Not IsNothing(objParametri.StringaConnessione) AndAlso hashDict.Sum(Function(x) x.Value.occorrenze) >= lepi Then
                Dim bulkCopyList As List(Of DbLogClass) = hashDict.Values.ToList
                Dim jsonData = JsonConvert.SerializeObject(New With {Key .Table = bulkCopyList})
                Dim ds As DataSet = JsonConvert.DeserializeObject(Of DataSet)(jsonData)
                Dim dbCnn = New SqlConnection(DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione))
                dbCnn.Open()
                Using sqlBulkCopy As New SqlBulkCopy(dbCnn, SqlBulkCopyOptions.UseInternalTransaction, Nothing)
                    sqlBulkCopy.DestinationTableName = "Agronica_Log_Errori"
                    sqlBulkCopy.ColumnMappings.Add("Username", "Username")
                    sqlBulkCopy.ColumnMappings.Add("Errore", "Errore")
                    sqlBulkCopy.ColumnMappings.Add("hash", "Hash")
                    sqlBulkCopy.ColumnMappings.Add("Data", "Data_Ultimo_Log")
                    sqlBulkCopy.ColumnMappings.Add("istanza", "Istanza")
                    sqlBulkCopy.ColumnMappings.Add("db", "Nome_DB")
                    sqlBulkCopy.ColumnMappings.Add("routine", "Routine")
                    sqlBulkCopy.ColumnMappings.Add("occorrenze", "Occorrenze")
                    sqlBulkCopy.WriteToServer(ds.Tables(0))
                End Using
                dbCnn.Close()
                dbCnn.Dispose()
                DataProviderDbLogFactory.Instance.HashDict.Clear()
            End If

        End SyncLock

    End Sub

    Private Sub ComponiESpedisciMail(ByVal cfg As ConfigurazioneLogProviderEsteso, ByVal subject As String, ByVal body As String)

        Try

            Dim mail As New MailMessage With {
                .From = New MailAddress(cfg.Mittente_Mail)
            }

            If Not String.IsNullOrEmpty(cfg.Destinatari_Mail) AndAlso cfg.Destinatari_Mail.Trim() <> "" Then
                Dim destinatari = cfg.Destinatari_Mail.Split({";"c})
                For Each d As String In destinatari
                    mail.To.Add(d)
                Next
            End If

            mail.Subject = subject
            mail.IsBodyHtml = False
            mail.Body = body


            If Not String.IsNullOrEmpty(cfg.ConfigurazioneSmtp.ClientSMTP) Then

                Dim send As New SmtpClient(cfg.ConfigurazioneSmtp.ClientSMTP)
                send.Credentials = New NetworkCredential(cfg.ConfigurazioneSmtp.User_SMTP, cfg.ConfigurazioneSmtp.Password_SMTP)

                If Not String.IsNullOrEmpty(cfg.ConfigurazioneSmtp.Enablessl_SMTP) Then
                    Dim boolTester As Boolean
                    If Boolean.TryParse(cfg.ConfigurazioneSmtp.Enablessl_SMTP, boolTester) Then
                        send.EnableSsl = boolTester
                    End If
                End If

                If Not String.IsNullOrEmpty(cfg.ConfigurazioneSmtp.ClientSMTP_Porta) AndAlso IsNumeric(cfg.ConfigurazioneSmtp.ClientSMTP_Porta) Then
                    send.Port = CInt(cfg.ConfigurazioneSmtp.ClientSMTP_Porta)
                End If
                send.Send(mail)

            End If

        Catch ex As Exception
        End Try

    End Sub

    Private Function ObjectToByteArray(ByVal obj As Object) As Byte()
        If obj Is Nothing Then Return Nothing
        Dim bf As BinaryFormatter = New BinaryFormatter()



        Using ms As MemoryStream = New MemoryStream()
            bf.Serialize(ms, obj)
            Return ms.ToArray()
        End Using
    End Function

End Class

