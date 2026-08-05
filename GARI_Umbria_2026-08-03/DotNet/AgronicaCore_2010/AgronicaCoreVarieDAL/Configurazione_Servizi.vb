Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Configurazione_Servizio

    'chiave
    Public PivaSuperuser As String
    Public Id_Servizio As enum_Id_Servizio
    Public Tipo_Sincro As enum_Tipi_Servizi_Background
    Public Id_Riga As Integer

    'parametri operazione
    Public Id_Db As Integer
    Public Descrizione As String
    Public Username As String
    Public Password As String
    Public Id_Cod_Cliente As Integer
    Public Piva_Padre As String
    Public Tipo_Operazione As enum_Tipo_Operazione_Sincronizzatore
    Public DirectoryLOG As String
    Public DirectoryFileEsportazioni As String
    Public DirectoryFileImportazioni As String
    Public Parametri_Extra As String
    Public Host_Smtp As String
    Public Mittente As String
    Public Destinatari As String

    Public Host_Smtp_Porta As String
    Public smtp_user As String
    Public smtp_password As String
    Public smtp_enablessl As String

    'attivazione task
    Public Attivo As Integer

    'stato corrente
    Public Stato As Integer

    'parametri periodo esecuzione
    Public periodoPolling As Integer
    Public Esecuzione_Ora_Intervallo_Inizio As Integer
    Public Esecuzione_Ora_Intervallo_Fine As Integer
    Public Frequenza_Nell_intervallo As Integer
    Public Giorno_Settimana_Esecuzione As enum_Giorno_Settimana

    Public GSB_Livello_Log As enum_GSB_Livello_Log


    Public Sub New()

    End Sub

    Public Sub New(ByVal dr As DataRow)

        PivaSuperuser = dr.Item("PivaSuperuser")
        Id_Servizio = dr.Item("Id_Servizio")
        Tipo_Sincro = dr.Item("Tipo_Sincro")
        Id_Riga = dr.Item("Id_Riga")

        'parametri operazione
        Id_Db = CInt(dr.Item("ID_Db"))
        Descrizione = dr.Item("Descrizione")
        Username = dr.Item("Username")
        Password = dr.Item("Password")
        Id_Cod_Cliente = dr.Item("Id_Cod_Cliente")
        Piva_Padre = dr.Item("Piva_Padre")
        Tipo_Operazione = dr.Item("Tipo_Operazione")
        DirectoryLOG = dr.Item("DirectoryLOG")
        DirectoryFileEsportazioni = dr.Item("DirectoryFileEsportazioni")
        DirectoryFileImportazioni = dr.Item("DirectoryFileImportazioni")
        Parametri_Extra = dr.Item("Parametri_Extra")

        'attivazione task
        Attivo = dr.Item("Attivo")

        'stato corrente
        Stato = dr.Item("Stato")

        'parametri periodo esecuzione
        periodoPolling = dr.Item("periodoPolling")
        Esecuzione_Ora_Intervallo_Inizio = dr.Item("Esecuzione_Ora_Intervallo_Inizio")
        Esecuzione_Ora_Intervallo_Fine = dr.Item("Esecuzione_Ora_Intervallo_Fine")
        Frequenza_Nell_intervallo = dr.Item("Frequenza_Nell_intervallo")
        Giorno_Settimana_Esecuzione = dr.Item("Giorno_Settimana_Esecuzione")

        'parametri invio messaggio risultato
        If dr.Table.Columns.Contains("Host_Smtp") AndAlso Not IsDBNull(dr.Item("Host_Smtp")) AndAlso
           dr.Table.Columns.Contains("Mittente") AndAlso Not IsDBNull(dr.Item("Mittente")) AndAlso
           dr.Table.Columns.Contains("Destinatari") AndAlso Not IsDBNull(dr.Item("Destinatari")) Then
            Host_Smtp = dr.Item("Host_Smtp")
            Mittente = dr.Item("Mittente")
            Destinatari = dr.Item("Destinatari")
        End If

        If dr.Table.Columns.Contains("Host_Smtp_Porta") AndAlso Not IsDBNull(dr.Item("Host_Smtp_Porta")) Then
            Host_Smtp_Porta = dr.Item("Host_Smtp_Porta")
        End If

        If dr.Table.Columns.Contains("smtp_user") AndAlso Not IsDBNull(dr.Item("smtp_user")) Then
            smtp_user = dr.Item("smtp_user")
        End If

        If dr.Table.Columns.Contains("smtp_password") AndAlso Not IsDBNull(dr.Item("smtp_password")) Then
            smtp_password = dr.Item("smtp_password")
        End If

        If dr.Table.Columns.Contains("smtp_enablessl") AndAlso Not IsDBNull(dr.Item("smtp_enablessl")) Then
            smtp_enablessl = dr.Item("smtp_enablessl")
        End If

        If dr.Table.Columns.Contains("GSB_enum_Livello_Log") AndAlso Not IsDBNull(dr.Item("GSB_enum_Livello_Log")) Then
            GSB_Livello_Log = dr.Item("GSB_enum_Livello_Log")
        Else
            GSB_Livello_Log = enum_GSB_Livello_Log.Tutto
        End If
    End Sub

