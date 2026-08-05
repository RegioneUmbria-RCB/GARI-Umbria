Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json

Public Class AuditCalcoloLivelli

    Public Risposte As String
    Public Disposizione As Integer
    Dim objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Sub New(ByVal _Disposizione As Integer, ByVal _Risposte As String, ByVal _objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Disposizione = _Disposizione
        Risposte = _Risposte
        objParametri = _objParametri
    End Sub

    Public Function LeggiRisposte(ByVal Disp_Cod As Integer, ByVal Punto_Numero As String, ByVal Valore As String) As DataTable
        Dim campi As List(Of AuditFormModel) = JsonConvert.DeserializeObject(Of List(Of AuditFormModel))(Risposte)

        Dim dtRisposte As DataTable = New DataTable("Risposte")
        dtRisposte.Columns.Add("Disp_Cod", GetType(System.Int32))
        dtRisposte.Columns.Add("Punto_Numero", GetType(System.String))
        dtRisposte.Columns.Add("Valore", GetType(System.String))
        dtRisposte.Columns.Add("Valore_2", GetType(System.String))
        dtRisposte.Columns.Add("Sezione_Cod", GetType(System.Int32))

        Dim risposte2 As New Dictionary(Of String, String)
        For Each campo In campi
            Dim name As String = campo.name
            Dim value As String = campo.value
            Dim names() As String = Split(name, "_")
            If names(0) = "risposta2" AndAlso value <> "" Then
                risposte2.Add(names(1) & "_" & names(2), value)
            End If
        Next

        For Each campo In campi
            Dim name As String = campo.name
            Dim value As String = campo.value
            Dim names() As String = Split(name, "_")
            If names(0) = "risposta" Then
                Dim disposizione As Integer = CInt(names(1))
                Dim punto As String = CStr(names(2))
                If Disp_Cod = 0 OrElse disposizione = Disp_Cod Then
                    If Punto_Numero = "" OrElse punto = Punto_Numero Then
                        If value <> "-1" AndAlso (Valore = "" Or value = Valore) Then
                            Dim value2 As String = ""
                            Dim key = names(1) & "_" & names(2)
                            If risposte2.ContainsKey(key) Then
                                value2 = risposte2(key)
                            End If
                            dtRisposte.Rows.Add(disposizione, punto, value, value2)
                        End If
                    End If
                End If
            End If
        Next

        Return dtRisposte
    End Function

    Public Function CalcolaPunteggi(ByVal Audit_Tipo As Integer,
                                    ByVal Regolamento_Cod As Integer,
                                    ByVal Disp_Cod As Integer,
                                    ByVal Piva As String,
                                    ByVal Data As Date,
                                    ByRef codici As List(Of AuditCodiciModel),
                                    ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of AuditFormModel)

        Dim punteggi As New List(Of AuditFormModel)

        Dim portata As Integer = 0
        Dim gravita As Integer = 0
        Dim durata As Integer = 0
        Dim portata_gravita_durata As Integer = 0

        Dim vetParametri(2) As String
        vetParametri(0) = objParametri.PivaSuperUser
        vetParametri(1) = Piva
        vetParametri(2) = Regolamento_Cod

        ' controllo esito verifica e calcolo livello
        Dim verifica As DataTable = LeggiRisposte(Disp_Cod, "", "0")

        If verifica.Rows.Count > 0 Then
            Try
                Select Case Disp_Cod
                    Case 30, 31
                        portata_gravita_durata = CallByName(Me, "Portata_Gravita_Durata_" & Disp_Cod, CallType.Method, vetParametri)
                    Case Else
                        portata = CallByName(Me, "Portata_" & Disp_Cod, CallType.Method, vetParametri)
                        gravita = CallByName(Me, "Gravita_" & Disp_Cod, CallType.Method, vetParametri)
                        durata = CallByName(Me, "Durata_" & Disp_Cod, CallType.Method, vetParametri)
                End Select
            Catch ex As Exception
            End Try
        End If

        ' restituisce la lista campi punteggi da aggiornare
        'Dim auditAgronica As New AuditAgronicaWS(objParametri)
        'Dim codici As List(Of AuditCodiciModel) = AuditAgronica.LeggiCodici(Audit_Tipo, Regolamento_Cod, Disp_Cod, 0, Data)
        For Each codice In codici
            If codice.Disp_Cod = Disp_Cod AndAlso codice.Punteggio > 0 Then
                Select Case codice.Sezione_Cod
                    Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Portata
                        punteggi.Add(New AuditFormModel With {.name = codice.Punto_Numero, .value = IIf(portata = codice.Punteggio, "1", "0")})
                    Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Gravita
                        punteggi.Add(New AuditFormModel With {.name = codice.Punto_Numero, .value = IIf(gravita = codice.Punteggio, "1", "0")})
                    Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_Durata
                        punteggi.Add(New AuditFormModel With {.name = codice.Punto_Numero, .value = IIf(durata = codice.Punteggio, "1", "0")})
                    Case TipiEnumerativi.enum_AuditSezioni.AuditSezioni_PortataGravitaDurata
                        punteggi.Add(New AuditFormModel With {.name = codice.Punto_Numero, .value = IIf(portata_gravita_durata = codice.Punteggio, "1", "0")})
                End Select
            End If
        Next

        Return punteggi

    End Function

    ' CGO 1 (ACQUE) - Portata
    Public Function Portata_4(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        ' Parametri calcolo
        Dim Livello As Integer = 0
        Dim infrazioneB3a = False
        Dim infrazioneB3b = False
        Dim infrazioneB4a = False
        Dim infrazioneB4b = False
        Dim infrazioneC5a = False
        Dim infrazioneC5b = False
        Dim infrazioneD = False
        Dim infrazioneCumuli = False
        Dim derogaC5a = False
        Dim superficieInfrazione = False
        Dim effettiExtraAziendali = False

        Dim DT As DataTable = LeggiRisposte(4, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0060" Then
                    infrazioneB3a = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0070" Then
                    infrazioneB3b = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0090" Then
                    infrazioneB4a = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0100" Then
                    infrazioneB4b = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0120" Then
                    infrazioneC5a = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0121" Then
                    infrazioneC5b = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0150" Or
                    DT.Rows(i).Item("Punto_Numero") = "0160" Or
                    DT.Rows(i).Item("Punto_Numero") = "0170" Or
                    DT.Rows(i).Item("Punto_Numero") = "0180" Or
                    DT.Rows(i).Item("Punto_Numero") = "0190" Or
                    DT.Rows(i).Item("Punto_Numero") = "0200" Or
                    DT.Rows(i).Item("Punto_Numero") = "0210" Or
                    DT.Rows(i).Item("Punto_Numero") = "0230" Or
                    DT.Rows(i).Item("Punto_Numero") = "0240" Or
                    DT.Rows(i).Item("Punto_Numero") = "0241" Then
                    infrazioneD = True
                    If DT.Rows(i).Item("Punto_Numero") = "0241" Then
                        infrazioneCumuli = True
                    End If

                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0242" Then
                    superficieInfrazione = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0243" Then
                    effettiExtraAziendali = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0125" Then
                    derogaC5a = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        Dim numInfrazioniBasse = 0
        Dim numInfrazioniMedie = 0
        Dim numInfrazioniAlte = 0

        If derogaC5a Then
            infrazioneC5a = False
        End If
        If infrazioneD Then
            If superficieInfrazione Then
                numInfrazioniMedie += 1
            Else
                numInfrazioniBasse += 1
            End If
        End If
        If infrazioneCumuli Then
            numInfrazioniMedie += 1
        End If
        If infrazioneB3b Or infrazioneB4a Or infrazioneB4b Then
            numInfrazioniMedie += 1
        End If

        If numInfrazioniMedie > 1 Or infrazioneB3a Or infrazioneC5a Or infrazioneC5b Or effettiExtraAziendali Then
            Livello = 5
        ElseIf numInfrazioniMedie > 0 Then
            Livello = 3
        ElseIf numInfrazioniBasse > 0 Then
            Livello = 1
        End If

        Return Livello

    End Function

    ' CGO 1 (ACQUE) - Gravita
    Public Function Gravita_4(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Portata As Integer = Portata_4(SuperPiva, Piva, Regolamento_Cod)

        ' Parametri calcolo
        Dim Livello As Integer = 0
        Dim Classe As Integer = 0
        Dim DT As DataTable = LeggiRisposte(4, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0290" Then
                    Classe = 1
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0300" Then
                    Classe = 2
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0310" Then
                    Classe = 3
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0320" Then
                    Classe = 4
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0330" Then
                    Classe = 5
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        If Portata = 5 And (Classe = 3 Or Classe = 4) Then
            Livello = 5
        ElseIf Portata = 3 And Classe = 4 Then
            Livello = 5
        ElseIf Classe = 5 Then
            Livello = 5
        ElseIf Portata = 5 And (Classe = 1 Or Classe = 2) Then
            Livello = 3
        ElseIf Portata = 3 And (Classe = 2 Or Classe = 3) Then
            Livello = 3
        ElseIf Portata = 1 And (Classe = 3 Or Classe = 4) Then
            Livello = 3
        ElseIf Portata = 1 And (Classe = 1 Or Classe = 2) Then
            Livello = 1
        ElseIf Portata = 3 And Classe = 1 Then
            Livello = 1
        End If

        Return Livello

    End Function

    ' CGO 1 (ACQUE) - Durata
    Public Function Durata_4(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        ' Parametri calcolo
        Dim Livello As Integer = 3

        ' Verifico se è selezionato il campo Effetti Extra Aziendali
        Dim DT As DataTable = LeggiRisposte(4, "0243", "1")
        If DT.Rows.Count > 0 Then
            Livello = 5
        End If

        DT.Dispose()
        DT = Nothing

        Return Livello

    End Function

    ' BCAA 1 (ACQUE) - Portata
    Public Function Portata_29(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneInerbita As Boolean = False
        Dim InfrazioneTampone As Boolean = False
        Dim Tampone100 As Boolean = False
        Dim Tampone200 As Boolean = False
        Dim Inerbita100 As Boolean = False
        Dim Inerbita200 As Boolean = False
        Dim ExtraAzienda As Boolean = False
        Dim DerogaTotale As Boolean = False
        Dim DerogaParziale As Boolean = False

        Dim DT As DataTable = LeggiRisposte(29, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0050" Or
                    DT.Rows(i).Item("Punto_Numero") = "0060" Then

                    InfrazioneTampone = True

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0070" Or
                    DT.Rows(i).Item("Punto_Numero") = "0080" Then

                    InfrazioneInerbita = True

                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0062" Then
                    Tampone100 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0063" Then
                    Tampone200 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0085" Then
                    Inerbita100 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0100" Then
                    ExtraAzienda = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0270" Then
                    DerogaTotale = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0271" Then
                    DerogaParziale = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' applico deroga
        If DerogaParziale Then
            InfrazioneInerbita = False
        ElseIf DerogaTotale Then
            InfrazioneInerbita = False
            InfrazioneTampone = False
        End If

        ' calcolo punteggio
        If InfrazioneInerbita Or InfrazioneTampone Then
            If InfrazioneInerbita Or (InfrazioneTampone And Tampone200) Then
                Livello = 5
            ElseIf (InfrazioneInerbita And Inerbita100) Or (InfrazioneTampone And Tampone100) Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' BCAA 1 (ACQUE) - Gravita
    Public Function Gravita_29(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione11 As Boolean = False
        Dim Infrazione12 As Boolean = False
        Dim Infrazione13 As Boolean = False
        Dim Infrazione14 As Boolean = False
        Dim DerogaTotale As Boolean = False
        Dim DerogaParziale As Boolean = False

        Dim DT As DataTable = LeggiRisposte(29, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0050" Then
                    Infrazione11 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0060" Then
                    Infrazione12 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0070" Then
                    Infrazione13 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0080" Then
                    Infrazione14 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0270" Then
                    DerogaTotale = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0271" Then
                    DerogaParziale = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' applico deroga
        If DerogaParziale Then
            Infrazione13 = False
            Infrazione14 = False
        ElseIf DerogaTotale Then
            Infrazione11 = False
            Infrazione12 = False
            Infrazione13 = False
            Infrazione14 = False
        End If

        ' calcolo punteggio
        If Infrazione11 Or Infrazione12 Or Infrazione13 Then
            Livello = 5
        ElseIf Infrazione14 Then
            Livello = 3
        End If

        Return Livello

    End Function

    ' BCAA 1 (ACQUE) - Durata
    Public Function Durata_29(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Portata As Integer = Portata_29(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0
        Dim Infrazione11 As Boolean = False
        Dim Infrazione12 As Boolean = False
        Dim Infrazione13 As Boolean = False
        Dim Infrazione14 As Boolean = False
        Dim DerogaTotale As Boolean = False
        Dim DerogaParziale As Boolean = False

        Dim DT As DataTable = LeggiRisposte(29, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0050" Then
                    Infrazione11 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0060" Then
                    Infrazione12 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0070" Then
                    Infrazione13 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0080" Then
                    Infrazione14 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0270" Then
                    DerogaTotale = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0271" Then
                    DerogaParziale = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' applico deroga
        If DerogaParziale Then
            Infrazione13 = False
            Infrazione14 = False
        ElseIf DerogaTotale Then
            Infrazione11 = False
            Infrazione12 = False
            Infrazione13 = False
            Infrazione14 = False
        End If

        ' calcolo punteggio
        If Infrazione13 Or Portata = 5 Then
            Livello = 5
        ElseIf Infrazione14 Then
            Livello = 3
        ElseIf Infrazione11 Or Infrazione12 Then
            If Portata = 1 Or Portata = 3 Then
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' BCAA 2 (ACQUE) - Portata, Gravità e Durata
    Public Function Portata_Gravita_Durata_30(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione21 As Boolean = False
        Dim Infrazione22 As Boolean = False
        Dim InfrazioneSAU As Boolean = False

        Dim DT As DataTable = LeggiRisposte(30, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    Infrazione21 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    Infrazione22 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    InfrazioneSAU = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' calcolo punteggio
        If InfrazioneSAU Then
            If Infrazione21 Then
                Livello = 5
            ElseIf Infrazione22 Then
                Livello = 3
            End If
        Else
            If Infrazione21 Or Infrazione22 Then
                Livello = 1
            End If
        End If

        Return Livello

    End Function

    ' BCAA 3 (ACQUE) - Portata, Gravità e Durata
    Public Function Portata_Gravita_Durata_31(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False
        Dim Infrazione4 As Boolean = False
        Dim Infrazione5 As Boolean = False
        Dim Infrazione6 As Boolean = False
        Dim Infrazione7 As Boolean = False
        Dim Perdite As Boolean = False
        Dim Dispersione As Boolean = False
        Dim Revoca As Boolean = False
        Dim ExtraAzienda As Boolean = False

        Dim DT As DataTable = LeggiRisposte(31, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0042" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0050" Then
                    Infrazione2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0088" Then
                    Infrazione3 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0089" Then
                    Infrazione4 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0090" Then
                    Infrazione5 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0092" Then
                    Infrazione5 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0093" Then
                    Infrazione5 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0044" Then
                    Perdite = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0052" Then
                    Dispersione = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0094" Then
                    Revoca = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0099" Then
                    ExtraAzienda = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' calcolo punteggio
        If Infrazione6 Or Infrazione7 Then
            If Infrazione6 Or Revoca Then
                Livello = 5
            Else
                If ExtraAzienda Or Infrazione3 Or Infrazione4 Or Infrazione5 Then
                    Livello = 5
                ElseIf (Infrazione1 And Perdite) Or (Infrazione2 And Dispersione) Then
                    Livello = 5
                Else
                    Livello = 3
                End If
            End If
        Else
            If ExtraAzienda Or Infrazione3 Or Infrazione4 Or Infrazione5 Then
                Livello = 5
            ElseIf (Infrazione1 And Perdite) Or (Infrazione2 And Dispersione) Then
                Livello = 3
            ElseIf Infrazione1 Or Infrazione2 Then
                Livello = 1
            End If
        End If

        Return Livello

    End Function

    ' BCAA 4 (SUOLO E STOCK DI CARBONIO) - Portata
    Public Function Portata_40(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False
        Dim Inferiore2Ettari As Boolean = False
        Dim Inferiore20SAU As Boolean = False
        Dim Superiore30SAU As Boolean = False
        Dim ExtraAzienda As Boolean = False
        Dim Deroga As Boolean = False

        Dim DT As DataTable = LeggiRisposte(40, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0060" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0078" Then
                    Infrazione2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0088" Then
                    Infrazione3 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0090" Then
                    Inferiore2Ettari = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0091" Then
                    Inferiore20SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0092" Then
                    Superiore30SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0095" Then
                    ExtraAzienda = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0160" Or
                    DT.Rows(i).Item("Punto_Numero") = "0170" Or
                    DT.Rows(i).Item("Punto_Numero") = "0180" Or
                    DT.Rows(i).Item("Punto_Numero") = "0190" Or
                    DT.Rows(i).Item("Punto_Numero") = "0200" Or
                    DT.Rows(i).Item("Punto_Numero") = "0210" Or
                    DT.Rows(i).Item("Punto_Numero") = "0215" Or
                    DT.Rows(i).Item("Punto_Numero") = "0217" Then
                    Deroga = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' applica deroga
        If Deroga Then
            Infrazione1 = False
            Infrazione2 = False
            Infrazione3 = False
        End If

        ' calcolo punteggio
        If Infrazione1 Or Infrazione2 Or Infrazione3 Then
            If Superiore30SAU Or ExtraAzienda Then
                Livello = 5
            ElseIf Inferiore2Ettari And Inferiore20SAU Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' BCAA 4 (SUOLO E STOCK DI CARBONIO) - Gravità
    Public Function Gravita_40(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False
        Dim ExtraAzienda As Boolean = False
        Dim Deroga As Boolean = False

        Dim DT As DataTable = LeggiRisposte(40, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0060" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0078" Then
                    Infrazione2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0088" Then
                    Infrazione3 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0095" Then
                    ExtraAzienda = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0160" Or
                    DT.Rows(i).Item("Punto_Numero") = "0170" Or
                    DT.Rows(i).Item("Punto_Numero") = "0180" Or
                    DT.Rows(i).Item("Punto_Numero") = "0190" Or
                    DT.Rows(i).Item("Punto_Numero") = "0200" Or
                    DT.Rows(i).Item("Punto_Numero") = "0210" Or
                    DT.Rows(i).Item("Punto_Numero") = "0215" Or
                    DT.Rows(i).Item("Punto_Numero") = "0217" Then
                    Deroga = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' applica deroga
        If Deroga Then
            Infrazione1 = False
            Infrazione2 = False
            Infrazione3 = False
        End If

        Dim NumInfrazioni As Integer = 0
        If Infrazione1 Then
            NumInfrazioni += 1
        End If
        If Infrazione2 Then
            NumInfrazioni += 1
        End If
        If Infrazione3 Then
            NumInfrazioni += 1
        End If

        ' calcolo punteggio
        If NumInfrazioni > 0 Then
            If NumInfrazioni > 1 Then
                Livello = 5
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' BCAA 4 (SUOLO E STOCK DI CARBONIO) - Durata
    Public Function Durata_40(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False
        Dim ExtraAzienda As Boolean = False
        Dim Deroga As Boolean = False

        Dim DT As DataTable = LeggiRisposte(40, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0060" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0078" Then
                    Infrazione2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0088" Then
                    Infrazione3 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0095" Then
                    ExtraAzienda = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0160" Or
                    DT.Rows(i).Item("Punto_Numero") = "0170" Or
                    DT.Rows(i).Item("Punto_Numero") = "0180" Or
                    DT.Rows(i).Item("Punto_Numero") = "0190" Or
                    DT.Rows(i).Item("Punto_Numero") = "0200" Or
                    DT.Rows(i).Item("Punto_Numero") = "0210" Or
                    DT.Rows(i).Item("Punto_Numero") = "0215" Or
                    DT.Rows(i).Item("Punto_Numero") = "0217" Then
                    Deroga = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' applica deroga
        If Deroga Then
            Infrazione1 = False
            Infrazione2 = False
            Infrazione3 = False
        End If

        Dim NumInfrazioni As Integer = 0
        If Infrazione1 Then
            NumInfrazioni += 1
        End If
        If Infrazione2 Then
            NumInfrazioni += 1
        End If
        If Infrazione3 Then
            NumInfrazioni += 1
        End If

        ' calcolo punteggio
        If NumInfrazioni > 0 Then
            If NumInfrazioni > 1 Or ExtraAzienda Then
                Livello = 5
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' BCAA 5 (SUOLO E STOCK DI CARBONIO) - Portata
    Public Function Portata_50(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False
        Dim Inferiore2Ettari As Boolean = False
        Dim Inferiore20SAU As Boolean = False
        Dim Superiore30SAU As Boolean = False
        Dim ExtraAzienda As Boolean = False
        Dim Deroga As Boolean = False

        Dim DT As DataTable = LeggiRisposte(50, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    Infrazione1 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0042" Then
                    Inferiore2Ettari = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0043" Then
                    Inferiore20SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0044" Then
                    Superiore30SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0045" Then
                    ExtraAzienda = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0110" Or
                    DT.Rows(i).Item("Punto_Numero") = "0120" Or
                    DT.Rows(i).Item("Punto_Numero") = "0130" Then
                    Deroga = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' applica deroga
        If Deroga Then
            Infrazione1 = False
            Infrazione2 = False
            Infrazione3 = False
        End If

        ' calcolo punteggio
        If Infrazione1 Or Infrazione2 Or Infrazione3 Then
            If Superiore30SAU Or ExtraAzienda Then
                Livello = 5
            ElseIf Inferiore2Ettari And Inferiore20SAU Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' BCAA 5 (SUOLO E STOCK DI CARBONIO) - Gravità
    Public Function Gravita_50(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False
        Dim Inferiore2Ettari As Boolean = False
        Dim Inferiore20SAU As Boolean = False
        Dim Superiore30SAU As Boolean = False
        Dim ExtraAzienda As Boolean = False
        Dim Deroga As Boolean = False

        Dim DT As DataTable = LeggiRisposte(50, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    Infrazione1 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0042" Then
                    Inferiore2Ettari = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0043" Then
                    Inferiore20SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0044" Then
                    Superiore30SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0045" Then
                    ExtraAzienda = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0110" Or
                    DT.Rows(i).Item("Punto_Numero") = "0120" Or
                    DT.Rows(i).Item("Punto_Numero") = "0130" Then
                    Deroga = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' applica deroga
        If Deroga Then
            Infrazione1 = False
            Infrazione2 = False
            Infrazione3 = False
        End If

        Dim NumInfrazioni As Integer = 0
        If Infrazione1 Then
            NumInfrazioni += 1
        End If
        If Infrazione2 Then
            NumInfrazioni += 1
        End If
        If Infrazione3 Then
            NumInfrazioni += 1
        End If

        ' calcolo punteggio
        If NumInfrazioni > 0 Then
            If Superiore30SAU Or ExtraAzienda Then
                Livello = 5
            ElseIf Inferiore2Ettari And Inferiore20SAU Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' BCAA 5 (SUOLO E STOCK DI CARBONIO) - Durata
    Public Function Durata_50(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Portata As Integer = Portata_50(SuperPiva, Piva, Regolamento_Cod)
        Dim Gravita As Integer = Gravita_50(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False
        Dim Inferiore2Ettari As Boolean = False
        Dim Inferiore20SAU As Boolean = False
        Dim Superiore30SAU As Boolean = False
        Dim ExtraAzienda As Boolean = False
        Dim Deroga As Boolean = False

        Dim DT As DataTable = LeggiRisposte(50, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    Infrazione1 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0042" Then
                    Inferiore2Ettari = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0043" Then
                    Inferiore20SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0044" Then
                    Superiore30SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0045" Then
                    ExtraAzienda = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0110" Or
                    DT.Rows(i).Item("Punto_Numero") = "0120" Or
                    DT.Rows(i).Item("Punto_Numero") = "0130" Then
                    Deroga = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' applica deroga
        If Deroga Then
            Infrazione1 = False
            Infrazione2 = False
            Infrazione3 = False
        End If

        Dim NumInfrazioni As Integer = 0
        If Infrazione1 Then
            NumInfrazioni += 1
        End If
        If Infrazione2 Then
            NumInfrazioni += 1
        End If
        If Infrazione3 Then
            NumInfrazioni += 1
        End If

        ' calcolo punteggio
        If NumInfrazioni > 0 Then
            If Infrazione1 And Infrazione3 Then
                Livello = 5
            ElseIf Infrazione1 Or Infrazione3 Then
                If Portata = 5 Then
                    Livello = 5
                ElseIf Portata > 0 Then
                    Livello = 3
                End If
            End If
        End If

        Return Livello

    End Function

    ' BCAA 6 (SUOLO E STOCK DI CARBONIO) - Portata
    Public Function Portata_10(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione61 As Boolean = False
        Dim Infrazione62 As Boolean = False
        Dim Inferiore2Ettari As Boolean = False
        Dim Inferiore20SAU As Boolean = False
        Dim Superiore30SAU As Boolean = False
        Dim ExtraAzienda As Boolean = False
        Dim Deroga As Boolean = False

        Dim DT As DataTable = LeggiRisposte(10, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    Infrazione61 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    Infrazione62 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0041" Then
                    Inferiore2Ettari = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0042" Then
                    Inferiore20SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0043" Then
                    Superiore30SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0044" Then
                    ExtraAzienda = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0110" Or
                    DT.Rows(i).Item("Punto_Numero") = "0120" Or
                    DT.Rows(i).Item("Punto_Numero") = "0121" Then
                    Deroga = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' applica deroga
        If Deroga Then
            Infrazione61 = False
            Infrazione62 = False
        End If

        ' calcolo punteggio
        If Infrazione61 Or Infrazione62 Then
            If Superiore30SAU Or ExtraAzienda Then
                Livello = 5
            ElseIf Inferiore2Ettari And Inferiore20SAU Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' BCAA 6 (SUOLO E STOCK DI CARBONIO) - Gravità
    Public Function Gravita_10(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Portata As Integer = Portata_10(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0
        Dim Infrazione61 As Boolean = False
        Dim Infrazione62 As Boolean = False
        Dim ExtraAzienda As Boolean = False
        Dim Deroga As Boolean = False

        Dim DT As DataTable = LeggiRisposte(10, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    Infrazione61 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    Infrazione62 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0044" Then
                    ExtraAzienda = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0110" Or
                    DT.Rows(i).Item("Punto_Numero") = "0120" Or
                    DT.Rows(i).Item("Punto_Numero") = "0121" Then
                    Deroga = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' applica deroga
        If Deroga Then
            Infrazione61 = False
            Infrazione62 = False
        End If

        ' calcolo punteggio
        If Infrazione61 Or Infrazione62 Then
            If Portata = 5 Then
                Livello = 5
            ElseIf Portata > 0 Then
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' BCAA 6 (SUOLO E STOCK DI CARBONIO) - Durata
    Public Function Durata_10(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Portata As Integer = Portata_10(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0
        Dim Infrazione61 As Boolean = False
        Dim Infrazione62 As Boolean = False
        Dim ExtraAzienda As Boolean = False
        Dim Deroga As Boolean = False

        Dim DT As DataTable = LeggiRisposte(10, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    Infrazione61 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    Infrazione62 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0044" Then
                    ExtraAzienda = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0110" Or
                    DT.Rows(i).Item("Punto_Numero") = "0120" Or
                    DT.Rows(i).Item("Punto_Numero") = "0121" Then
                    Deroga = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' applica deroga
        If Deroga Then
            Infrazione61 = False
            Infrazione62 = False
        End If

        ' calcolo punteggio
        If Infrazione61 Or Infrazione62 Then
            If Portata = 5 Or ExtraAzienda Then
                Livello = 5
            ElseIf Portata = 3 Then
                Livello = 3
            ElseIf Portata = 1 Then
                Livello = 1
            End If
        End If

        Return Livello

    End Function

    ' CGO 2 (BIODIVERSITA') - Portata
    Public Function Portata_1(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False
        Dim Infrazione4 As Boolean = False
        Dim Infrazione5 As Boolean = False
        Dim Superiore1Ettaro As Boolean = False
        Dim Superiore2Ettari As Boolean = False
        Dim Superiore20SAU As Boolean = False
        Dim Superiore30SAU As Boolean = False
        Dim ExtraAzienda As Boolean = False

        Dim DT As DataTable = LeggiRisposte(1, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    Infrazione2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0031" Or
                    DT.Rows(i).Item("Punto_Numero") = "0033" Then
                    Infrazione3 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Or
                    DT.Rows(i).Item("Punto_Numero") = "0041" Then
                    Infrazione4 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0080" Or
                    DT.Rows(i).Item("Punto_Numero") = "0090" Then
                    Infrazione5 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0092" Then
                    Superiore20SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0093" Then
                    Superiore2Ettari = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0094" Then
                    Superiore30SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0095" Then
                    ExtraAzienda = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0097" Then
                    Superiore1Ettaro = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' calcolo punteggio
        If Infrazione5 Then
            Livello = 5
        ElseIf (Infrazione1 Or Infrazione3 Or Infrazione4) And Superiore30SAU Then
            Livello = 5
        ElseIf (Infrazione2 And Superiore1Ettaro) Or ExtraAzienda Then
            Livello = 5
        ElseIf (Infrazione1 Or Infrazione3 Or Infrazione4) And Not Superiore20SAU And Not Superiore2Ettari Then
            Livello = 1
        Else
            Livello = 3
        End If

        Return Livello

    End Function

    ' CGO 2 (BIODIVERSITA') - Gravità
    Public Function Gravita_1(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False
        Dim Infrazione4 As Boolean = False
        Dim Infrazione5 As Boolean = False

        Dim DT As DataTable = LeggiRisposte(1, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    Infrazione2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0031" Or
                    DT.Rows(i).Item("Punto_Numero") = "0033" Then
                    Infrazione3 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Or
                    DT.Rows(i).Item("Punto_Numero") = "0041" Then
                    Infrazione4 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0080" Or
                    DT.Rows(i).Item("Punto_Numero") = "0090" Then
                    Infrazione5 = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        Dim NumInfrazioni As Integer = 0
        If Infrazione1 Then
            NumInfrazioni += 1
        End If
        If Infrazione3 Then
            NumInfrazioni += 1
        End If
        If Infrazione4 Then
            NumInfrazioni += 1
        End If

        ' calcolo punteggio
        If NumInfrazioni = 3 Or Infrazione2 Or Infrazione5 Then
            Livello = 5
        ElseIf NumInfrazioni = 2 Then
            Livello = 3
        ElseIf NumInfrazioni = 1 Then
            Livello = 1
        End If

        Return Livello

    End Function

    ' CGO 2 (BIODIVERSITA') - Durata
    Public Function Durata_1(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Portata As Integer = Portata_1(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0

        If Portata = 5 Then
            Livello = 5
        ElseIf Portata = 3 Then
            Livello = 3
        End If

        Return Livello

    End Function

    ' CGO 3 (BIODIVERSITA') - Portata
    Public Function Portata_5(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False
        Dim Infrazione4 As Boolean = False
        Dim Superiore1Ettaro As Boolean = False
        Dim Superiore2Ettari As Boolean = False
        Dim Superiore20SAU As Boolean = False
        Dim Superiore30SAU As Boolean = False
        Dim ExtraAzienda As Boolean = False

        Dim DT As DataTable = LeggiRisposte(5, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    Infrazione2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0031" Or
                    DT.Rows(i).Item("Punto_Numero") = "0033" Then
                    Infrazione3 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Or
                    DT.Rows(i).Item("Punto_Numero") = "0041" Or
                    DT.Rows(i).Item("Punto_Numero") = "0042" Then
                    Infrazione4 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0045" Then
                    Superiore20SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0046" Then
                    Superiore2Ettari = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0047" Then
                    Superiore30SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0048" Then
                    ExtraAzienda = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0050" Then
                    Superiore1Ettaro = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' calcolo punteggio
        If (Infrazione1 Or Infrazione3 Or Infrazione4) And Superiore30SAU Then
            Livello = 5
        ElseIf (Infrazione2 And Superiore1Ettaro) Or ExtraAzienda Then
            Livello = 5
        ElseIf (Infrazione1 Or Infrazione3 Or Infrazione4) And Not Superiore20SAU And Not Superiore2Ettari Then
            Livello = 1
        Else
            Livello = 3
        End If

        Return Livello

    End Function

    ' CGO 3 (BIODIVERSITA') - Gravità
    Public Function Gravita_5(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False
        Dim Infrazione4 As Boolean = False

        Dim DT As DataTable = LeggiRisposte(5, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    Infrazione2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0031" Or
                    DT.Rows(i).Item("Punto_Numero") = "0033" Then
                    Infrazione3 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Or
                    DT.Rows(i).Item("Punto_Numero") = "0041" Or
                    DT.Rows(i).Item("Punto_Numero") = "0042" Then
                    Infrazione4 = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        Dim NumInfrazioni As Integer = 0
        If Infrazione1 Then
            NumInfrazioni += 1
        End If
        If Infrazione3 Then
            NumInfrazioni += 1
        End If
        If Infrazione4 Then
            NumInfrazioni += 1
        End If

        ' calcolo punteggio
        If NumInfrazioni = 3 Or Infrazione2 Then
            Livello = 5
        ElseIf NumInfrazioni = 2 Then
            Livello = 3
        ElseIf NumInfrazioni = 1 Then
            Livello = 1
        End If

        Return Livello

    End Function

    ' CGO 3 (BIODIVERSITA') - Durata
    Public Function Durata_5(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Portata As Integer = Portata_5(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0

        If Portata = 5 Then
            Livello = 5
        ElseIf Portata = 3 Then
            Livello = 3
        End If

        Return Livello

    End Function

    ' BCAA7 (LIV. MIN. DI MANTENIMENTO DEI PAESAGGI) - Portata
    Public Function Portata_12(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Superiore3Ettari As Boolean = False
        Dim Superiore20SAU As Boolean = False
        Dim Superiore30SAU As Boolean = False
        Dim ExtraAzienda As Boolean = False
        Dim Deroga As Boolean = False

        Dim DT As DataTable = LeggiRisposte(12, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0150" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0160" Then
                    Infrazione2 = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0162" Then
                    Superiore20SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0163" Then
                    Superiore3Ettari = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0164" Then
                    Superiore30SAU = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0050" Then
                    ExtraAzienda = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0320" Or
                    DT.Rows(i).Item("Punto_Numero") = "0330" Or
                    DT.Rows(i).Item("Punto_Numero") = "0340" Or
                    DT.Rows(i).Item("Punto_Numero") = "0350" Or
                    DT.Rows(i).Item("Punto_Numero") = "0351" Or
                    DT.Rows(i).Item("Punto_Numero") = "0352" Then
                    Deroga = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        If Deroga Then
            Infrazione1 = False
            Infrazione2 = False
        End If

        ' calcolo punteggio
        If Infrazione1 Or Infrazione2 Then
            If Superiore30SAU Or ExtraAzienda Then
                Livello = 5
            ElseIf Superiore3Ettari Or Superiore20SAU Then
                Livello = 3
            Else
                Livello = 1
            End If
        End If

        Return Livello

    End Function

    ' BCAA7 (LIV. MIN. DI MANTENIMENTO DEI PAESAGGI) - Gravità
    Public Function Gravita_12(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Portata As Integer = Portata_12(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False

        Dim DT As DataTable = LeggiRisposte(12, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0150" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0160" Then
                    Infrazione2 = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        If Portata = 5 And Infrazione2 Then
            Livello = 5
        ElseIf Infrazione1 And Portata > 0 Then
            Livello = 5
        ElseIf Infrazione2 And Portata > 0 Then
            Livello = 3
        End If

        Return Livello

    End Function

    ' BCAA7 (LIV. MIN. DI MANTENIMENTO DEI PAESAGGI) - Durata
    Public Function Durata_12(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False

        Dim DT As DataTable = LeggiRisposte(12, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0150" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0160" Then
                    Infrazione2 = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        If Infrazione1 Then
            Livello = 5
        ElseIf Infrazione2 Then
            Livello = 3
        End If

        Return Livello

    End Function

    ' CGO 4.1 (SICUREZZA ALIMENTARE-PRODUZIONI ANIMALI) - Portata
    Public Function Portata_33(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA As Boolean = False
        Dim InfrazioneB As Boolean = False
        Dim InfrazioneC As Boolean = False
        Dim InfrazioneD As Boolean = False
        Dim InfrazioneE As Boolean = False
        Dim InfrazioneF As Boolean = False
        Dim ExtraAzienda As Boolean = False

        Dim DT As DataTable = LeggiRisposte(33, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    InfrazioneA = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    InfrazioneB = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    InfrazioneC = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0050" Or
                    DT.Rows(i).Item("Punto_Numero") = "0060" Or
                    DT.Rows(i).Item("Punto_Numero") = "0070" Or
                    DT.Rows(i).Item("Punto_Numero") = "0080" Or
                    DT.Rows(i).Item("Punto_Numero") = "0081" Then
                    InfrazioneD = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0082" Then
                    InfrazioneE = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0083" Then
                    InfrazioneF = True
                End If
            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0090" Then
                    ExtraAzienda = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' calcolo punteggio
        If InfrazioneA Or InfrazioneB Or InfrazioneC Or InfrazioneD Or InfrazioneE Or InfrazioneF Then
            If ExtraAzienda Then
                Livello = 5
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 4.1 (SICUREZZA ALIMENTARE-PRODUZIONI ANIMALI) - Gravità
    Public Function Gravita_33(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA As Boolean = False
        Dim InfrazioneB As Boolean = False
        Dim CarenzeStrutturali As Boolean = False

        Dim DT As DataTable = LeggiRisposte(33, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    InfrazioneA = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    InfrazioneB = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "1451" Then
                    CarenzeStrutturali = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' calcolo punteggio
        If InfrazioneA Or InfrazioneB Then
            If CarenzeStrutturali Then
                Livello = 5
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 4.1 (SICUREZZA ALIMENTARE-PRODUZIONI ANIMALI) - Durata
    Public Function Durata_33(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione As Boolean = False
        Dim CarenzeStrutturali As Boolean = False

        Dim DT As DataTable = LeggiRisposte(33, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0020" Or
                    DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0040" Or
                    DT.Rows(i).Item("Punto_Numero") = "0050" Or
                    DT.Rows(i).Item("Punto_Numero") = "0060" Or
                    DT.Rows(i).Item("Punto_Numero") = "0070" Or
                    DT.Rows(i).Item("Punto_Numero") = "0080" Or
                    DT.Rows(i).Item("Punto_Numero") = "0081" Or
                    DT.Rows(i).Item("Punto_Numero") = "0082" Or
                    DT.Rows(i).Item("Punto_Numero") = "0083" Then
                    Infrazione = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "1451" Then
                    CarenzeStrutturali = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' calcolo punteggio
        If Infrazione Then
            If CarenzeStrutturali Then
                Livello = 5
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 4.2 (SICUREZZA ALIMENTARE-PRODUZIONI VEGETALI) - Portata
    Public Function Portata_34(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA As Boolean = False
        Dim InfrazioneB As Boolean = False
        Dim InfrazioneC As Boolean = False
        Dim InfrazioneD As Boolean = False
        Dim ExtraAzienda As Boolean = False

        Dim DT As DataTable = LeggiRisposte(34, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0100" Then
                    InfrazioneA = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0110" Then
                    InfrazioneB = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0120" Or DT.Rows(i).Item("Punto_Numero") = "0130" Or DT.Rows(i).Item("Punto_Numero") = "0140" Then
                    InfrazioneC = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0150" Then
                    InfrazioneD = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0155" Then
                    ExtraAzienda = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' calcolo punteggio
        If InfrazioneA Or InfrazioneB Or InfrazioneC Or InfrazioneD Then
            If ExtraAzienda Then
                Livello = 5
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 4.2 (SICUREZZA ALIMENTARE-PRODUZIONI VEGETALI) - Gravità
    Public Function Gravita_34(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA As Boolean = False
        Dim InfrazioneB As Boolean = False
        Dim InfrazioneC As Boolean = False
        Dim InfrazioneD As Boolean = False
        Dim CarenzeStrutturali As Boolean = False

        Dim DT As DataTable = LeggiRisposte(34, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0100" Then
                    InfrazioneA = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0110" Then
                    InfrazioneB = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0120" Or DT.Rows(i).Item("Punto_Numero") = "0130" Or DT.Rows(i).Item("Punto_Numero") = "0140" Then
                    InfrazioneC = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0150" Then
                    InfrazioneD = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "1461" Then
                    CarenzeStrutturali = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' calcolo punteggio
        If (InfrazioneA And InfrazioneD) Or (InfrazioneB And InfrazioneC) Then
            Livello = 5
        ElseIf InfrazioneA Then
            Livello = 3
        ElseIf InfrazioneB Or InfrazioneC Then
            Livello = 1
        End If

        Return Livello

    End Function

    ' CGO 4.2 (SICUREZZA ALIMENTARE-PRODUZIONI VEGETALI) - Durata
    Public Function Durata_34(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione As Boolean = False
        Dim CarenzeStrutturali As Boolean = False

        Dim DT As DataTable = LeggiRisposte(34, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0100" Or
                    DT.Rows(i).Item("Punto_Numero") = "0110" Or
                    DT.Rows(i).Item("Punto_Numero") = "0120" Or
                    DT.Rows(i).Item("Punto_Numero") = "0130" Or
                    DT.Rows(i).Item("Punto_Numero") = "0140" Or
                    DT.Rows(i).Item("Punto_Numero") = "0150" Then
                    Infrazione = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "1461" Then
                    CarenzeStrutturali = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' calcolo punteggio
        If Infrazione Then
            If CarenzeStrutturali Then
                Livello = 5
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 4.3 (SICUREZZA ALIMENTARE-PRODUZIONE LATTE) - Portata, Gravità e Durata
    Public Function Portata_Gravita_Durata_35(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione As Boolean = False

        Dim DT As DataTable = LeggiRisposte(35, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0221" Or
                    DT.Rows(i).Item("Punto_Numero") = "0222" Or
                    DT.Rows(i).Item("Punto_Numero") = "0223" Or
                    DT.Rows(i).Item("Punto_Numero") = "0224" Or
                    DT.Rows(i).Item("Punto_Numero") = "0231" Or
                    DT.Rows(i).Item("Punto_Numero") = "0232" Or
                    DT.Rows(i).Item("Punto_Numero") = "0233" Or
                    DT.Rows(i).Item("Punto_Numero") = "0234" Or
                    DT.Rows(i).Item("Punto_Numero") = "0235" Or
                    DT.Rows(i).Item("Punto_Numero") = "0290" Or
                    DT.Rows(i).Item("Punto_Numero") = "0300" Or
                    DT.Rows(i).Item("Punto_Numero") = "0310" Or
                    DT.Rows(i).Item("Punto_Numero") = "0322" Then
                    Infrazione = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        If Infrazione Then
            Livello = 3
        End If

        Return Livello

    End Function

    ' CGO 4.3 (SICUREZZA ALIMENTARE-PRODUZIONE LATTE) - Portata
    Public Function Portata_35(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione As Boolean = False
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False

        Dim DT As DataTable = LeggiRisposte(35, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0316" Then
                    Infrazione = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0317" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0318" Then
                    Infrazione2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0319" Then
                    Infrazione3 = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        If Infrazione Then
            If Infrazione1 Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 4.3 (SICUREZZA ALIMENTARE-PRODUZIONE LATTE) - Gravità
    Public Function Gravita_35(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione As Boolean = False
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False

        Dim DT As DataTable = LeggiRisposte(35, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0316" Then
                    Infrazione = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0317" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0318" Then
                    Infrazione2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0319" Then
                    Infrazione3 = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        If Infrazione Then
            If Infrazione2 Then
                Livello = 5
            ElseIf Infrazione3 Then
                Livello = 3
            Else
                Livello = 1
            End If
        End If

        Return Livello

    End Function

    ' CGO 4.3 (SICUREZZA ALIMENTARE-PRODUZIONE LATTE) - Durata
    Public Function Durata_35(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione As Boolean = False
        Dim Infrazione1 As Boolean = False
        Dim Infrazione2 As Boolean = False
        Dim Infrazione3 As Boolean = False

        Dim DT As DataTable = LeggiRisposte(35, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0316" Then
                    Infrazione = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0317" Then
                    Infrazione1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0318" Then
                    Infrazione2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0319" Then
                    Infrazione3 = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        If Infrazione Then
            If Infrazione2 And Infrazione3 Then
                Livello = 5
            ElseIf Infrazione2 Or Infrazione3 Then
                Livello = 3
            Else
                Livello = 1
            End If
        End If

        Return Livello

    End Function

    ' CGO 4.4 (SICUREZZA ALIMENTARE-PRODUZIONE UOVO) - Portata, Gravita e Durata
    Public Function Portata_Gravita_Durata_36(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione As Boolean = False

        Dim DT As DataTable = LeggiRisposte(36, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0380" Then
                    Infrazione = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        If Infrazione Then
            Livello = 3
        End If

        Return Livello

    End Function

    ' CGO 4.5 (SICUREZZA ALIMENTARE-PRODUZIONE MANGIMI) - Portata
    Public Function Portata_37(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA As Boolean = False
        Dim InfrazioneB As Boolean = False
        Dim InfrazioneC As Boolean = False
        Dim InfrazioneD As Boolean = False
        Dim ExtraAzienda As Boolean = False

        Dim DT As DataTable = LeggiRisposte(37, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then

                If DT.Rows(i).Item("Punto_Numero") = "0405" Then
                    InfrazioneA = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0410" Then
                    InfrazioneB = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0420" Then
                    InfrazioneC = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0422" Or
                    DT.Rows(i).Item("Punto_Numero") = "0430" Or
                    DT.Rows(i).Item("Punto_Numero") = "0440" Or
                    DT.Rows(i).Item("Punto_Numero") = "0450" Then
                    InfrazioneD = True
                End If

            ElseIf DT.Rows(i).Item("Valore") = "1" Then

                If DT.Rows(i).Item("Punto_Numero") = "0380" Then
                    ExtraAzienda = True
                End If

            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioneA Or InfrazioneB Or InfrazioneC Or InfrazioneD Then
            If ExtraAzienda Then
                Livello = 5
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 4.5 (SICUREZZA ALIMENTARE-PRODUZIONE MANGIMI) - Gravità
    Public Function Gravita_37(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA As Boolean = False
        Dim InfrazioneB As Boolean = False
        Dim InfrazioneC As Boolean = False
        Dim InfrazioneD As Boolean = False
        Dim InfrazioneD3 As Boolean = False

        Dim DT As DataTable = LeggiRisposte(37, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0405" Then
                    InfrazioneA = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0410" Then
                    InfrazioneB = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0420" Then
                    InfrazioneC = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0422" Or
                    DT.Rows(i).Item("Punto_Numero") = "0430" Or
                    DT.Rows(i).Item("Punto_Numero") = "0440" Or
                    DT.Rows(i).Item("Punto_Numero") = "0450" Then
                    InfrazioneD = True
                    If DT.Rows(i).Item("Punto_Numero") = "0450" Then
                        InfrazioneD3 = True
                    End If
                End If
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioneA Or InfrazioneB Or InfrazioneC Or InfrazioneD Then
            If InfrazioneB And InfrazioneD3 Then
                Livello = 5
            ElseIf InfrazioneB Then
                Livello = 3
            Else
                Livello = 1
            End If
        End If

        Return Livello

    End Function

    ' CGO 4.5 (SICUREZZA ALIMENTARE-PRODUZIONE LATTE) - Durata
    Public Function Durata_37(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA As Boolean = False
        Dim InfrazioneB As Boolean = False
        Dim InfrazioneC As Boolean = False
        Dim InfrazioneD As Boolean = False

        Dim DT As DataTable = LeggiRisposte(37, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0405" Then
                    InfrazioneA = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0410" Then
                    InfrazioneB = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0420" Then
                    InfrazioneC = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0422" Or
                    DT.Rows(i).Item("Punto_Numero") = "0430" Or
                    DT.Rows(i).Item("Punto_Numero") = "0440" Or
                    DT.Rows(i).Item("Punto_Numero") = "0450" Then
                    InfrazioneD = True
                End If
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioneA Or InfrazioneB Or InfrazioneC Or InfrazioneD Then
            If InfrazioneB Then
                Livello = 5
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 5 (SICUREZZA ALIMENTARE) - Portata, Gravità e Durata
    Public Function Portata_Gravita_Durata_16(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione As Boolean = False

        Dim DT As DataTable = LeggiRisposte(16, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0010" Or
                    DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    Infrazione = True
                End If
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If Infrazione Then
            Livello = 5
        End If

        Return Livello

    End Function

    ' CGO 6 (IDENTIFICAZIONE E REGISTRAZIONE ANIMALI) - Portata
    Public Function Portata_6(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA1 As Boolean = False
        Dim InfrazioneA2 As Boolean = False
        Dim InfrazioneB1 As Boolean = False
        Dim InfrazioneB2 As Boolean = False
        Dim InfrazioneB3 As Boolean = False
        Dim InfrazioneB4 As Boolean = False
        Dim InfrazioneC1 As Boolean = False
        Dim IncidenzaCapi As Double = 0
        Dim CapiNonConformi As Double = 0

        Dim DT As DataTable = LeggiRisposte(6, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    InfrazioneA1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    InfrazioneA2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    InfrazioneB1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    InfrazioneB2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0041" Then
                    InfrazioneB3 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0042" Or
                    DT.Rows(i).Item("Punto_Numero") = "0043" Or
                    DT.Rows(i).Item("Punto_Numero") = "0044" Then
                    InfrazioneB4 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0045" Then
                    InfrazioneC1 = True
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0052" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                CapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            ElseIf DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                IncidenzaCapi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioneA1 Or InfrazioneA2 Or InfrazioneB1 Or InfrazioneB2 Or InfrazioneB3 Or InfrazioneB4 Or InfrazioneC1 Then
            If IncidenzaCapi > 10 Or CapiNonConformi > 20 Or (InfrazioneB4 And InfrazioneC1) Then
                Livello = 5
            ElseIf (IncidenzaCapi <= 5 And CapiNonConformi <= 10) Or InfrazioneA2 Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 6 (IDENTIFICAZIONE E REGISTRAZIONE ANIMALI) - Gravità
    Public Function Gravita_6(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA1 As Boolean = False
        Dim InfrazioneA2 As Boolean = False
        Dim InfrazioneB1 As Boolean = False
        Dim InfrazioneB2 As Boolean = False
        Dim InfrazioneB3 As Boolean = False
        Dim InfrazioneB4 As Boolean = False
        Dim InfrazioneC1 As Boolean = False
        Dim IncidenzaCapi As Double = 0
        Dim CapiNonConformi As Double = 0

        Dim DT As DataTable = LeggiRisposte(6, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    InfrazioneA1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    InfrazioneA2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    InfrazioneB1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    InfrazioneB2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0041" Then
                    InfrazioneB3 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0042" Or
                    DT.Rows(i).Item("Punto_Numero") = "0043" Or
                    DT.Rows(i).Item("Punto_Numero") = "0044" Then
                    InfrazioneB4 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0045" Then
                    InfrazioneC1 = True
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0052" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                CapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            ElseIf DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                IncidenzaCapi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            End If

        Next

        DT.Dispose()
        DT = Nothing

        Dim Infrazione1 As Boolean = (InfrazioneB1 And InfrazioneB2 And InfrazioneB3) Or InfrazioneA2
        Dim Infrazione2 As Boolean = InfrazioneC1
        Dim Infrazione3 As Boolean = InfrazioneC1
        Dim Infrazione4 As Boolean = InfrazioneB4 And InfrazioneC1

        If (Infrazione1 And Infrazione2 And Infrazione3) Or Infrazione4 Then
            Livello = 5
        ElseIf Infrazione1 And Infrazione2 Then
            Livello = 3
        ElseIf Infrazione1 Or Infrazione2 Then
            Livello = 1
        End If

        Return Livello

    End Function

    ' CGO 6 (IDENTIFICAZIONE E REGISTRAZIONE ANIMALI) - Durata
    Public Function Durata_6(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer
        Dim Portata As Integer = Portata_6(SuperPiva, Piva, Regolamento_Cod)
        Dim Gravita As Integer = Gravita_6(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0
        Dim InfrazioneA1 As Boolean = False
        Dim InfrazioneA2 As Boolean = False
        Dim InfrazioneB1 As Boolean = False
        Dim InfrazioneB2 As Boolean = False
        Dim InfrazioneB3 As Boolean = False
        Dim InfrazioneB4 As Boolean = False
        Dim InfrazioneC1 As Boolean = False
        Dim IncidenzaCapi As Double = 0
        Dim CapiNonConformi As Double = 0

        Dim DT As DataTable = LeggiRisposte(6, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    InfrazioneA1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    InfrazioneA2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    InfrazioneB1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    InfrazioneB2 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0041" Then
                    InfrazioneB3 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0042" Or
                    DT.Rows(i).Item("Punto_Numero") = "0043" Or
                    DT.Rows(i).Item("Punto_Numero") = "0044" Then
                    InfrazioneB4 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0045" Then
                    InfrazioneC1 = True
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0052" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                CapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            ElseIf DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                IncidenzaCapi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioneA1 Or InfrazioneA2 Or InfrazioneB1 Or InfrazioneB2 Or InfrazioneB3 Or InfrazioneB4 Or InfrazioneC1 Then
            If IncidenzaCapi > 50 Or (InfrazioneB4 And InfrazioneC1) Then
                Livello = 5
            ElseIf Portata = 1 And Gravita = 1 Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 7 (IDENTIFICAZIONE E REGISTRAZIONE ANIMALI) - Portata
    Public Function Portata_24(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA12 As Boolean = False
        Dim InfrazioneB12 As Boolean = False
        Dim InfrazioneB34 As Boolean = False
        Dim InfrazioneB5 As Boolean = False
        Dim InfrazioneC123 As Boolean = False
        Dim InfrazioneC4 As Boolean = False
        Dim InfrazioneDE As Boolean = False
        Dim IncidenzaCapi As Double = 0
        Dim CapiNonConformi As Double = 0

        Dim DT As DataTable = LeggiRisposte(6, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    InfrazioneA12 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    InfrazioneB12 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    InfrazioneB34 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    InfrazioneB5 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0041" Then
                    InfrazioneC123 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0042" Then
                    InfrazioneC4 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0043" Then
                    InfrazioneDE = True
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0052" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                CapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            ElseIf DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                IncidenzaCapi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioneA12 Or InfrazioneB12 Or InfrazioneB34 Or InfrazioneB5 Or InfrazioneC123 Or InfrazioneC4 Or InfrazioneDE Then
            If IncidenzaCapi > 10 Or CapiNonConformi > 10 Or (InfrazioneB12 And InfrazioneB34 And InfrazioneB5 And InfrazioneDE) Then
                Livello = 5
            ElseIf (IncidenzaCapi <= 5 And CapiNonConformi <= 5) Or InfrazioneA12 Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 7 (IDENTIFICAZIONE E REGISTRAZIONE ANIMALI) - Gravità
    Public Function Gravita_24(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA12 As Boolean = False
        Dim InfrazioneB12 As Boolean = False
        Dim InfrazioneB34 As Boolean = False
        Dim InfrazioneB5 As Boolean = False
        Dim InfrazioneC123 As Boolean = False
        Dim InfrazioneC4 As Boolean = False
        Dim InfrazioneDE As Boolean = False
        Dim IncidenzaCapi As Double = 0
        Dim CapiNonConformi As Double = 0

        Dim DT As DataTable = LeggiRisposte(6, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    InfrazioneA12 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    InfrazioneB12 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    InfrazioneB34 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    InfrazioneB5 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0041" Then
                    InfrazioneC123 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0042" Then
                    InfrazioneC4 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0043" Then
                    InfrazioneDE = True
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0052" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                CapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            ElseIf DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                IncidenzaCapi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            End If

        Next

        DT.Dispose()
        DT = Nothing

        Dim Infrazione1 As Boolean = InfrazioneC123 Or InfrazioneA12
        Dim Infrazione2 As Boolean = InfrazioneB12 Or InfrazioneB34 Or InfrazioneB5
        Dim Infrazione3 As Boolean = InfrazioneC4
        Dim Infrazione4 As Boolean = InfrazioneDE
        Dim Infrazione5 As Boolean = Infrazione4 Or Infrazione2

        Dim NumInfrazioni As Integer = 0
        If Infrazione1 Then
            NumInfrazioni += 1
        End If
        If Infrazione2 Then
            NumInfrazioni += 1
        End If
        If Infrazione3 Then
            NumInfrazioni += 1
        End If

        If InfrazioneA12 Or InfrazioneB12 Or InfrazioneB34 Or InfrazioneB5 Or InfrazioneC123 Or InfrazioneC4 Or InfrazioneDE Then
            If NumInfrazioni = 3 Or (NumInfrazioni = 2 And Infrazione4) Or Infrazione5 Then
                Livello = 5
            ElseIf NumInfrazioni = 2 Or Infrazione4 Then
                Livello = 3
            ElseIf NumInfrazioni = 1 Then
                Livello = 1
            End If
        End If

        Return Livello

    End Function

    ' CGO 7 (IDENTIFICAZIONE E REGISTRAZIONE ANIMALI) - Durata
    Public Function Durata_24(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer
        Dim Portata As Integer = Portata_24(SuperPiva, Piva, Regolamento_Cod)
        Dim Gravita As Integer = Gravita_24(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0
        Dim InfrazioneA12 As Boolean = False
        Dim InfrazioneB12 As Boolean = False
        Dim InfrazioneB34 As Boolean = False
        Dim InfrazioneB5 As Boolean = False
        Dim InfrazioneC123 As Boolean = False
        Dim InfrazioneC4 As Boolean = False
        Dim InfrazioneDE As Boolean = False
        Dim IncidenzaCapi As Double = 0
        Dim CapiNonConformi As Double = 0

        Dim DT As DataTable = LeggiRisposte(6, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    InfrazioneA12 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    InfrazioneB12 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    InfrazioneB34 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    InfrazioneB5 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0041" Then
                    InfrazioneC123 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0042" Then
                    InfrazioneC4 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0043" Then
                    InfrazioneDE = True
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0052" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                CapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            ElseIf DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                IncidenzaCapi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioneA12 Or InfrazioneB12 Or InfrazioneB34 Or InfrazioneB5 Or InfrazioneC123 Or InfrazioneC4 Or InfrazioneDE Then
            If IncidenzaCapi > 50 Or ((InfrazioneB12 Or InfrazioneB34 Or InfrazioneB5) And InfrazioneDE) Then
                Livello = 5
            ElseIf Portata = 1 And Gravita = 1 Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 8 (IDENTIFICAZIONE E REGISTRAZIONE ANIMALI) - Portata
    Public Function Portata_25(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA1 As Boolean = False
        Dim InfrazioneA3 As Boolean = False
        Dim InfrazioneB As Boolean = False
        Dim InfrazioneC As Boolean = False
        Dim IncidenzaCapi As Double = 0
        Dim CapiNonConformi As Double = 0

        Dim DT As DataTable = LeggiRisposte(6, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    InfrazioneA1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    InfrazioneA3 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    InfrazioneB = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    InfrazioneC = True
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0052" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                CapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            ElseIf DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                IncidenzaCapi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioneA1 Or InfrazioneA3 Or InfrazioneB Or InfrazioneC Then
            If IncidenzaCapi > 10 Or CapiNonConformi > 40 Or (InfrazioneB And InfrazioneC) Then
                Livello = 5
            ElseIf (IncidenzaCapi <= 5 And CapiNonConformi <= 20) Or InfrazioneA3 Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 8 (IDENTIFICAZIONE E REGISTRAZIONE ANIMALI) - Gravità
    Public Function Gravita_25(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioneA1 As Boolean = False
        Dim InfrazioneA3 As Boolean = False
        Dim InfrazioneB As Boolean = False
        Dim InfrazioneC As Boolean = False
        Dim IncidenzaCapi As Double = 0
        Dim CapiNonConformi As Double = 0

        Dim DT As DataTable = LeggiRisposte(6, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    InfrazioneA1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    InfrazioneA3 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    InfrazioneB = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    InfrazioneC = True
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0052" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                CapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            ElseIf DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                IncidenzaCapi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            End If

        Next

        DT.Dispose()
        DT = Nothing

        Dim Infrazione1 As Boolean = InfrazioneB Or InfrazioneA3
        Dim Infrazione2 As Boolean = InfrazioneC 'marcatura non conforme ???
        Dim Infrazione3 As Boolean = InfrazioneC 'senza marcatura ???
        Dim Infrazione4 As Boolean = InfrazioneB
        Dim Infrazione5 As Boolean = InfrazioneB And InfrazioneC

        Dim NumInfrazioni As Integer = 0
        If Infrazione1 Then
            NumInfrazioni += 1
        End If
        If Infrazione2 Then
            NumInfrazioni += 1
        End If
        If Infrazione3 Then
            NumInfrazioni += 1
        End If

        If InfrazioneA1 Or InfrazioneA3 Or InfrazioneB Or InfrazioneC Then
            If NumInfrazioni = 3 Or Infrazione4 Or Infrazione5 Then
                Livello = 5
            ElseIf NumInfrazioni = 2 Then
                Livello = 3
            ElseIf NumInfrazioni = 1 Then
                Livello = 1
            End If
        End If

        Return Livello

    End Function

    ' CGO 8 (IDENTIFICAZIONE E REGISTRAZIONE ANIMALI) - Durata
    Public Function Durata_25(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer
        Dim Portata As Integer = Portata_25(SuperPiva, Piva, Regolamento_Cod)
        Dim Gravita As Integer = Gravita_25(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0
        Dim InfrazioneA1 As Boolean = False
        Dim InfrazioneA3 As Boolean = False
        Dim InfrazioneB As Boolean = False
        Dim InfrazioneC As Boolean = False
        Dim IncidenzaCapi As Double = 0
        Dim CapiNonConformi As Double = 0

        Dim DT As DataTable = LeggiRisposte(6, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    InfrazioneA1 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    InfrazioneA3 = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    InfrazioneB = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0010" Then
                    InfrazioneC = True
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0052" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                CapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            ElseIf DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                IncidenzaCapi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioneA1 Or InfrazioneA3 Or InfrazioneB Or InfrazioneC Then
            If IncidenzaCapi > 50 Or (InfrazioneB And InfrazioneC) Then
                Livello = 5
            ElseIf Portata = 1 And Gravita = 1 Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 9 (MALATTIE DEGLI ANIMALI) - Portata, Gravità e Durata
    Public Function Portata_Gravita_Durata17(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione As Boolean = False

        Dim DT As DataTable = LeggiRisposte(17, "", "0")
        Infrazione = DT.Rows.Count > 0

        DT.Dispose()
        DT = Nothing

        If Infrazione Then
            Livello = 5
        End If

        Return Livello

    End Function

    ' CGO 10 (PRODOTTI FITOSANITARI) - Portata
    Public Function Portata_7(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim Infrazione As Boolean = False

        Dim DT As DataTable = LeggiRisposte(7, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") = "0" Then
                If DT.Rows(i).Item("Punto_Numero") = "0020" Or
                    DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0040" Or
                    DT.Rows(i).Item("Punto_Numero") = "0050" Or
                    DT.Rows(i).Item("Punto_Numero") = "0060" Or
                    DT.Rows(i).Item("Punto_Numero") = "0070" Or
                    DT.Rows(i).Item("Punto_Numero") = "0110" Or
                    DT.Rows(i).Item("Punto_Numero") = "0111" Then
                    Infrazione = True
                End If
            End If

        Next

        DT.Dispose()
        DT = Nothing

        ' TODO: Calcolare livello corretto
        If Infrazione Then
            Livello = 3
        End If

        Return Livello

    End Function

    ' CGO 10 (PRODOTTI FITOSANITARI) - Gravità
    Public Function Gravita_7(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = Portata_7(SuperPiva, Piva, Regolamento_Cod)

        Return Livello

    End Function

    ' CGO 10 (PRODOTTI FITOSANITARI) - Durata
    Public Function Durata_7(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = Portata_7(SuperPiva, Piva, Regolamento_Cod)

        Return Livello

    End Function

    ' CGO 11 (BENESSERE DEGLI ANIMALI) - Portata
    Public Function Portata_21(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioniA As Integer = 0
        Dim InfrazioniB As Integer = 0
        Dim InfrazioniC As Integer = 0

        Dim DT As DataTable = LeggiRisposte(21, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") <> "-1" Then
                If DT.Rows(i).Item("Punto_Numero") = "0010" Or
                    DT.Rows(i).Item("Punto_Numero") = "0020" Or
                    DT.Rows(i).Item("Punto_Numero") = "0025" Or
                    DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0035" Or
                    DT.Rows(i).Item("Punto_Numero") = "0040" Or
                    DT.Rows(i).Item("Punto_Numero") = "0045" Or
                    DT.Rows(i).Item("Punto_Numero") = "0050" Or
                    DT.Rows(i).Item("Punto_Numero") = "0055" Or
                    DT.Rows(i).Item("Punto_Numero") = "0060" Or
                    DT.Rows(i).Item("Punto_Numero") = "0065" Then
                    Dim NumInfrazioni = 1
                    If DT.Rows(i).Item("Valore_2") <> "" AndAlso IsNumeric(DT.Rows(i).Item("Valore_2")) Then
                        NumInfrazioni = CInt(DT.Rows(i).Item("Valore_2"))
                    End If
                    If DT.Rows(i).Item("Valore") = "1" Then
                        InfrazioniA += NumInfrazioni
                    ElseIf DT.Rows(i).Item("Valore") = "2" Then
                        InfrazioniB += NumInfrazioni
                    ElseIf DT.Rows(i).Item("Valore") = "3" Then
                        InfrazioniC += NumInfrazioni
                    End If
                End If
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioniA > 0 Or InfrazioniB > 0 Or InfrazioniC > 0 Then
            If InfrazioniA > 4 Or InfrazioniB > 4 Or InfrazioniC > 0 Then
                Livello = 5
            ElseIf InfrazioniA >= 3 Or InfrazioniB >= 3 Then
                Livello = 3
            ElseIf InfrazioniA < 3 Or InfrazioniB < 3 Then
                Livello = 1
            End If
        End If

        Return Livello

    End Function

    ' CGO 11 (BENESSERE DEGLI ANIMALI) - Gravità
    Public Function Gravita_21(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = Portata_21(SuperPiva, Piva, Regolamento_Cod)

        Return Livello

    End Function

    ' CGO 11 (BENESSERE DEGLI ANIMALI) - Durata
    Public Function Durata_21(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Portata As Integer = Portata_21(SuperPiva, Piva, Regolamento_Cod)
        Dim Gravita As Integer = Gravita_21(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0
        Dim InfrazioniA As Integer = 0
        Dim InfrazioniB As Integer = 0
        Dim InfrazioniC As Integer = 0

        Dim DT As DataTable = LeggiRisposte(21, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") <> "-1" Then
                If DT.Rows(i).Item("Punto_Numero") = "0010" Or
                    DT.Rows(i).Item("Punto_Numero") = "0020" Or
                    DT.Rows(i).Item("Punto_Numero") = "0025" Or
                    DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0035" Or
                    DT.Rows(i).Item("Punto_Numero") = "0040" Or
                    DT.Rows(i).Item("Punto_Numero") = "0045" Or
                    DT.Rows(i).Item("Punto_Numero") = "0050" Or
                    DT.Rows(i).Item("Punto_Numero") = "0055" Or
                    DT.Rows(i).Item("Punto_Numero") = "0060" Or
                    DT.Rows(i).Item("Punto_Numero") = "0065" Then
                    Dim NumInfrazioni = 1
                    If DT.Rows(i).Item("Valore_2") <> "" AndAlso IsNumeric(DT.Rows(i).Item("Valore_2")) Then
                        NumInfrazioni = CInt(DT.Rows(i).Item("Valore_2"))
                    End If
                    If DT.Rows(i).Item("Valore") = "1" Then
                        InfrazioniA += NumInfrazioni
                    ElseIf DT.Rows(i).Item("Valore") = "2" Then
                        InfrazioniB += NumInfrazioni
                    ElseIf DT.Rows(i).Item("Valore") = "3" Then
                        InfrazioniC += NumInfrazioni
                    End If
                End If
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioniA > 0 Or InfrazioniB > 0 Or InfrazioniC > 0 Then
            If InfrazioniC > 0 Then
                Livello = 5
            ElseIf Portata = 1 And Gravita = 1 Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 12 (BENESSERE DEGLI ANIMALI) - Portata
    Public Function Portata_22(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioniA As Integer = 0
        Dim InfrazioniB As Integer = 0
        Dim InfrazioniC As Integer = 0

        Dim DT As DataTable = LeggiRisposte(22, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") <> "-1" Then
                If DT.Rows(i).Item("Punto_Numero") = "0005" Or
                    DT.Rows(i).Item("Punto_Numero") = "0010" Or
                    DT.Rows(i).Item("Punto_Numero") = "0020" Or
                    DT.Rows(i).Item("Punto_Numero") = "0025" Or
                    DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0035" Or
                    DT.Rows(i).Item("Punto_Numero") = "0040" Or
                    DT.Rows(i).Item("Punto_Numero") = "0045" Or
                    DT.Rows(i).Item("Punto_Numero") = "0051" Or
                    DT.Rows(i).Item("Punto_Numero") = "0055" Or
                    DT.Rows(i).Item("Punto_Numero") = "0060" Or
                    DT.Rows(i).Item("Punto_Numero") = "0065" Or
                    DT.Rows(i).Item("Punto_Numero") = "0070" Then
                    Dim NumInfrazioni = 1
                    If DT.Rows(i).Item("Valore_2") <> "" AndAlso IsNumeric(DT.Rows(i).Item("Valore_2")) Then
                        NumInfrazioni = CInt(DT.Rows(i).Item("Valore_2"))
                    End If
                    If DT.Rows(i).Item("Valore") = "1" Then
                        InfrazioniA += NumInfrazioni
                    ElseIf DT.Rows(i).Item("Valore") = "2" Then
                        InfrazioniB += NumInfrazioni
                    ElseIf DT.Rows(i).Item("Valore") = "3" Then
                        InfrazioniC += NumInfrazioni
                    End If
                End If
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioniA > 0 Or InfrazioniB > 0 Or InfrazioniC > 0 Then
            If InfrazioniA > 4 Or InfrazioniB > 4 Or InfrazioniC > 0 Then
                Livello = 5
            ElseIf InfrazioniA >= 3 Or InfrazioniB >= 3 Then
                Livello = 3
            ElseIf InfrazioniA < 3 Or InfrazioniB < 3 Then
                Livello = 1
            End If
        End If

        Return Livello

    End Function

    ' CGO 12 (BENESSERE DEGLI ANIMALI) - Gravità
    Public Function Gravita_22(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = Portata_22(SuperPiva, Piva, Regolamento_Cod)

        Return Livello

    End Function

    ' CGO 12 (BENESSERE DEGLI ANIMALI) - Durata
    Public Function Durata_22(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer
        Dim Portata As Integer = Portata_22(SuperPiva, Piva, Regolamento_Cod)
        Dim Gravita As Integer = Gravita_22(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0
        Dim InfrazioniA As Integer = 0
        Dim InfrazioniB As Integer = 0
        Dim InfrazioniC As Integer = 0

        Dim DT As DataTable = LeggiRisposte(22, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") <> "-1" Then
                If DT.Rows(i).Item("Punto_Numero") = "0005" Or
                    DT.Rows(i).Item("Punto_Numero") = "0010" Or
                    DT.Rows(i).Item("Punto_Numero") = "0020" Or
                    DT.Rows(i).Item("Punto_Numero") = "0025" Or
                    DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0035" Or
                    DT.Rows(i).Item("Punto_Numero") = "0040" Or
                    DT.Rows(i).Item("Punto_Numero") = "0045" Or
                    DT.Rows(i).Item("Punto_Numero") = "0051" Or
                    DT.Rows(i).Item("Punto_Numero") = "0055" Or
                    DT.Rows(i).Item("Punto_Numero") = "0060" Or
                    DT.Rows(i).Item("Punto_Numero") = "0065" Or
                    DT.Rows(i).Item("Punto_Numero") = "0070" Then
                    Dim NumInfrazioni = 1
                    If DT.Rows(i).Item("Valore_2") <> "" AndAlso IsNumeric(DT.Rows(i).Item("Valore_2")) Then
                        NumInfrazioni = CInt(DT.Rows(i).Item("Valore_2"))
                    End If
                    If DT.Rows(i).Item("Valore") = "1" Then
                        InfrazioniA += NumInfrazioni
                    ElseIf DT.Rows(i).Item("Valore") = "2" Then
                        InfrazioniB += NumInfrazioni
                    ElseIf DT.Rows(i).Item("Valore") = "3" Then
                        InfrazioniC += NumInfrazioni
                    End If
                End If
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioniA > 0 Or InfrazioniB > 0 Or InfrazioniC > 0 Then
            If InfrazioniC > 0 Then
                Livello = 5
            ElseIf Portata = 1 And Gravita = 1 Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

    ' CGO 13 (BENESSERE DEGLI ANIMALI) - Portata
    Public Function Portata_23(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = 0
        Dim InfrazioniA As Integer = 0
        Dim InfrazioniB As Integer = 0
        Dim InfrazioniC As Integer = 0

        Dim DT As DataTable = LeggiRisposte(23, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") <> "-1" Then
                If DT.Rows(i).Item("Punto_Numero") = "0005" Or
                    DT.Rows(i).Item("Punto_Numero") = "0010" Or
                    DT.Rows(i).Item("Punto_Numero") = "0020" Or
                    DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0035" Or
                    DT.Rows(i).Item("Punto_Numero") = "0040" Or
                    DT.Rows(i).Item("Punto_Numero") = "0045" Or
                    DT.Rows(i).Item("Punto_Numero") = "0060" Or
                    DT.Rows(i).Item("Punto_Numero") = "0065" Then
                    Dim NumInfrazioni = 1
                    If DT.Rows(i).Item("Valore_2") <> "" AndAlso IsNumeric(DT.Rows(i).Item("Valore_2")) Then
                        NumInfrazioni = CInt(DT.Rows(i).Item("Valore_2"))
                    End If
                    If DT.Rows(i).Item("Valore") = "1" Then
                        InfrazioniA += NumInfrazioni
                    ElseIf DT.Rows(i).Item("Valore") = "2" Then
                        InfrazioniB += NumInfrazioni
                    ElseIf DT.Rows(i).Item("Valore") = "3" Then
                        InfrazioniC += NumInfrazioni
                    End If
                End If
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioniA > 0 Or InfrazioniB > 0 Or InfrazioniC > 0 Then
            If InfrazioniA > 4 Or InfrazioniB > 4 Or InfrazioniC > 0 Then
                Livello = 5
            ElseIf InfrazioniA >= 3 Or InfrazioniB >= 3 Then
                Livello = 3
            ElseIf InfrazioniA < 3 Or InfrazioniB < 3 Then
                Livello = 1
            End If
        End If

        Return Livello

    End Function

    ' CGO 13 (BENESSERE DEGLI ANIMALI) - Gravità
    Public Function Gravita_23(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer

        Dim Livello As Integer = Portata_23(SuperPiva, Piva, Regolamento_Cod)

        Return Livello

    End Function

    ' CGO 13 (BENESSERE DEGLI ANIMALI) - Durata
    Public Function Durata_23(ByVal SuperPiva As String, ByVal Piva As String, ByVal Regolamento_Cod As Integer) As Integer
        Dim Portata As Integer = Portata_23(SuperPiva, Piva, Regolamento_Cod)
        Dim Gravita As Integer = Gravita_23(SuperPiva, Piva, Regolamento_Cod)

        Dim Livello As Integer = 0
        Dim InfrazioniA As Integer = 0
        Dim InfrazioniB As Integer = 0
        Dim InfrazioniC As Integer = 0

        Dim DT As DataTable = LeggiRisposte(23, "", "")

        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Valore") <> "-1" Then
                If DT.Rows(i).Item("Punto_Numero") = "0005" Or
                    DT.Rows(i).Item("Punto_Numero") = "0010" Or
                    DT.Rows(i).Item("Punto_Numero") = "0020" Or
                    DT.Rows(i).Item("Punto_Numero") = "0030" Or
                    DT.Rows(i).Item("Punto_Numero") = "0035" Or
                    DT.Rows(i).Item("Punto_Numero") = "0040" Or
                    DT.Rows(i).Item("Punto_Numero") = "0045" Or
                    DT.Rows(i).Item("Punto_Numero") = "0060" Or
                    DT.Rows(i).Item("Punto_Numero") = "0065" Then
                    Dim NumInfrazioni = 1
                    If DT.Rows(i).Item("Valore_2") <> "" AndAlso IsNumeric(DT.Rows(i).Item("Valore_2")) Then
                        NumInfrazioni = CInt(DT.Rows(i).Item("Valore_2"))
                    End If
                    If DT.Rows(i).Item("Valore") = "1" Then
                        InfrazioniA += NumInfrazioni
                    ElseIf DT.Rows(i).Item("Valore") = "2" Then
                        InfrazioniB += NumInfrazioni
                    ElseIf DT.Rows(i).Item("Valore") = "3" Then
                        InfrazioniC += NumInfrazioni
                    End If
                End If
            End If

        Next

        DT.Dispose()
        DT = Nothing

        If InfrazioniA > 0 Or InfrazioniB > 0 Or InfrazioniC > 0 Then
            If InfrazioniC > 0 Then
                Livello = 5
            ElseIf Portata = 1 And Gravita = 1 Then
                Livello = 1
            Else
                Livello = 3
            End If
        End If

        Return Livello

    End Function

End Class
