Imports AgronicaCoreAuditDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class AuditProfilazione

    Public Function LeggiIntervisteJson(Audit_Tipo As Integer, Regolamento_Cod As Integer, Intervista_Cod As Integer, Piva As String, ByVal Data_Inizio As Date, ByVal Data_Fine As Date, ByVal objParametri As AgronicaCoreParametri) As String

        'Dim dp As New DataProvider
        'Dim dp As DataProvider = Nothing
        'SeScriviLogLeggiIntervisteJson(dp, objParametri, "TipoAudit : " + Audit_Tipo.ToString())

        Dim auditAgronica As New AuditAgronicaWS(objParametri)
        Dim regolamenti As List(Of AuditRegolamentiModel) = auditAgronica.LeggiRegolamenti(Audit_Tipo)
        Dim ddRegolamenti = regolamenti.ToDictionary(Function(x) x.Regolamento_Cod, Function(x) x.Regolamento_Des)
        Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}

        Dim auditLeggi As New Audit_Interviste_R
        Dim auditRisposteLeggi As New Audit_Risposte_Interviste_R
        'SeScriviLogLeggiIntervisteJson(dp, objParametri, "LeggiInterviste_B")
        Dim dt As DataTable = auditLeggi.LeggiInterviste(Audit_Tipo, Regolamento_Cod, Intervista_Cod, Piva, Data_Inizio, Data_Fine, "", "", objParametri)
        'SeScriviLogLeggiIntervisteJson(dp, objParametri, "LeggiInterviste_E : " + dt.Rows.Count().ToString())

        'SeScriviLogLeggiIntervisteJson(dp, objParametri, "LeggiDomandeInterviste_B")
        Dim domande As List(Of AuditDomandeIntervisteModel) = auditAgronica.LeggiDomandeInterviste(Audit_Tipo, Regolamento_Cod)
        'SeScriviLogLeggiIntervisteJson(dp, objParametri, "LeggiDomandeInterviste_E : " + domande.Count().ToString())

        Dim jArrayProfilazione As New JArray

        dt.Columns.Add("Piva_Url", Type.GetType("System.String"))
        dt.Columns.Add("Regolamento_Des", Type.GetType("System.String"))
        dt.Columns.Add("Risposte", Type.GetType("System.String"))

        Dim listaInterviste As New List(Of Integer)

        'SeScriviLogLeggiIntervisteJson(dp, objParametri, "ForEach_B")

        For Each row In dt.Rows

            ' fix per righe ripetute (19599)
            If Not listaInterviste.Contains(row.Item("Intervista_Cod")) Then
                listaInterviste.Add(row.Item("Intervista_Cod"))
            Else
                Continue For
            End If

            Dim objProfilazione As New JObject(
                New JProperty("Audit_Tipo", row.Item("Audit_Tipo")),
                New JProperty("Regolamento_Cod", row.Item("Regolamento_Cod")),
                New JProperty("Regolamento_Des", ddRegolamenti(row.Item("Regolamento_Cod"))),
                New JProperty("Intervista_Cod", row.Item("Intervista_Cod")),
                New JProperty("Intervista_Nome", row.Item("Intervista_Nome")),
                New JProperty("Intervista_SuperUser", row.Item("Intervista_SuperUser")),
                New JProperty("Piva", row.Item("Piva")),
                New JProperty("Piva_Url", Stringa_Codifica(row.Item("Piva"), AgroKey_EncoderDecoder, objParametri)),
                New JProperty("Rag_Soc", row.Item("Rag_Soc")),
                New JProperty("Validita_Inizio", AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(row.Item("Validita_Inizio"), DateTimeKind.Local)),
                New JProperty("Validita_Fine", AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(row.Item("Validita_Fine"), DateTimeKind.Local))
            )

            If Audit_Tipo <> 4 Then
                objProfilazione.Add(New JProperty("CUAA", row.Item("CUAA")))
                objProfilazione.Add(New JProperty("Piva_OP", row.Item("Piva_OP")))
                objProfilazione.Add(New JProperty("Rag_Soc_OP", row.Item("Rag_Soc_OP")))
                Dim risposte = LeggiRisposteInterviste(Audit_Tipo, Regolamento_Cod, row.Item("Intervista_Cod"), row.Item("Piva"), objParametri, domande)
                For Each r In risposte
                    objProfilazione.Add("Domanda_" & r.Domanda_Cod, r.Valore)
                Next
            End If

            jArrayProfilazione.Add(objProfilazione)

        Next

        'SeScriviLogLeggiIntervisteJson(dp, objParametri, "ForEach_E")

        Return JsonConvert.SerializeObject(jArrayProfilazione, Formatting.None, serializerSettings)

    End Function

    Private Sub SeScriviLogLeggiIntervisteJson(ByRef dp As DataProvider,
                                               ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByVal messaggio As String)
        If Not IsNothing(dp) Then
            dp.Scrivi_LOG(objParametri_Server, "LeggiIntervisteJson", messaggio)
        End If

    End Sub

    Public Function LeggiInterviste(Audit_Tipo As Integer, Regolamento_Cod As Integer, Intervista_Cod As Integer, Piva As String, ByVal Data_Inizio As Date, ByVal Data_Fine As Date, ByVal Id_Area As Integer, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditIntervisteModel)

        Dim auditAgronica As New AuditAgronicaWS(objParametri)
        Dim regolamenti As List(Of AuditRegolamentiModel) = auditAgronica.LeggiRegolamenti(Audit_Tipo)
        Dim ddRegolamenti = regolamenti.ToDictionary(Function(x) x.Regolamento_Cod, Function(x) x.Regolamento_Des)

        Dim auditLeggi As New Audit_Interviste_R
        Dim dt As DataTable = auditLeggi.LeggiInterviste(Audit_Tipo, Regolamento_Cod, Intervista_Cod, Piva, Data_Inizio, Data_Fine, "", "", objParametri)

        Dim rval As New List(Of AuditIntervisteModel)

        For i = 0 To dt.Rows.Count - 1

            Dim item As New AuditIntervisteModel With {
                .Audit_Tipo = dt.Rows(i).Item("Audit_tipo"),
                .Regolamento_Cod = dt.Rows(i).Item("Regolamento_Cod"),
                .Regolamento_Des = ddRegolamenti(dt.Rows(i).Item("Regolamento_Cod")),
                .Intervista_Cod = dt.Rows(i).Item("Intervista_Cod"),
                .Intervista_Nome = If(IsDBNull(dt.Rows(i).Item("Intervista_Nome")), "", dt.Rows(i).Item("Intervista_Nome")),
                .Intervista_SuperUser = dt.Rows(i).Item("Intervista_SuperUser"),
                .Piva = dt.Rows(i).Item("Piva"),
                .PivaReale = dt.Rows(i).Item("PivaReale"),
                .Piva_Url = Stringa_Codifica(dt.Rows(i).Item("Piva"), AgroKey_EncoderDecoder, objParametri),
                .Rag_Soc = dt.Rows(i).Item("Rag_Soc"),
                .Validita_Inizio = AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(dt.Rows(i).Item("Validita_Inizio"), DateTimeKind.Local),
                .Validita_Fine = AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(dt.Rows(i).Item("Validita_Fine"), DateTimeKind.Local)
            }

            rval.Add(item)

        Next

        'Dim rval As List(Of AuditIntervisteModel) =
        '    (From dd In dt.AsEnumerable
        '     Select New AuditIntervisteModel With {
        '        .Audit_Tipo = dd("Audit_tipo"),
        '        .Regolamento_Cod = dd("Regolamento_Cod"),
        '        .Intervista_Cod = dd("Intervista_Cod"),
        '        .Intervista_SuperUser = dd("Intervista_SuperUser"),
        '        .Piva = dd("Piva"),
        '        .Rag_Soc = dd("Rag_Soc"),
        '        .Validita_Inizio = AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(dd("Validita_Inizio"), DateTimeKind.Local),
        '        .Validita_Fine = AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(dd("Validita_Fine"), DateTimeKind.Local)
        '    }).ToList       


        Return rval

    End Function

    Public Function LeggiRisposteInterviste(Audit_Tipo As Integer, Regolamento_Cod As Integer, Intervista_Cod As Integer, ByVal Piva As String,
                                            ByVal objParametri As AgronicaCoreParametri, Optional ByRef domande As List(Of AuditDomandeIntervisteModel) = Nothing) As List(Of AuditRisposteIntervisteModel)

        If domande Is Nothing Then
            Dim auditController As New AuditAgronicaWS(objParametri)
            domande = auditController.LeggiDomandeInterviste(Audit_Tipo, Regolamento_Cod)
        End If

        Dim risposte As New Dictionary(Of Integer, String)
        If Intervista_Cod <> 0 Then
            Dim auditLeggi As New Audit_Risposte_Interviste_R
            Dim dt As DataTable = auditLeggi.LeggiRisposte(Audit_Tipo, Regolamento_Cod, Intervista_Cod, 0, "", "", objParametri)
            For i = 0 To dt.Rows.Count - 1
                risposte.Add(dt.Rows(i).Item("Domanda_Cod"), dt.Rows(i).Item("Valore"))
            Next
        End If

        Dim AuditProfilazione As New AuditAttivazione(objParametri)
        Dim vetParametri(1) As String
        vetParametri(0) = objParametri.PivaSuperUser
        vetParametri(1) = Piva

        Dim rval As New List(Of AuditRisposteIntervisteModel)
        For Each domanda In domande

            Dim bRet As Boolean
            Dim sValore As String = ""

            If Intervista_Cod <> 0 Then
                If risposte.ContainsKey(domanda.Domanda_Cod) Then
                    sValore = risposte(domanda.Domanda_Cod)
                End If
            ElseIf Piva <> "" AndAlso Not IsDBNull(domanda.Attivazione) AndAlso CStr(domanda.Attivazione) <> "" Then
                bRet = CallByName(AuditProfilazione, CStr(domanda.Attivazione), CallType.Method, vetParametri)
                sValore = IIf(bRet, "1", "0")
            End If

            Dim item As New AuditRisposteIntervisteModel With {
                .Audit_Tipo = domanda.Audit_Tipo,
                .Regolamento_Cod = domanda.Regolamento_Cod,
                .Domanda_Cod = domanda.Domanda_Cod,
                .Domanda_Des = domanda.Domanda_Des,
                .Attivazione = domanda.Attivazione,
                .Tipo = domanda.Tipo,
                .Ordine = domanda.Ordine,
                .Intervista_Cod = Intervista_Cod,
                .Valore = sValore
            }
            rval.Add(item)

        Next

        Return rval

    End Function

    Public Function ScriviInterviste(Audit_Tipo As Integer, Regolamento_Cod As Integer, Intervista_Cod As Integer, Intervista_Nome As String, Piva As String, ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date, ByVal Risposte As String, ByVal objParametri As AgronicaCoreParametri) As Boolean

        Dim auditLeggi As New Audit_Interviste_R
        Dim auditScrivi As New Audit_Interviste_W

        If Validita_Inizio > Validita_Fine Then
            Return False
        End If

        Dim filtro_aggiuntivo As String = ""
        If Intervista_Cod <> 0 Then
            filtro_aggiuntivo = "Audit_Interviste.Intervista_Cod<>" & Intervista_Cod
        End If

        Dim dt As DataTable = auditLeggi.LeggiInterviste(Audit_Tipo, Regolamento_Cod, Intervista_Cod, Piva, Validita_Inizio, Validita_Fine, filtro_aggiuntivo, "", objParametri)
        If dt.Rows.Count > 0 Then
            Return False
        End If

        Dim azione As String = "UPD"
        Dim errore As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        If Intervista_Cod = 0 Then
            'Intervista_Cod = AgroSequenze.Agronica_SequenzaTabelle_NuovoID("Audit_Interviste", objParametri)
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            Intervista_Cod = AgroSequenze.NuovoId_Tabella("Audit_Interviste", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
            azione = "INS"
        End If

        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            If azione = "UPD" Then
                auditScrivi.Modifica(Audit_Tipo, Regolamento_Cod, Intervista_Cod, Piva, Validita_Inizio, Validita_Fine, objParametri, Intervista_Nome:=Intervista_Nome)
            Else
                auditScrivi.Scrivi(Audit_Tipo, Regolamento_Cod, Intervista_Cod, Piva, Validita_Inizio, Validita_Fine, objParametri, Intervista_Nome:=Intervista_Nome)
            End If

            If Risposte <> "" Then
                Dim auditAgronica As New AuditAgronicaWS(objParametri)
                Dim auditRisposteIntervisteW As New Audit_Risposte_Interviste_W
                Dim domande As List(Of AuditDomandeIntervisteModel) = auditAgronica.LeggiDomandeInterviste(Audit_Tipo, Regolamento_Cod)
                Dim campi As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(Risposte)
                For Each domanda In domande
                    Dim valore As String = "0"
                    Dim codice As Integer = domanda.Domanda_Cod
                    If campi.Contains("domanda" & codice) Then
                        valore = "1"
                    End If
                    If azione = "UPD" Then
                        auditRisposteIntervisteW.ModificaRisposte(Audit_Tipo, Regolamento_Cod, Intervista_Cod, Piva, codice, valore, Validita_Inizio, Validita_Fine, objParametri)
                    Else
                        auditRisposteIntervisteW.ScriviRisposte(Audit_Tipo, Regolamento_Cod, Intervista_Cod, Piva, codice, valore, Validita_Inizio, Validita_Fine, objParametri)
                    End If
                Next
            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            errore = True

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Not errore

    End Function

    Public Function CancellaInterviste(Audit_Tipo As Integer, Regolamento_Cod As Integer, Intervista_Cod As Integer, Piva As String, ByVal objParametri As AgronicaCoreParametri) As Boolean

        Dim errore As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim auditScrivi As New Audit_Interviste_W
        Dim auditRisposteScrivi As New Audit_Risposte_Interviste_W

        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            auditRisposteScrivi.CancellaRisposte(Audit_Tipo, Regolamento_Cod, Intervista_Cod, 0, objParametri)
            auditScrivi.Cancella(Audit_Tipo, Regolamento_Cod, Intervista_Cod, Piva, objParametri)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            errore = True

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Not errore

    End Function

    Public Function ScriviProfilo(Audit_Tipo As Integer, Regolamento_Cod As Integer, Intervista_Cod As Integer, Piva As String, ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date, ByVal Risposte As String, ByVal objParametri As AgronicaCoreParametri) As Boolean

        Dim auditLeggi As New Audit_Interviste_R
        Dim auditScrivi As New Audit_Interviste_W
        Dim auditRisposteIntervisteW As New Audit_Risposte_Interviste_W

        If Validita_Inizio > Validita_Fine Then
            Return False
        End If

        Dim filtro_aggiuntivo As String = ""
        If Intervista_Cod <> 0 Then
            filtro_aggiuntivo = "Audit_Interviste.Intervista_Cod<>" & Intervista_Cod
        End If

        Dim dt As DataTable = auditLeggi.LeggiInterviste(Audit_Tipo, Regolamento_Cod, Intervista_Cod, Piva, Validita_Inizio, Validita_Fine, filtro_aggiuntivo, "", objParametri)
        If dt.Rows.Count > 0 Then
            Return False
        End If

        Dim azione As String = "UPD"
        Dim errore As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim AgroSequenze As New Agro_Sequenze
        If Intervista_Cod = 0 Then
            'Intervista_Cod = AgroSequenze.Agronica_SequenzaTabelle_NuovoID("Audit_Interviste", objParametri)
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            Intervista_Cod = AgroSequenze.NuovoId_Tabella("Audit_Interviste", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
            azione = "INS"
        End If

        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            If azione = "UPD" Then
                auditRisposteIntervisteW.CancellaRisposte(Audit_Tipo, Regolamento_Cod, Intervista_Cod, 0, objParametri)
                auditScrivi.Modifica(Audit_Tipo, Regolamento_Cod, Intervista_Cod, Piva, Validita_Inizio, Validita_Fine, objParametri)
            Else
                auditScrivi.Scrivi(Audit_Tipo, Regolamento_Cod, Intervista_Cod, Piva, Validita_Inizio, Validita_Fine, objParametri)
            End If

            If Risposte <> "" Then
                'Dim auditAgronica As New AuditAgronicaWS(objParametri)
                'Dim domande As List(Of AuditDomandeIntervisteModel) = auditAgronica.LeggiDomandeInterviste(Audit_Tipo, Regolamento_Cod)
                Dim campi As List(Of AuditFormModel) = JsonConvert.DeserializeObject(Of List(Of AuditFormModel))(Risposte)
                For Each campo In campi
                    Dim Codice As String = campo.name
                    Dim Valore As String = campo.value
                    Dim Codici() As String = Split(Codice, "_")
                    If Codici(0) = "domanda" Then
                        auditRisposteIntervisteW.ScriviRisposte(Audit_Tipo, Regolamento_Cod, Intervista_Cod, Piva, Codici(1), Valore, Validita_Inizio, Validita_Fine, objParametri)
                    End If
                Next
            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            errore = True

        Finally

            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return Not errore

    End Function

End Class