End Class

Public Class Configurazione_Servizi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiSingolo(ByVal pivaSuperuser As String,
                                 ByVal idServizio As enum_Id_Servizio,
                                 ByVal tipoSincro As enum_Tipi_Servizi_Background,
                                 ByVal idRiga As Integer,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByRef objParametriServer As AgronicaCoreParametri
                                 ) As Configurazione_Servizio

        Const nomeRoutine = "AgronicaCoreVarieDAL.Configurazione_Servizi.LeggiSingolo()"

        Dim messaggioErrore As String = ""
        Dim objConfigServizio As Configurazione_Servizio
        Try

            Dim dt As DataTable = Leggi(pivaSuperuser,
                                        idServizio,
                                        tipoSincro,
                                        idRiga,
                                        xFiltroAggiuntivo,
                                        "",
                                        objParametriServer)
            If dt.Rows.Count <> 1 Then
                Throw New Exception("dt.Rows.Count = " & dt.Rows.Count & ", pivaSuperuser = " & pivaSuperuser & ", tipoSincro = " & tipoSincro)
            End If

            objConfigServizio = New Configurazione_Servizio(dt.Rows(0))

            Return objConfigServizio

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Return Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Nothing

    End Function

    Public Function Leggi(ByVal pivaSuperuser As String,
                          ByVal idServizio As enum_Id_Servizio,
                          ByVal tipoSincro As enum_Tipi_Servizi_Background,
                          ByVal idRiga As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreVarieDAL.Configurazione_Servizi.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  Configurazione_Servizi ")
            strSql.AppendLine(" WHERE 1=1  ")
            If pivaSuperuser <> "" Then
                strSql.AppendLine(" AND PivaSuperuser ='" & Agro_SQL_SaveText(pivaSuperuser) & "' ")
            End If

            If idServizio <> 0 Then
                strSql.AppendLine(" AND Id_Servizio =" & Agro_SQL_SaveNum(idServizio) & " ")
            End If

            If tipoSincro <> 0 Then
                strSql.AppendLine(" AND Tipo_Sincro =" & Agro_SQL_SaveNum(tipoSincro) & " ")
            End If

            If idRiga <> 0 Then
                strSql.AppendLine(" AND Id_Riga =" & Agro_SQL_SaveNum(idRiga) & " ")
            End If
            '---------------------------------------------

            ' NOn parametrizzato in attesa fix json
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY PivaSuperUser, Id_Servizio, Tipo_Sincro, Id_Riga  ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            Dim sicurezza As New Sicurezza
            Dim configurazioneSiti As New Configurazione_Siti_R
            Dim cr = configurazioneSiti.Leggi_Valore(0, "cr", "", "", objParametri)

            For Each row As DataRow In dt.Rows
                If row("smtp_password_isEncrypted") = True Then
                    row("smtp_password") = sicurezza.DecryptString(row("smtp_password"), cr)
                    row("smtp_password_isEncrypted") = False
                Else
                    If Not String.IsNullOrEmpty(row("smtp_password")) Then
                        Dim encryptedPassword = sicurezza.EncryptString(row("smtp_password"), cr)
                        Dim configurazioneServiziW As New Configurazione_Servizi_W
                        configurazioneServiziW.SetSmtp_Password(row("PivaSuperUser"), row("Id_Servizio"), row("Tipo_Sincro"), row("Id_Riga"), encryptedPassword, 1, objParametri)
                    End If

                End If
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi(ByVal pivaSuperuser As String,
                          ByVal idServizio As enum_Id_Servizio,
                          ByVal tipoSincro As enum_Tipi_Servizi_Background,
                          ByVal idRiga As Integer,
                          ByVal leggiSoloAttivi As Boolean,
                          ByVal stringaConnessioneServer As String
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreVarieDAL.Configurazione_Servizi.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  Configurazione_Servizi ")
            strSql.AppendLine(" WHERE 1=1  ")

            If leggiSoloAttivi Then
                strSql.AppendLine(" AND Attivo <> 0")
            End If
            If pivaSuperuser <> "" Then
                strSql.AppendLine(" AND PivaSuperuser ='" & Agro_SQL_SaveText(pivaSuperuser) & "' ")
            End If

            If idServizio <> 0 Then
                strSql.AppendLine(" AND Id_Servizio =" & Agro_SQL_SaveNum(idServizio) & " ")
            End If

            If tipoSincro <> 0 Then
                strSql.AppendLine(" AND Tipo_Sincro =" & Agro_SQL_SaveNum(tipoSincro) & " ")
            End If

            If idRiga <> 0 Then
                strSql.AppendLine(" AND Id_Riga =" & Agro_SQL_SaveNum(idRiga) & " ")
            End If
            '---------------------------------------------


            strSql.AppendLine(" ORDER BY PivaSuperUser, Id_Servizio, Tipo_Sincro, Id_Riga  ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(stringaConnessioneServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            Dim sicurezza As New Sicurezza
            Dim query As String = "SELECT Valore From Configurazione_Siti WHERE Chiave = 'cr'"

            Dim dtConfigSiti As DataTable = EseguiQuery_Lettura(stringaConnessioneServer, query, nomeRoutine)
            Dim cr As String = dtConfigSiti.Rows(0)("Valore")

            'aggiornamento password per codificare i campi non codificati
            For Each row As DataRow In dt.Rows
                If row("smtp_password_isEncrypted") = True Then
                    row("smtp_password") = sicurezza.DecryptString(row("smtp_password"), cr)
                    row("smtp_password_isEncrypted") = False
                Else
                    If Not String.IsNullOrEmpty(row("smtp_password")) Then
                        Dim encryptedPassword = sicurezza.EncryptString(row("smtp_password"), cr)

                        Dim objParametri As New AgronicaCoreParametri
                        objParametri.StringaConnessione = stringaConnessioneServer
                        Dim configurazioneServiziW As New Configurazione_Servizi_W
                        configurazioneServiziW.SetSmtp_Password(row("PivaSuperUser"), row("Id_Servizio"), row("Tipo_Sincro"), row("Id_Riga"), encryptedPassword, 1, objParametri)

                    End If

                End If
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            'Scrivi_LOG(StringaConnessione_Server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function SetUserNameEPassword(ByVal pivaSuperuser As String,
                             ByVal idServizio As enum_Id_Servizio,
                             ByVal tipoSincro As enum_Tipi_Servizi_Background,
                             ByVal idRiga As Integer,
                             ByVal userName As String,
                             ByVal password As String,
                             ByVal objParametriServer As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "Configurazione_Servizi_R.SetStato()"

        If pivaSuperuser = "" Then
            Throw New Exception("PivaSuperuser non valorizzata")
        End If

        If idServizio = 0 Then
            Throw New Exception("Id_Servizio non valorizzato")
        End If

        If tipoSincro = 0 Then
            Throw New Exception("Tipo_Sincro non valorizzato")
        End If

        If idRiga = 0 Then
            Throw New Exception("Id_Riga non valorizzata")
        End If


        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" Update Configurazione_Servizi ")
            strSql.AppendLine(" SET userName = '" & Agro_SQL_SaveText(userName) & "', ")
            strSql.AppendLine(" password = '" & Agro_SQL_SaveText(password) & "' ")
            strSql.AppendLine(" WHERE   ")
            strSql.AppendLine("    PivaSuperuser ='" & Agro_SQL_SaveText(pivaSuperuser) & "' ")
            strSql.AppendLine(" AND Id_Servizio =" & Agro_SQL_SaveNum(idServizio) & " ")
            strSql.AppendLine(" AND Tipo_Sincro =" & Agro_SQL_SaveNum(tipoSincro) & " ")
            strSql.AppendLine(" AND Id_Riga =" & Agro_SQL_SaveNum(idRiga) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function SetStato(ByVal pivaSuperuser As String,
                             ByVal idServizio As enum_Id_Servizio,
                             ByVal tipoSincro As enum_Tipi_Servizi_Background,
                             ByVal idRiga As Integer,
                             ByVal stato As Integer,
                             ByVal objParametriServer As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "Configurazione_Servizi_R.SetStato()"

        If pivaSuperuser = "" Then
            Throw New Exception("PivaSuperuser non valorizzata")
        End If

        If idServizio = 0 Then
            Throw New Exception("Id_Servizio non valorizzato")
        End If

        If tipoSincro = 0 Then
            Throw New Exception("Tipo_Sincro non valorizzato")
        End If

        If idRiga = 0 Then
            Throw New Exception("Id_Riga non valorizzata")
        End If


        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" Update Configurazione_Servizi ")
            strSql.AppendLine(" SET stato = " & Agro_SQL_SaveNum(stato) & " ")
            strSql.AppendLine(" WHERE   ")
            strSql.AppendLine("    PivaSuperuser ='" & Agro_SQL_SaveText(PivaSuperuser) & "' ")
            strSql.AppendLine(" AND Id_Servizio =" & Agro_SQL_SaveNum(idServizio) & " ")
            strSql.AppendLine(" AND Tipo_Sincro =" & Agro_SQL_SaveNum(tipoSincro) & " ")
            strSql.AppendLine(" AND Id_Riga =" & Agro_SQL_SaveNum(idRiga) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function AbilitaDisabilita(ByVal pivaSuperuser As String,
                                      ByVal idServizio As enum_Id_Servizio,
                                      ByVal tipoSincro As enum_Tipi_Servizi_Background,
                                      ByVal idRiga As Integer,
                                      ByVal AbilitaTrue_DisabilitaFalse As Boolean,
                                      ByVal objParametriServer As AgronicaCoreParametri
                                      ) As Boolean

        Const nomeRoutine = "Configurazione_Servizi_R.AbilitaDisabilita()"

        If PivaSuperuser = "" Then
            Throw New Exception("PivaSuperuser non valorizzata")
        End If

        If idServizio = 0 Then
            Throw New Exception("Id_Servizio non valorizzato")
        End If

        If tipoSincro = 0 Then
            Throw New Exception("Tipo_Sincro non valorizzato")
        End If

        If idRiga = 0 Then
            Throw New Exception("Id_Riga non valorizzata")
        End If


        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" Update Configurazione_Servizi ")
            If AbilitaTrue_DisabilitaFalse Then
                strSql.AppendLine(" SET Attivo = " & Agro_SQL_SaveNum(1) & " ")
            Else
                strSql.AppendLine(" SET Attivo = " & Agro_SQL_SaveNum(0) & " ")
            End If
            strSql.AppendLine(" WHERE   ")
            strSql.AppendLine("    PivaSuperuser ='" & Agro_SQL_SaveText(PivaSuperuser) & "' ")
            strSql.AppendLine(" AND Id_Servizio =" & Agro_SQL_SaveNum(idServizio) & " ")
            strSql.AppendLine(" AND Tipo_Sincro =" & Agro_SQL_SaveNum(tipoSincro) & " ")
            strSql.AppendLine(" AND Id_Riga =" & Agro_SQL_SaveNum(idRiga) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function GetStato(ByVal pivaSuperuser As String,
                             ByVal idServizio As enum_Id_Servizio,
                             ByVal tipoSincro As enum_Tipi_Servizi_Background,
                             ByVal idRiga As Integer,
                             ByVal objParametriServer As AgronicaCoreParametri
                             ) As Integer

        Const nomeRoutine = "Configurazione_Servizi_R.GetStato()"

        If PivaSuperuser = "" Then
            Throw New Exception("PivaSuperuser non valorizzata")
        End If

        If idServizio = 0 Then
            Throw New Exception("Id_Servizio non valorizzato")
        End If

        If tipoSincro = 0 Then
            Throw New Exception("Tipo_Sincro non valorizzato")
        End If

        If idRiga = 0 Then
            Throw New Exception("Id_Riga non valorizzata")
        End If


        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" Select stato from Configurazione_Servizi ")
            strSql.AppendLine(" WHERE   ")
            strSql.AppendLine("    PivaSuperuser ='" & Agro_SQL_SaveText(PivaSuperuser) & "' ")
            strSql.AppendLine(" AND Id_Servizio =" & Agro_SQL_SaveNum(idServizio) & " ")
            strSql.AppendLine(" AND Tipo_Sincro =" & Agro_SQL_SaveNum(tipoSincro) & " ")
            strSql.AppendLine(" AND Id_Riga =" & Agro_SQL_SaveNum(idRiga) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count = 1 Then
                Return dt.Rows(0).Item("Stato")
            Else
                Throw New Exception("dt.Rows.Count <> 1")
            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return Nothing

    End Function

End Class

Public Class Configurazione_Servizi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function CodificaPasswordSmtpNonCodificati(ByVal stringaConnessioneServer As String) As Boolean

        Dim nomeRoutine As String = NameOf(CodificaPasswordSmtpNonCodificati)
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim risposta As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" SELECT     PivaSuperUser, Id_Servizio, Tipo_Sincro, Id_Riga, smtp_password, smtp_password_isEncrypted ")
            strSql.AppendLine(" FROM       Configurazione_Servizi")
            strSql.AppendLine(" WHERE      smtp_password_isEncrypted = 0")
            strSql.AppendLine(" AND        smtp_password <> ''")

            Dim dt As DataTable = EseguiQuery_Lettura(stringaConnessioneServer, strSql.ToString, nomeRoutine)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                Dim query As String = "SELECT Valore From Configurazione_Siti WHERE Chiave = 'cr'"

                Dim dtConfigSiti As DataTable = EseguiQuery_Lettura(stringaConnessioneServer, query, nomeRoutine)
                If dtConfigSiti Is Nothing OrElse dtConfigSiti.Rows.Count = 0 Then
                    Throw New Exception("Impossibile recuperare il valore 'cr' da Configurazione_Siti")
                End If

                Dim objParametri As New AgronicaCoreParametri
                objParametri.StringaConnessione = stringaConnessioneServer

                Dim sicurezza As New Sicurezza
                Dim cr As String = dtConfigSiti.Rows(0)("Valore")

                For Each row As DataRow In dt.Rows
                    Dim encryptedPassword = sicurezza.EncryptString(row("smtp_password"), cr)
                    SetSmtp_Password(row("PivaSuperUser"), row("Id_Servizio"), row("Tipo_Sincro"), row("Id_Riga"), encryptedPassword, 1, objParametri)
                Next
            End If

            risposta = True
        Catch ex As Exception
            messaggioErrore = ex.Message
            'Scrivi_LOG(StringaConnessione_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function

    Public Function SetSmtp_Password(ByVal pivaSuperuser As String,
                             ByVal idServizio As enum_Id_Servizio,
                             ByVal tipoSincro As enum_Tipi_Servizi_Background,
                             ByVal idRiga As Integer,
                             ByVal smtp_password As String,
                             ByVal smtp_password_isEncrypted As Int16,
                             ByVal objParametriServer As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine = NameOf(SetSmtp_Password)

        If pivaSuperuser = "" Then
            Throw New Exception("PivaSuperuser non valorizzata")
        End If

        If idServizio = 0 Then
            Throw New Exception("Id_Servizio non valorizzato")
        End If

        If tipoSincro = 0 Then
            Throw New Exception("Tipo_Sincro non valorizzato")
        End If

        If idRiga = 0 Then
            Throw New Exception("Id_Riga non valorizzata")
        End If

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim risposta As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" Update Configurazione_Servizi ")
            strSql.AppendLine(" SET smtp_password = '" & Agro_SQL_SaveText(smtp_password) & "', ")
            strSql.AppendLine("     smtp_password_isEncrypted = " & Agro_SQL_SaveNum(smtp_password_isEncrypted) & " ")
            strSql.AppendLine(" WHERE   ")
            strSql.AppendLine("    PivaSuperuser ='" & Agro_SQL_SaveText(pivaSuperuser) & "' ")
            strSql.AppendLine(" AND Id_Servizio =" & Agro_SQL_SaveNum(idServizio) & " ")
            strSql.AppendLine(" AND Tipo_Sincro =" & Agro_SQL_SaveNum(tipoSincro) & " ")
            strSql.AppendLine(" AND Id_Riga =" & Agro_SQL_SaveNum(idRiga) & " ")

            '--------------------------------------------------------------------------
            risposta = EseguiQuery_Scrittura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            risposta = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return risposta

    End Function

End Class
