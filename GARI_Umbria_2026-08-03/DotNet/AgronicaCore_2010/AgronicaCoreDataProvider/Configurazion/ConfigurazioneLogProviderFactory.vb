Imports System.Configuration
Imports System.Text
Imports Microsoft.SqlServer.Management.SqlParser.Metadata
Imports Newtonsoft.Json

Public Class ConfigurazioneLogProviderFactory : Inherits ConfigurazioneBase

    Public Const LIMITE_ERRORI_PER_INSERIMENTO As Integer = 500

    Private Shared classLocker As New Object()
    Private Shared objSingleton As ConfigurazioneLogProviderEsteso

    Private Shared ReadOnly _databaseDaNonConsiderare As List(Of String) = New List(Of String) From
        {
            "_super_server", "_utenti", "_matrice", "_disciplinari"
        }

    Public Shared Function Instance(ByVal objParametri As AgronicaCoreParametri) As ConfigurazioneLogProviderEsteso

        If (objSingleton Is Nothing) Then
            ' Thread Safe
            SyncLock (classLocker)
                If (objSingleton Is Nothing) Then

                    objSingleton = LeggiConfigurazione(objParametri)

                End If
            End SyncLock
        End If
        Return objSingleton

    End Function

    Private Shared Function LeggiConfigurazione(ByVal objParametri As AgronicaCoreParametri) As ConfigurazioneLogProviderEsteso

        If objParametri Is Nothing Then
            Return Nothing
        End If

        If _databaseDaNonConsiderare.Any(Function(d) objParametri.StringaConnessione.ToLower.Contains(d)) Then
            Return Nothing
        End If

        Dim provider As IDataProvider = DataProviderFactory.Instance.Provider
        Dim sb = New StringBuilder

        Try

            If Not EsisteTabella("configurazione_siti", objParametri) Then
                Return Nothing
            End If

            sb.AppendLine(" select * from configurazione_siti")
            sb.AppendLine(" where chiave IN ('ClientSMTP','ClientSMTP_Porta','enablessl_smtp','password_smtp','password_smtp_isEncrypted','user_smtp', 'MailFrom_smtp', 'DataProviderLogConfig','Coldiretti_ElasticSearchUrl','ParametriElasticSearch','cr' )")
            Dim dt As DataTable = provider.EseguiQuery_Lettura(objParametri, sb.ToString, "", chiamaScriviLog:=False)
            Dim mailFrom As String = String.Empty

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                Dim chiavi = (From r In dt.AsEnumerable
                              Select New With {
                                    .Nome = CStr(r.Item("Chiave")),
                                    .Valore = CStr(r.Item("Valore"))
                        }).ToList()

                Dim cfg = New ConfigurazioneLogProviderEsteso
                cfg.ConfigurazioneSmtp = New ConfigurazioneSmtp
                Dim chiave = chiavi.FirstOrDefault(Function(c) c.Nome = "ClientSMTP")
                If chiave IsNot Nothing Then
                    cfg.ConfigurazioneSmtp.ClientSMTP = chiave.Valore
                End If
                chiave = chiavi.FirstOrDefault(Function(c) c.Nome = "ClientSMTP_Porta")
                If chiave IsNot Nothing Then
                    cfg.ConfigurazioneSmtp.ClientSMTP_Porta = chiave.Valore
                End If
                chiave = chiavi.FirstOrDefault(Function(c) c.Nome = "enablessl_smtp")
                If chiave IsNot Nothing Then
                    cfg.ConfigurazioneSmtp.Enablessl_SMTP = chiave.Valore
                End If

                Dim password_smtp_KeyAndValue = chiavi.FirstOrDefault(Function(c) c.Nome = "password_smtp")
                Dim password_smtp_isEncrypted_KeyAndValue = chiavi.FirstOrDefault(Function(c) c.Nome = "password_smtp_isEncrypted")
                Dim crKeyAndValue = chiavi.FirstOrDefault(Function(c) c.Nome = "cr")

                If Not IsNothing(password_smtp_isEncrypted_KeyAndValue) AndAlso Not String.IsNullOrEmpty(password_smtp_isEncrypted_KeyAndValue.Valore) Then

                    If Not IsNothing(password_smtp_KeyAndValue) AndAlso Not String.IsNullOrEmpty(password_smtp_KeyAndValue.Valore) Then

                        If password_smtp_isEncrypted_KeyAndValue.Valore = "0" Then

                            cfg.ConfigurazioneSmtp.Password_SMTP = password_smtp_KeyAndValue.Valore
                            If (Not IsNothing(crKeyAndValue)) AndAlso Not String.IsNullOrEmpty(crKeyAndValue.Valore) Then
                                Dim sicurezza As New Sicurezza
                                Dim encryptedField = sicurezza.EncryptString(password_smtp_KeyAndValue.Valore, crKeyAndValue.Valore)

                                Dim chiaviDaAggiornare As String() = {"password_smtp", "password_smtp_isEncrypted"}
                                Dim valoriDaAggiornare As String() = {encryptedField, "1"}

                                Dim stb As New StringBuilder
                                stb.Length = 0

                                stb.AppendLine(" UPDATE Configurazione_Siti")
                                stb.AppendLine(" SET    Valore = CASE ")

                                For i = 0 To chiaviDaAggiornare.Length - 1
                                    Dim chiaveDaAggiornare As String = chiaviDaAggiornare(i)
                                    Dim valore As String = valoriDaAggiornare(i)

                                    stb.AppendLine($"    WHEN   Chiave = '{chiaveDaAggiornare}' THEN '{valore}' ")
                                Next
                                stb.AppendLine("    ELSE   Valore   END")
                                stb.AppendLine(" WHERE  Chiave IN (")

                                For i = 0 To chiaviDaAggiornare.Length - 1
                                    Dim chiaveDaAggiornare As String = chiaviDaAggiornare(i)

                                    If (i <> (chiaviDaAggiornare.Length - 1)) Then
                                        stb.AppendLine($" '{chiaveDaAggiornare}', ")
                                    Else
                                        stb.AppendLine($" '{chiaveDaAggiornare}' )")
                                    End If
                                Next

                                provider.EseguiQuery_Scrittura(objParametri, stb.ToString(), NameOf(LeggiConfigurazione))

                            End If

                        ElseIf password_smtp_isEncrypted_KeyAndValue.Valore = "1" Then

                            If Not IsNothing(crKeyAndValue) AndAlso Not String.IsNullOrEmpty(crKeyAndValue.Valore) Then
                                Dim sicurezza As New Sicurezza
                                cfg.ConfigurazioneSmtp.Password_SMTP = sicurezza.DecryptString(password_smtp_KeyAndValue.Valore, crKeyAndValue.Valore)
                            End If

                        Else
                            Throw New NotImplementedException("Valore della chiave password_smtp_isEncrypted non gestita")
                        End If

                    End If

                End If

                chiave = chiavi.FirstOrDefault(Function(c) c.Nome = "user_smtp")
                If chiave IsNot Nothing Then
                    cfg.ConfigurazioneSmtp.User_SMTP = chiave.Valore
                End If
                chiave = chiavi.FirstOrDefault(Function(c) c.Nome = "MailFrom_smtp")
                If chiave IsNot Nothing Then
                    mailFrom = chiave.Valore
                End If

                Dim cfgES As New ConfigurazioneElasticSearch
                chiave = chiavi.FirstOrDefault(Function(c) c.Nome = "Coldiretti_ElasticSearchUrl")
                If chiave IsNot Nothing AndAlso Not String.IsNullOrEmpty(chiave.Valore) AndAlso Not String.IsNullOrWhiteSpace(chiave.Valore) Then
                    cfgES.ElasticSearchUrl = chiave.Valore
                End If
                chiave = chiavi.FirstOrDefault(Function(c) c.Nome = "ParametriElasticSearch")
                If chiave IsNot Nothing AndAlso Not String.IsNullOrEmpty(chiave.Valore) AndAlso Not String.IsNullOrWhiteSpace(chiave.Valore) Then
                    cfgES.Parametri = JsonConvert.DeserializeObject(Of ParametriElasticSearch)(chiave.Valore)
                End If
                cfg.ConfigurazioneElasticSearch = cfgES

                chiave = chiavi.FirstOrDefault(Function(c) c.Nome = "DataProviderLogConfig")
                If chiave IsNot Nothing AndAlso Not String.IsNullOrEmpty(chiave.Valore) Then
                    Dim cfgLog As ConfigurazioneLogProviderEsteso = JsonConvert.DeserializeObject(Of ConfigurazioneLogProviderEsteso)(chiave.Valore)

                    cfg.Ambiente = ""
                    If Not IsNothing(cfgLog) AndAlso Not IsNothing(cfgLog.Ambiente) Then
                        cfg.Ambiente = cfgLog.Ambiente
                    End If

                    cfg.Limite_Errori_X_Scrittura_DBLog = LIMITE_ERRORI_PER_INSERIMENTO
                    If Not IsNothing(cfgLog) AndAlso Not IsNothing(cfgLog.Limite_Errori_X_Scrittura_DBLog) AndAlso
                    Not String.IsNullOrEmpty(cfgLog.Limite_Errori_X_Scrittura_DBLog) Then
                        Dim lepi As Integer = 0
                        If Int32.TryParse(cfgLog.Limite_Errori_X_Scrittura_DBLog, lepi) Then
                            cfg.Limite_Errori_X_Scrittura_DBLog = lepi
                        End If
                    End If

                    cfg.LivelloLOG = cfgLog.LivelloLOG
                    'cfg.Mittente_Mail = MailFrom_smtp
                    cfg.Mittente_Mail = mailFrom
                    cfg.Destinatari_Mail = cfgLog.Destinatari_Mail

                    If String.IsNullOrEmpty(cfg.Mittente_Mail) Then
                        cfg.LivelloLOG = TipiEnumerativi.enum_Livello_Log_Applicazioni.LogSoloSuEmailEDB
                    End If

                Else
                    cfg.LivelloLOG = TipiEnumerativi.enum_Livello_Log_Applicazioni.LogSoloSuEmailEDB
                End If

                Return cfg
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return New ConfigurazioneLogProviderEsteso With
            {
                .LivelloLOG = TipiEnumerativi.enum_Livello_Log_Applicazioni.LogSoloSuEmailEDB
            }
        End Try

    End Function

End Class
