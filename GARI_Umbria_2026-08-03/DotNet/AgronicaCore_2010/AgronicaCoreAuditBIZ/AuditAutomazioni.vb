Imports AgronicaCoreAuditDAL
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json

Public Class AuditAutomazioni

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

    Public Function Portata_A1(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer


        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametriViolati As Integer = 0
        Dim DT As DataTable = LeggiRisposte(1, "", "1")

        ' Controllo solo i Casi particolari
        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0260" Then
                ' questo controllo serve per far valere sempre il livello + alto
                ' nel caso ci sia ambiguità tra i vari controlli
                If iLivello < 3 Then
                    iLivello = 3
                End If

                ' 08/10/2014 aggiunto il caso particolare 280 per la condizionalità 2014. Per le precedenti non dovrebbe dare fastidio
            ElseIf DT.Rows(i).Item("Punto_Numero") = "0270" Or
                   DT.Rows(i).Item("Punto_Numero") = "0280" Then
                iLivello = 5

            End If

        Next

        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Gravita_A1(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer

        Dim DT As DataTable = LeggiRisposte(1, "", "1")
        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim iLivelloCP As Integer = 0   ' livello dei Casi Particolari
        Dim nParametriViolati As Integer = 0

        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0160" Then
                nParametriViolati = 3

            Else
                If DT.Rows(i).Item("Punto_Numero") = "0150" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0170" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0180" Then

                    nParametriViolati += 1

                End If
            End If

            ' CASI PARTICOLARI
            If DT.Rows(i).Item("Punto_Numero") = "0260" Then
                ' questo controllo serve per far valere sempre il livello + alto
                ' nel caso ci sia ambiguità tra i vari controlli
                If iLivelloCP < 3 Then
                    iLivelloCP = 3
                End If


                ' 08/10/2014 aggiunto il caso particolare 280 per la condizionalità 2014. Per le precedenti non dovrebbe dare fastidio
            ElseIf DT.Rows(i).Item("Punto_Numero") = "0270" Or
                   DT.Rows(i).Item("Punto_Numero") = "0280" Then
                iLivelloCP = 5


            End If
            ' FINE CASI PARTICOLARI

        Next

        Select Case nParametriViolati
            Case 0
                iLivello = 0
            Case 1
                iLivello = 1
            Case 2
                iLivello = 3
            Case Else
                iLivello = 5
        End Select

        iLivello = Math.Max(iLivello, iLivelloCP)


        ' libero la memoria
        DT.Dispose()
        DT = Nothing

        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_A1(ByVal SuperPiva As String,
                              ByVal Piva As String,
                              ByVal Regolamento_Cod As Integer) As Integer

        Dim strErr As String = ""
        Dim iLivello As Integer = 3
        Dim iLivelloCP As Integer = 0       ' Livello dei casi Particolari
        Dim nParametriViolati As Integer = 0
        Dim DT As DataTable = LeggiRisposte(1, "", "1")
        Dim i As Integer

        If Regolamento_Cod = 1 Then
            For i = 0 To DT.Rows.Count - 1
                If DT.Rows(i).Item("Punto_Numero") = "0150" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0160" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0170" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0180" Then

                    nParametriViolati += 1

                End If

                ' CASI PARTICOLARI
                If DT.Rows(i).Item("Punto_Numero") = "0260" Then
                    ' questo controllo serve per far valere sempre il livello + alto
                    ' nel caso ci sia ambiguità tra i vari controlli
                    If iLivelloCP < 3 Then
                        iLivelloCP = 3
                    End If


                    ' 08/10/2014 aggiunto il caso particolare 280 per la condizionalità 2014. Per le precedenti non dovrebbe dare fastidio
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0270" Or
                       DT.Rows(i).Item("Punto_Numero") = "0280" Then
                    iLivelloCP = 5

                End If
                ' FINE CASI PARTICOLARI

            Next

            ' se sono presenti infrazioni per tutti gli impegni, il livello assume valore ALTO
            If nParametriViolati = 4 Then
                iLivello = 5
            End If

            iLivello = Math.Max(iLivello, iLivelloCP)

        ElseIf Regolamento_Cod >= 2 Then

            ' CONDIZIONALITA' 2010, 2011, 2012 e 2013

            For i = 0 To DT.Rows.Count - 1

                ' CASI PARTICOLARI
                If DT.Rows(i).Item("Punto_Numero") = "0260" Then
                    ' questo controllo serve per far valere sempre il livello + alto
                    ' nel caso ci sia ambiguità tra i vari controlli
                    If iLivelloCP < 3 Then
                        iLivelloCP = 3
                    End If

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0270" Then
                    iLivelloCP = 5
                End If
                ' FINE CASI PARTICOLARI

            Next

            Dim livPortata As Integer = 0

            Dim dr() As DataRow
            dr = DT.Select("Punto_Numero='0140' AND Valore = '1' ")
            If dr.Length > 0 Then
                livPortata = 5
            End If

            If livPortata = 5 Then
                iLivello = 5
            End If

            iLivello = Math.Max(iLivello, iLivelloCP)

        End If


        ' libero la memoria
        DT.Dispose()
        DT = Nothing

        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Portata_A2(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer


        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametriViolati As Integer = 0

        ' cerco quello con valore 0, ovvero quelli con valore 'NO' (appartenenti alla prima parte)
        Dim DT As DataTable = LeggiRisposte(2, "", "0")

        Dim i As Integer

        If Regolamento_Cod = 1 Then
            iLivello = 5
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    nParametriViolati += 1

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0025" Then
                    nParametriViolati += 2

                End If

            Next

            Select Case nParametriViolati
                Case 1
                    iLivello = 1
                Case Else
                    iLivello = 3
            End Select


        ElseIf Regolamento_Cod = 2 Or Regolamento_Cod = 3 Then
            ' CONDIZIONALITA' 2010 e 2011
            Dim nParametri As Integer = 0
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0055" Or
                   DT.Rows(i).Item("Punto_Numero") = "0026" Or
                   DT.Rows(i).Item("Punto_Numero") = "0050" Or
                   DT.Rows(i).Item("Punto_Numero") = "0030" Then

                    iLivello = 5
                    Exit For

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Or
                       DT.Rows(i).Item("Punto_Numero") = "0025" Then

                    nParametri += 1

                End If
            Next

            If nParametri = 2 Then
                iLivello = 5
            Else
                If iLivello < 5 Then
                    iLivello = 3
                End If
            End If


        ElseIf Regolamento_Cod >= 4 Then

            Dim Diffida As Boolean = False
            Dim Dispersione As Boolean = False

            ' CONDIZIONALITA' 2012 e 2013
            Dim nParametri As Integer = 0
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0055" Or
                   DT.Rows(i).Item("Punto_Numero") = "0026" Or
                   DT.Rows(i).Item("Punto_Numero") = "0027" Or
                   DT.Rows(i).Item("Punto_Numero") = "0050" Or
                   DT.Rows(i).Item("Punto_Numero") = "0030" Then

                    iLivello = 5
                    Exit For

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    Diffida = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0015" Or
                       DT.Rows(i).Item("Punto_Numero") = "0025" Then
                    Dispersione = True
                End If
            Next

            If iLivello <> 5 Then
                If Diffida And Dispersione Then
                    iLivello = 5
                ElseIf Diffida Or Dispersione Then
                    iLivello = 3
                Else
                    iLivello = 1
                End If
            End If

        End If

        ' libero la memoria
        DT.Dispose()
        DT = Nothing

        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Gravita_A2(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0

        ' cerco quello con valore 0, ovvero quelli con valore 'NO' (appartenenti alla prima parte)
        Dim DT As DataTable = LeggiRisposte(2, "", "0")

        Dim i As Integer

        If Regolamento_Cod = 1 Then
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    If iLivello < 1 Then
                        iLivello = 1
                    End If

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0031" Then
                    If iLivello < 3 Then
                        iLivello = 3
                    End If

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0030" Then
                    If iLivello < 5 Then
                        iLivello = 5
                    End If

                End If

            Next
        ElseIf Regolamento_Cod = 2 Or Regolamento_Cod = 3 Then
            ' CONDIZIONALITA' 2010 e 2011
            Dim nParametri As Integer = 0
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0055" Or
                   DT.Rows(i).Item("Punto_Numero") = "0026" Or
                   DT.Rows(i).Item("Punto_Numero") = "0050" Or
                   DT.Rows(i).Item("Punto_Numero") = "0030" Then

                    iLivello = 5
                    Exit For

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Or
                       DT.Rows(i).Item("Punto_Numero") = "0025" Then

                    nParametri += 1

                End If
            Next

            If nParametri = 2 Then
                iLivello = 5
            Else
                If iLivello < 5 Then
                    iLivello = 3
                End If
            End If

        ElseIf Regolamento_Cod >= 4 Then

            Dim Diffida As Boolean = False
            Dim Dispersione As Boolean = False

            ' CONDIZIONALITA' 2012 e 2013
            Dim nParametri As Integer = 0
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0055" Or
                   DT.Rows(i).Item("Punto_Numero") = "0026" Or
                   DT.Rows(i).Item("Punto_Numero") = "0027" Or
                   DT.Rows(i).Item("Punto_Numero") = "0050" Or
                   DT.Rows(i).Item("Punto_Numero") = "0030" Then

                    iLivello = 5
                    Exit For

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    Diffida = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0015" Or
                       DT.Rows(i).Item("Punto_Numero") = "0025" Then
                    Dispersione = True
                End If
            Next

            If iLivello <> 5 Then
                If Diffida And Dispersione Then
                    iLivello = 5
                ElseIf Diffida Or Dispersione Then
                    iLivello = 3
                Else
                    iLivello = 1
                End If
            End If

        End If

        ' libero la memoria
        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_A2(ByVal SuperPiva As String,
                              ByVal Piva As String,
                              ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 3

        ' cerco quello con valore 0, ovvero quelli con valore 'NO' (appartenenti alla prima parte)
        Dim DT As DataTable = LeggiRisposte(2, "", "0")

        Dim i As Integer

        If Regolamento_Cod = 1 Then
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0030" Or
                   DT.Rows(i).Item("Punto_Numero") = "0025" Then

                    iLivello = 5

                    Exit For

                End If

            Next

        ElseIf Regolamento_Cod = 2 Or Regolamento_Cod = 3 Then
            ' CONDIZIONALITA' 2010 e 2011
            Dim nParametri As Integer = 0
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0055" Or
                   DT.Rows(i).Item("Punto_Numero") = "0026" Or
                   DT.Rows(i).Item("Punto_Numero") = "0050" Or
                   DT.Rows(i).Item("Punto_Numero") = "0030" Then

                    iLivello = 5
                    Exit For

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Or
                       DT.Rows(i).Item("Punto_Numero") = "0025" Then

                    nParametri += 1

                End If
            Next

            If nParametri = 2 Then
                iLivello = 5
            Else
                If iLivello < 5 Then
                    iLivello = 3
                End If
            End If

        ElseIf Regolamento_Cod >= 4 Then

            Dim Diffida As Boolean = False
            Dim Dispersione As Boolean = False

            ' CONDIZIONALITA' 2012 e 2013
            Dim nParametri As Integer = 0
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0055" Or
                   DT.Rows(i).Item("Punto_Numero") = "0026" Or
                   DT.Rows(i).Item("Punto_Numero") = "0027" Or
                   DT.Rows(i).Item("Punto_Numero") = "0050" Or
                   DT.Rows(i).Item("Punto_Numero") = "0030" Then

                    iLivello = 5
                    Exit For

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then
                    Diffida = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0015" Or
                       DT.Rows(i).Item("Punto_Numero") = "0025" Then
                    Dispersione = True
                End If
            Next

            If iLivello <> 5 Then
                If Diffida And Dispersione Then
                    iLivello = 5
                ElseIf Diffida Or Dispersione Then
                    iLivello = 3
                Else
                    iLivello = 1
                End If
            End If

        End If

        ' libero la memoria
        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Portata_A3(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 5
        Dim nParametriViolati As Integer = 0

        ' condizionalità 2010
        Dim bSup_0 As Boolean = False
        Dim bSup_20 As Boolean = False
        Dim bSup_30 As Boolean = False
        Dim bDatiIdent As Boolean = False
        Dim bRegUtilizzazione As Boolean = False
        Dim bAutorizzazione As Boolean = False
        Dim bExtra As Boolean = False
        Dim bCasiParticolari As Boolean = False

        Dim DT As DataTable = LeggiRisposte(3, "", "1")

        Dim i As Integer

        If Regolamento_Cod = 1 Then
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    nParametriViolati += 1

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0025" Then
                    nParametriViolati += 2

                End If

            Next

            Select Case nParametriViolati
                Case 1
                    iLivello = 1
                Case Else
                    iLivello = 3
            End Select

        ElseIf Regolamento_Cod >= 2 Then
            'CONDIZIONALITA' 2010, 2011, 2012, 2013
            For i = 0 To DT.Rows.Count - 1

                Select Case DT.Rows(i).Item("Punto_Numero")
                    Case "0310"
                        bSup_0 = True
                    Case "0320"
                        bSup_20 = True
                    Case "0330"
                        bSup_30 = True
                    Case "410"
                        bDatiIdent = True
                    Case "420"
                        bRegUtilizzazione = True
                    Case "0430"
                        bAutorizzazione = True
                    Case "0340"
                        bExtra = True
                    Case "0610", "0620", "0640"
                        bCasiParticolari = True
                End Select


            Next

            If bExtra Or bSup_30 Or bCasiParticolari Or
               (bAutorizzazione And bDatiIdent) Or
               (bAutorizzazione And bRegUtilizzazione) Or
               (bRegUtilizzazione And bDatiIdent) Then

                iLivello = 5

            ElseIf bRegUtilizzazione Or bAutorizzazione Or bDatiIdent Or bSup_20 Or
                   (bSup_0 And bDatiIdent) Or
                   (bSup_0 And bRegUtilizzazione) Then

                iLivello = 3

            Else

                iLivello = 1

            End If

        End If

        ' libero la memoria
        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Gravita_A3(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0

        ' condizionalità 2010
        Dim bSup_0 As Boolean = False
        Dim bSup_20 As Boolean = False
        Dim bSup_30 As Boolean = False
        Dim bDatiIdent As Boolean = False
        Dim bRegUtilizzazione As Boolean = False
        Dim bAutorizzazione As Boolean = False
        Dim bExtra As Boolean = False
        Dim bCasiParticolari As Boolean = False

        Dim DT As DataTable = LeggiRisposte(3, "", "1")


        Dim i As Integer

        If Regolamento_Cod = 1 Then

            iLivello = 1

            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0430" Then
                    If iLivello < 3 Then
                        iLivello = 3
                    End If
                End If

            Next


        ElseIf Regolamento_Cod >= 2 Then
            'CONDIZIONALITA' 2010, 2011, 2012, 2013
            For i = 0 To DT.Rows.Count - 1

                Select Case DT.Rows(i).Item("Punto_Numero")
                    Case "0310"
                        bSup_0 = True
                    Case "0320"
                        bSup_20 = True
                    Case "0330"
                        bSup_30 = True
                    Case "410"
                        bDatiIdent = True
                    Case "420"
                        bRegUtilizzazione = True
                    Case "0430"
                        bAutorizzazione = True
                    Case "0340"
                        bExtra = True
                    Case "0610", "0620", "0640"
                        bCasiParticolari = True
                End Select


            Next

            If bExtra Or bSup_30 Or bCasiParticolari Or
               (bAutorizzazione And bDatiIdent) Or
               (bAutorizzazione And bRegUtilizzazione) Or
               (bRegUtilizzazione And bDatiIdent) Then

                iLivello = 5

            ElseIf bRegUtilizzazione Or bAutorizzazione Or bDatiIdent Or bSup_20 Or
                   (bSup_0 And bDatiIdent) Or
                   (bSup_0 And bRegUtilizzazione) Then

                iLivello = 3

            Else

                iLivello = 1

            End If

        End If

        ' libero la memoria
        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_A3(ByVal SuperPiva As String,
                              ByVal Piva As String,
                              ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer

        ' condizionalità 2010
        Dim bSup_0 As Boolean = False
        Dim bSup_20 As Boolean = False
        Dim bSup_30 As Boolean = False
        Dim bDatiIdent As Boolean = False
        Dim bRegUtilizzazione As Boolean = False
        Dim bAutorizzazione As Boolean = False
        Dim bExtra As Boolean = False
        Dim bCasiParticolari As Boolean = False

        Dim DT As DataTable = LeggiRisposte(3, "", "1")

        Dim i As Integer

        If Regolamento_Cod = 1 Then
            ' inizializzazione
            iLivello = 3

            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0030" Or
                   DT.Rows(i).Item("Punto_Numero") = "0025" Then

                    iLivello = 5

                    Exit For

                End If

            Next
        ElseIf Regolamento_Cod >= 2 Then

            'CONDIZIONALITA' 2010, 2011, 2012, 2013
            For i = 0 To DT.Rows.Count - 1

                Select Case DT.Rows(i).Item("Punto_Numero")
                    Case "0310"
                        bSup_0 = True
                    Case "0320"
                        bSup_20 = True
                    Case "0330"
                        bSup_30 = True
                    Case "410"
                        bDatiIdent = True
                    Case "420"
                        bRegUtilizzazione = True
                    Case "0430"
                        bAutorizzazione = True
                    Case "0340"
                        bExtra = True
                    Case "0610", "0620", "0640"
                        bCasiParticolari = True
                End Select


            Next

            If bExtra Or bSup_30 Or bCasiParticolari Or
               (bAutorizzazione And bDatiIdent) Or
               (bAutorizzazione And bRegUtilizzazione) Or
               (bRegUtilizzazione And bDatiIdent) Then

                iLivello = 5

            ElseIf bRegUtilizzazione Or bAutorizzazione Or bDatiIdent Or bSup_20 Or
                   (bSup_0 And bDatiIdent) Or
                   (bSup_0 And bRegUtilizzazione) Then

                iLivello = 3

            Else

                iLivello = 1

            End If

        End If

        ' libero la memoria
        DT.Dispose()
        DT = Nothing


        Return iLivello
    End Function

    '#########################################################################################################
    Public Function Portata_A4(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametriViolati As Integer = 0

        ' controlli i valori della prima parte, quindi cerco il valore ''
        Dim DT As DataTable = LeggiRisposte(4, "", "")

        ' Controllo solo i Casi particolari
        Dim i As Integer

        If Regolamento_Cod = 1 Then

            For i = 0 To DT.Rows.Count - 1
                If DT.Rows(i).Item("Valore") <> "-1" Then

                    If DT.Rows(i).Item("Punto_Numero") = "0090" Or
                       DT.Rows(i).Item("Punto_Numero") = "0100" Or
                       DT.Rows(i).Item("Punto_Numero") = "0460" Then
                        ' questo controllo serve per far valere sempre il livello + alto
                        ' nel caso ci sia ambiguità tra i vari controlli
                        If iLivello < 3 Then
                            iLivello = 3
                        End If

                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0120" Or
                           DT.Rows(i).Item("Punto_Numero") = "0060" Or
                           DT.Rows(i).Item("Punto_Numero") = "0070" Or
                           DT.Rows(i).Item("Punto_Numero") = "0440" Or
                           DT.Rows(i).Item("Punto_Numero") = "0470" Then
                        iLivello = 5

                    End If
                End If
            Next

        ElseIf Regolamento_Cod >= 2 Then
            'CONDIZIONALITA' 2010, 2011, 2012, 2013
            Dim Classe As Integer = 0
            For i = 0 To DT.Rows.Count - 1
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
            Next

            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Valore") <> "-1" Then
                    If DT.Rows(i).Item("Punto_Numero") = "0241" Or
                       DT.Rows(i).Item("Punto_Numero") = "0070" Or
                       DT.Rows(i).Item("Punto_Numero") = "0090" Or
                       DT.Rows(i).Item("Punto_Numero") = "0100" Then
                        ' questo controllo serve per far valere sempre il livello + alto
                        ' nel caso ci sia ambiguità tra i vari controlli
                        If iLivello < 3 Then
                            iLivello = 3
                        End If

                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0120" Or
                           DT.Rows(i).Item("Punto_Numero") = "0060" Then
                        iLivello = 5

                    ElseIf (DT.Rows(i).Item("Punto_Numero") = "0020" Or
                           DT.Rows(i).Item("Punto_Numero") = "0030") And
                           (Classe = 2 Or Classe = 3) Then
                        iLivello = 5
                    End If
                End If
            Next

            If Regolamento_Cod >= 5 Then

                Dim dr As DataRow()

                ' CASI PARTICOLARI
                ' CASO 1
                dr = DT.Select("Punto_Numero='0060'")
                If dr.Length > 0 Then
                    If dr(0).Item("Valore") = "1" And Classe <= 3 Then
                        iLivello = 5
                    End If
                End If

                dr = DT.Select("Punto_Numero='0440'")
                If dr.Length > 0 Then
                    If dr(0).Item("Valore") = "1" Then
                        iLivello = 5
                    End If
                End If

                'CASO 2
                dr = DT.Select("Punto_Numero='0020'")
                If dr.Length > 0 Then
                    If dr(0).Item("Valore") = "1" And (Classe = 2 Or Classe = 3) Then
                        iLivello = 5
                    End If
                End If
                dr = DT.Select("Punto_Numero='0030'")
                If dr.Length > 0 Then
                    If dr(0).Item("Valore") = "1" And (Classe = 2 Or Classe = 3) Then
                        iLivello = 5
                    End If
                End If
                dr = DT.Select("Punto_Numero='0460'")
                If dr.Length > 0 Then
                    If dr(0).Item("Valore") = "1" Then
                        iLivello = 5
                    End If
                End If

                'CASO 3
                dr = DT.Select("Punto_Numero='0465'")
                If dr.Length > 0 Then
                    If dr(0).Item("Valore") = "1" Then
                        iLivello = 5
                    End If
                End If

                'CASO 4
                dr = DT.Select("Punto_Numero='0470'")
                If dr.Length > 0 Then
                    If dr(0).Item("Valore") = "1" And iLivello <= 3 Then
                        iLivello = 3
                    End If
                End If

                'CASO 5 solo per la 2014
                dr = DT.Select("Punto_Numero='0480'")
                If dr.Length > 0 Then
                    If dr(0).Item("Valore") = "1" Then
                        iLivello = 5
                    End If
                End If

            End If


        End If

        ' libero la memoria
        DT.Dispose()
        DT = Nothing

        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Gravita_A4(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer

        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim iCatDimensionale As Integer = 0
        Dim iCatAzoto As Integer = 0
        Dim iClasse As Integer = 0
        Dim DT As DataTable

        If Regolamento_Cod = 1 Then

            DT = LeggiRisposte(4, "0380", "")

            If DT.Rows.Count > 0 Then

                If Not IsDBNull(DT.Rows(0).Item("Valore")) AndAlso IsNumeric(DT.Rows(0).Item("Valore")) Then

                    iCatDimensionale = CInt(DT.Rows(0).Item("Valore"))
                Else

                    Return iLivello

                End If

            Else

                Return iLivello

            End If


            If iCatDimensionale = 0 Then

                DT.Dispose()
                DT = Nothing

                DT = LeggiRisposte(4, "", "1")

                For i = 0 To DT.Rows.Count - 1

                    ' CLASSE DIMENSIONALE
                    If DT.Rows(i).Item("Punto_Numero") = "0290" Then
                        If iClasse < 1 Then
                            iClasse = 1
                        End If

                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0300" Then
                        If iClasse < 2 Then
                            iClasse = 2
                        End If

                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0310" Then
                        If iClasse < 3 Then
                            iClasse = 3
                        End If

                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0320" Then
                        If iClasse < 4 Then
                            iClasse = 4
                        End If

                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0330" Then
                        If iClasse < 5 Then
                            iClasse = 5
                        End If

                        'ZVN
                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0350" Then

                        If iCatAzoto < 1 Then
                            iCatAzoto = 1
                        End If

                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0360" Then

                        If iCatAzoto < 2 Then
                            iCatAzoto = 2
                        End If

                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0370" Then

                        If iCatAzoto < 3 Then
                            iCatAzoto = 3
                        End If
                    End If

                Next

                If (iClasse = 1 And iCatAzoto = 1) Or (iClasse = 2 And iCatAzoto = 1) Or (iClasse = 1 And iCatAzoto = 2) Then
                    iCatDimensionale = 1

                ElseIf (iClasse = 3 And iCatAzoto = 1) Or (iClasse = 2 And iCatAzoto = 2) Or (iClasse = 1 And iCatAzoto = 3) Then
                    iCatDimensionale = 2

                ElseIf (iClasse = 4 And iCatAzoto = 1) Or (iClasse = 3 And iCatAzoto = 2) Or (iClasse = 2 And iCatAzoto = 3) Then
                    iCatDimensionale = 3

                    'Amoroso 07/2024 Commentato per segnalazione SonarQube: verificare se sistemare la condizione che risulta identica a quella sopra,
                    'considerando che la funzione in se sembra non essere richiamata
                    'ElseIf (iClasse = 4 And iCatAzoto = 1) Or (iClasse = 3 And iCatAzoto = 2) Or (iClasse = 2 And iCatAzoto = 3) Then
                    '    iCatDimensionale = 4

                ElseIf (iClasse = 5 And iCatAzoto = 2) Or (iClasse = 4 And iCatAzoto = 3) Then
                    iCatDimensionale = 5

                ElseIf (iClasse = 5 And iCatAzoto = 3) Then
                    iCatDimensionale = 6

                End If


            End If


            ' libero la memoria
            DT.Dispose()
            DT = Nothing

            If iCatDimensionale = 6 Then

                iLivello = 5

            Else

                DT = LeggiRisposte(4, "", "1")

                Dim i As Integer
                For i = 0 To DT.Rows.Count - 1

                    ' portata bassa o media
                    If DT.Rows(i).Item("Punto_Numero") = "0250" Or
                       DT.Rows(i).Item("Punto_Numero") = "0260" Then

                        If iCatDimensionale <= 2 Then
                            If iLivello < 1 Then
                                iLivello = 1
                            End If

                        ElseIf iCatDimensionale <= 5 Then
                            If iLivello < 3 Then
                                iLivello = 3
                            End If
                        End If

                        ' portata alta
                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0270" Then

                        If iCatDimensionale <= 2 Then
                            If iLivello < 3 Then
                                iLivello = 3
                            End If
                        ElseIf iCatDimensionale <= 5 Then
                            iLivello = 5
                        End If

                        ' casi particolari
                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0460" Then

                        If iLivello < 3 Then
                            iLivello = 3
                        End If

                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0440" Or
                           DT.Rows(i).Item("Punto_Numero") = "0470" Then

                        iLivello = 5


                    End If

                Next

                ' libero la memoria
                DT.Dispose()
                DT = Nothing

            End If

        ElseIf Regolamento_Cod = 2 Or Regolamento_Cod = 3 Or Regolamento_Cod = 4 Then
            'CONDIZIONALITA' 2010, 2011, 2012

            DT = LeggiRisposte(4, "", "1")

            ' Calcolo la classe
            Dim Classe As Integer = 0
            For i = 0 To DT.Rows.Count - 1
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
            Next

            ' Calcolo la portata
            Dim Portata As Integer = 0
            Dim dr As DataRow()
            ' livello basso di portata
            dr = DT.Select("Punto_Numero='0250'")
            ' il controllo che il valore sia 1 è già nel where della query (vale anche per gli altri livelli di portata qui sotto)
            If dr.Length > 0 Then
                Portata = 1
            End If
            ' livello medio di portata
            dr = DT.Select("Punto_Numero='0260'")
            If dr.Length > 0 Then
                Portata = 3
            End If
            ' livello alto di portata
            dr = DT.Select("Punto_Numero='0270'")
            If dr.Length > 0 Then
                Portata = 5
            End If


            If (Portata = 1 Or Portata = 3) And (Classe = 1 Or Classe = 2) Then
                iLivello = 1

            ElseIf ((Portata = 1 Or Portata = 3) And (Classe = 3 Or Classe = 4)) Or
                   (Portata = 5 And (Classe = 1 Or Classe = 2)) Then
                iLivello = 3

            ElseIf ((Portata = 5) And (Classe = 3 Or Classe = 4)) Or
                   (Classe = 5) Then
                iLivello = 5
            End If

            ' CASI PARTICOLARI
            dr = DT.Select("Punto_Numero='0060'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" And Classe <= 3 Then
                    iLivello = 5
                End If
            End If

            dr = DT.Select("Punto_Numero='0020'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" And (Classe = 2 Or Classe = 3) Then
                    iLivello = 5
                End If
            End If

            dr = DT.Select("Punto_Numero='0030'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" And (Classe = 2 Or Classe = 3) Then
                    iLivello = 5
                End If
            End If

        ElseIf Regolamento_Cod >= 5 Then
            'CONDIZIONALITA' 2013 e 2014


            DT = LeggiRisposte(4, "", "1")

            ' Calcolo la classe
            Dim Classe As Integer = 0
            For i = 0 To DT.Rows.Count - 1
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
            Next


            ' Calcolo la portata
            Dim Portata As Integer = 0
            Dim dr As DataRow()
            ' livello basso di portata
            dr = DT.Select("Punto_Numero='0250'")
            ' il controllo che il valore sia 1 è già nel where della query (vale anche per gli altri livelli di portata qui sotto)
            If dr.Length > 0 Then
                Portata = 1
            End If
            ' livello medio di portata
            dr = DT.Select("Punto_Numero='0260'")
            If dr.Length > 0 Then
                Portata = 3
            End If
            ' livello alto di portata
            dr = DT.Select("Punto_Numero='0270'")
            If dr.Length > 0 Then
                Portata = 5
            End If


            If (Portata = 1 And (Classe = 1 Or Classe = 2)) Or
               (Portata = 3 And Classe = 1) Then

                iLivello = 1

            ElseIf (Portata = 5 And (Classe = 1 Or Classe = 2)) Or
                   (Portata = 3 And (Classe = 2 Or Classe = 3)) Or
                   (Portata = 1 And (Classe = 3 Or Classe = 4)) Then

                iLivello = 3

            ElseIf (Portata = 5 And (Classe = 3 Or Classe = 4)) Or
                   (Portata = 3 And Classe = 3) Or
                   (Classe = 5) Then

                iLivello = 5

            End If


            ' CASI PARTICOLARI
            ' CASO 1
            dr = DT.Select("Punto_Numero='0060'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" And Classe <= 3 Then
                    iLivello = 5
                End If
            End If

            dr = DT.Select("Punto_Numero='0440'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" Then
                    iLivello = 5
                End If
            End If

            'CASO 2
            dr = DT.Select("Punto_Numero='0020'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" And (Classe = 2 Or Classe = 3) Then
                    iLivello = 5
                End If
            End If
            dr = DT.Select("Punto_Numero='0030'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" And (Classe = 2 Or Classe = 3) Then
                    iLivello = 5
                End If
            End If
            dr = DT.Select("Punto_Numero='0460'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" Then
                    iLivello = 5
                End If
            End If

            'CASO 3
            dr = DT.Select("Punto_Numero='0465'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" Then
                    iLivello = 5
                End If
            End If

            'CASO 4
            dr = DT.Select("Punto_Numero='0470'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" And iLivello <= 3 Then
                    iLivello = 3
                End If
            End If

            'CASO 5 solo per la 2014
            dr = DT.Select("Punto_Numero='0480'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" Then
                    iLivello = 5
                End If
            End If


        End If

        ' libero la memoria


        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_A4(ByVal SuperPiva As String,
                              ByVal Piva As String,
                              ByVal Regolamento_Cod As Integer) As Integer





        Dim strErr As String = ""
        Dim iLivello As Integer = 3

        Dim DT As DataTable = LeggiRisposte(4, "", "1")

        If Regolamento_Cod = 1 Then

            For i = 0 To DT.Rows.Count - 1

                ' portata bassa o media
                If DT.Rows(i).Item("Punto_Numero") = "0460" Then

                    If iLivello < 3 Then
                        iLivello = 3
                    End If

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0440" Or
                       DT.Rows(i).Item("Punto_Numero") = "0470" Then

                    iLivello = 5

                End If

            Next


        ElseIf Regolamento_Cod >= 2 Then
            ' CONDIZIONALITA' 2010, 2011, 2012, 2013

            ' Livello di default impostato a medio
            iLivello = 3

            ' Calcolo la classe
            Dim Classe As Integer = 0
            For i = 0 To DT.Rows.Count - 1
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
            Next

            Dim dr As DataRow()
            ' CASI PARTICOLARI
            ' CASO 1
            dr = DT.Select("Punto_Numero='0060'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" And Classe <= 3 Then
                    iLivello = 5
                End If
            End If

            dr = DT.Select("Punto_Numero='0440'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" Then
                    iLivello = 5
                End If
            End If

            'CASO 2
            dr = DT.Select("Punto_Numero='0020'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" And (Classe = 2 Or Classe = 3) Then
                    iLivello = 5
                End If
            End If
            dr = DT.Select("Punto_Numero='0030'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" And (Classe = 2 Or Classe = 3) Then
                    iLivello = 5
                End If
            End If
            dr = DT.Select("Punto_Numero='0460'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" Then
                    iLivello = 5
                End If
            End If

            'CASO 3
            dr = DT.Select("Punto_Numero='0465'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" Then
                    iLivello = 5
                End If
            End If

            'CASO 4 (solo ne 2013)
            dr = DT.Select("Punto_Numero='0470'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" And iLivello <= 3 Then
                    iLivello = 3
                End If
            End If

            'CASO 5 solo per la 2014
            dr = DT.Select("Punto_Numero='0480'")
            If dr.Length > 0 Then
                If dr(0).Item("Valore") = "1" Then
                    iLivello = 5
                End If
            End If

            ' effetti extra-aziendali
            dr = DT.Select("Punto_Numero='0242'")
            If dr.Length > 0 And (Classe = 2 Or Classe = 3) Then
                iLivello = 5
            End If

        End If



        ' libero la memoria
        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Portata_A5(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametriViolati As Integer = 0

        Dim DT As DataTable = LeggiRisposte(5, "", "1")

        ' Controllo solo i Casi particolari
        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0260" Then
                ' questo controllo serve per far valere sempre il livello + alto
                ' nel caso ci sia ambiguità tra i vari controlli
                If iLivello < 3 Then
                    iLivello = 3
                End If

            ElseIf DT.Rows(i).Item("Punto_Numero") = "0270" Then
                iLivello = 5

            End If

        Next

        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Gravita_A5(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim iLivelloCP As Integer = 0   ' livello dei Casi Particolari
        Dim nParametriViolati As Integer = 0

        Dim DT As DataTable = LeggiRisposte(5, "", "1")


        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0160" Then
                nParametriViolati = 3

            Else
                If DT.Rows(i).Item("Punto_Numero") = "0150" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0170" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0180" Then

                    nParametriViolati += 1

                End If
            End If

            ' CASI PARTICOLARI
            If DT.Rows(i).Item("Punto_Numero") = "0260" Then
                ' questo controllo serve per far valere sempre il livello + alto
                ' nel caso ci sia ambiguità tra i vari controlli
                If iLivelloCP < 3 Then
                    iLivelloCP = 3
                End If

            ElseIf DT.Rows(i).Item("Punto_Numero") = "0270" Then
                iLivelloCP = 5

            End If
            ' FINE CASI PARTICOLARI

        Next

        Select Case nParametriViolati
            Case 0
                iLivello = 0
            Case 1
                iLivello = 1
            Case 2
                iLivello = 3
            Case Else
                iLivello = 5
        End Select

        iLivello = Math.Max(iLivello, iLivelloCP)


        ' libero la memoria
        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_A5(ByVal SuperPiva As String,
                              ByVal Piva As String,
                              ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 3
        Dim iLivelloCP As Integer = 0       ' Livello dei casi Particolari
        Dim nParametriViolati As Integer = 0

        Dim DT As DataTable = LeggiRisposte(5, "", "1")

        Dim i As Integer

        If Regolamento_Cod = 1 Then


            For i = 0 To DT.Rows.Count - 1
                If DT.Rows(i).Item("Punto_Numero") = "0150" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0160" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0170" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0180" Then

                    nParametriViolati += 1

                End If

                ' CASI PARTICOLARI
                If DT.Rows(i).Item("Punto_Numero") = "0260" Then
                    ' questo controllo serve per far valere sempre il livello + alto
                    ' nel caso ci sia ambiguità tra i vari controlli
                    If iLivelloCP < 3 Then
                        iLivelloCP = 3
                    End If

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0270" Then
                    iLivelloCP = 5
                End If
                ' FINE CASI PARTICOLARI

            Next

            ' se sono presenti infrazioni per tutti gli impegni, il livello assume valore ALTO
            If nParametriViolati = 4 Then
                iLivello = 5
            End If

            iLivello = Math.Max(iLivello, iLivelloCP)

        ElseIf Regolamento_Cod >= 2 Then

            For i = 0 To DT.Rows.Count - 1

                ' CASI PARTICOLARI
                If DT.Rows(i).Item("Punto_Numero") = "0260" Then
                    ' questo controllo serve per far valere sempre il livello + alto
                    ' nel caso ci sia ambiguità tra i vari controlli
                    If iLivelloCP < 3 Then
                        iLivelloCP = 3
                    End If

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0270" Then
                    iLivelloCP = 5
                End If
                ' FINE CASI PARTICOLARI

            Next

            Dim livPortata As Integer = 0

            Dim dr() As DataRow
            dr = DT.Select("Punto_Numero='0140' AND Valore = '1' ")
            If dr.Length > 0 Then
                livPortata = 5
            End If

            If livPortata = 5 Then
                iLivello = 5
            End If

            iLivello = Math.Max(iLivello, iLivelloCP)

        End If

        ' libero la memoria
        DT.Dispose()
        DT = Nothing

        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Portata_A6(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nCapiNonConformi As Double = 0
        Dim percCapiNonConformi As Double = 0

        Dim CasiParticolari As Boolean = False

        Dim DT As DataTable = LeggiRisposte(6, "", "")

        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0052" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then

                nCapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))

            ElseIf DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                percCapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))

            End If

            If Regolamento_Cod = 4 Then
                If DT.Rows(i).Item("Punto_Numero") = "0470" Or DT.Rows(i).Item("Punto_Numero") = "0480" Then
                    CasiParticolari = True
                End If
            End If

        Next

        If CasiParticolari Then
            iLivello = 3
        End If

        iLivello = 5

        If Regolamento_Cod >= 2 Then

            ' CONDIZIONALITA' 2010 e 2011
            ' cambia solo il minore uguale al posto del minore
            If percCapiNonConformi <= 5 Then
                If nCapiNonConformi <= 10 Then
                    iLivello = 1
                End If

            ElseIf percCapiNonConformi > 5 And percCapiNonConformi <= 10 Then
                If nCapiNonConformi <= 20 Then
                    iLivello = 3
                End If

            End If

        Else
            If percCapiNonConformi < 5 Then
                If nCapiNonConformi <= 10 Then
                    iLivello = 1
                End If

            ElseIf percCapiNonConformi >= 5 And percCapiNonConformi < 10 Then
                If nCapiNonConformi <= 20 Then
                    iLivello = 3
                End If

            End If

        End If


        ' Libero la memoria
        DT.Dispose()
        DT = Nothing




        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Gravita_A6(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim iLivelloCP As Integer = 0   ' livello dei Casi Particolari
        Dim nParametriViolati As Integer = 0
        Dim CasiParticolari As Boolean = False

        Dim DT As DataTable = LeggiRisposte(6, "", "1")


        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0170" Then
                nParametriViolati = 3

            Else
                If DT.Rows(i).Item("Punto_Numero") = "0150" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0160" Then

                    nParametriViolati += 1

                End If
            End If

            If Regolamento_Cod >= 4 Then
                If DT.Rows(i).Item("Punto_Numero") = "0470" Or DT.Rows(i).Item("Punto_Numero") = "0480" Then
                    CasiParticolari = True
                End If
            End If

        Next

        Select Case nParametriViolati
            Case 0
                iLivello = 0
            Case 1
                iLivello = 1
            Case 2
                iLivello = 3
            Case Else
                iLivello = 5
        End Select

        If CasiParticolari Then
            iLivello = 3
        End If


        ' libero la memoria
        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_A6(ByVal SuperPiva As String,
                              ByVal Piva As String,
                              ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""

        Dim CasiParticolari As Boolean = False

        ' La durata viene stabilita normalmente a livello medio
        Dim iLivello As Integer = 3

        Dim percCapiNonConformi As Integer = 0

        Dim DT As DataTable = LeggiRisposte(6, "", "")

        If Regolamento_Cod >= 5 Then
            Dim livPortata As Integer = 0
            Dim livGravita As Integer = 0

            ' livello di portata
            Dim dr() As DataRow
            dr = DT.Select("Punto_Numero='0120' AND Valore = '1' ")
            If dr.Length > 0 Then
                livPortata = 1
            End If

            ' livello di gravità
            dr = DT.Select("Punto_Numero='0200' AND Valore = '1' ")
            If dr.Length > 0 Then
                livGravita = 1
            End If

            If livPortata = 1 And livGravita = 1 Then
                iLivello = 1
            End If

        End If

        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                percCapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            End If

            If Regolamento_Cod >= 4 Then
                If IsNumeric(DT.Rows(i).Item("Valore")) AndAlso DT.Rows(i).Item("Valore") = "1" Then
                    If DT.Rows(i).Item("Punto_Numero") = "0470" Or DT.Rows(i).Item("Punto_Numero") = "0480" Then
                        CasiParticolari = True
                    End If
                End If
            End If

        Next

        If CasiParticolari Then
            iLivello = 3
        End If

        ' Quando si rilevano non conformità riguardante oltre il 50% dei casi, la durata è fissata a livello ALTO
        If percCapiNonConformi > 50 Then
            iLivello = 5
        End If



        ' Libero la memoria
        DT.Dispose()
        DT = Nothing



        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Portata_A7(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nCapiNonConformi As Double = 0
        Dim percCapiNonConformi As Double = 0

        ' Disp_Cod=24
        Dim DT As DataTable = LeggiRisposte(24, "", "")

        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0052" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then

                nCapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))

            ElseIf DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                percCapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))

            End If

        Next

        iLivello = 5

        If Regolamento_Cod >= 2 Then

            ' CONDIZIONALITA' 2010, 2011, 2012, 2013
            ' cambia solo il minore uguale al posto del minore
            If percCapiNonConformi <= 5 Then
                If nCapiNonConformi <= 5 Then
                    iLivello = 1
                End If

            ElseIf percCapiNonConformi > 5 And percCapiNonConformi <= 10 Then
                If nCapiNonConformi <= 10 Then
                    iLivello = 3
                End If

            End If

        Else

            If percCapiNonConformi < 5 Then
                If nCapiNonConformi <= 5 Then
                    iLivello = 1
                End If

            ElseIf percCapiNonConformi >= 5 And percCapiNonConformi < 10 Then
                If nCapiNonConformi <= 10 Then
                    iLivello = 3
                End If

            End If

        End If

        ' Libero la memoria
        DT.Dispose()
        DT = Nothing




        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Gravita_A7(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim iLivelloCP As Integer = 0   ' livello dei Casi Particolari
        Dim nParametriViolati As Integer = 0

        Dim DT As DataTable = LeggiRisposte(24, "", "1")


        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0180" Then
                nParametriViolati = 4

            Else
                If DT.Rows(i).Item("Punto_Numero") = "0150" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0160" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0170" Then

                    nParametriViolati += 1

                End If
            End If

        Next

        Select Case nParametriViolati
            Case 0
                iLivello = 0
            Case 1
                iLivello = 1
            Case 2
                iLivello = 3
            Case Else
                iLivello = 5
        End Select


        ' libero la memoria
        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_A7(ByVal SuperPiva As String,
                              ByVal Piva As String,
                              ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""

        ' La durata viene stabilita normalmente a livello medio
        Dim iLivello As Integer = 3

        Dim percCapiNonConformi As Integer = 0

        Dim DT As DataTable = LeggiRisposte(24, "", "")

        If Regolamento_Cod >= 5 Then
            Dim livPortata As Integer = 0
            Dim livGravita As Integer = 0

            ' livello di portata
            Dim dr() As DataRow
            dr = DT.Select("Punto_Numero='0120' AND Valore = '1' ")
            If dr.Length > 0 Then
                livPortata = 1
            End If

            ' livello di gravità
            dr = DT.Select("Punto_Numero='0200' AND Valore = '1' ")
            If dr.Length > 0 Then
                livGravita = 1
            End If

            If livPortata = 1 And livGravita = 1 Then
                iLivello = 1
            End If

        End If

        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                percCapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            End If

        Next

        ' Quando si rilevano non conformità riguardante oltre il 50% dei casi, la durata è fissata a livello ALTO
        If percCapiNonConformi > 50 Then
            iLivello = 5
        End If

        ' Libero la memoria
        DT.Dispose()
        DT = Nothing



        Return iLivello

    End Function



    '#########################################################################################################
    Public Function Portata_A8_bis(ByVal SuperPiva As String,
                                   ByVal Piva As String,
                                   ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nCapiNonConformi As Double = 0
        Dim percCapiNonConformi As Double = 0

        ' Disp_Cod=24
        Dim DT As DataTable = LeggiRisposte(24, "", "")

        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0052" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then

                nCapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))

            ElseIf DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                percCapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))

            End If

        Next

        iLivello = 5

        If Regolamento_Cod >= 2 Then

            ' CONDIZIONALITA' 2010
            ' cambia solo il minore uguale al posto del minore
            If percCapiNonConformi <= 5 Then
                If nCapiNonConformi <= 20 Then
                    iLivello = 1
                End If

            ElseIf percCapiNonConformi > 5 And percCapiNonConformi <= 10 Then
                If nCapiNonConformi <= 40 Then
                    iLivello = 3
                End If

            End If

        Else

            If percCapiNonConformi < 5 Then
                If nCapiNonConformi <= 20 Then
                    iLivello = 1
                End If

            ElseIf percCapiNonConformi >= 5 And percCapiNonConformi < 10 Then
                If nCapiNonConformi <= 40 Then
                    iLivello = 3
                End If

            End If

        End If

        ' Libero la memoria
        DT.Dispose()
        DT = Nothing




        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Gravita_A8_bis(ByVal SuperPiva As String,
                                   ByVal Piva As String,
                                   ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim iLivelloCP As Integer = 0   ' livello dei Casi Particolari
        Dim nParametriViolati As Integer = 0

        Dim DT As DataTable = LeggiRisposte(25, "", "1")


        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0180" Then
                nParametriViolati = 4

            Else
                If DT.Rows(i).Item("Punto_Numero") = "0150" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0160" _
                   Or DT.Rows(i).Item("Punto_Numero") = "0170" Then

                    nParametriViolati += 1

                End If
            End If

        Next

        Select Case nParametriViolati
            Case 0
                iLivello = 0
            Case 1
                iLivello = 1
            Case 2
                iLivello = 3
            Case Else
                iLivello = 5
        End Select


        ' libero la memoria
        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_A8_bis(ByVal SuperPiva As String,
                                  ByVal Piva As String,
                                  ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""

        ' La durata viene stabilita normalmente a livello medio
        Dim iLivello As Integer = 3

        Dim percCapiNonConformi As Integer = 0

        Dim DT As DataTable = LeggiRisposte(24, "", "")

        If Regolamento_Cod >= 5 Then
            Dim livPortata As Integer = 0
            Dim livGravita As Integer = 0

            ' livello di portata
            Dim dr() As DataRow
            dr = DT.Select("Punto_Numero='0120' AND Valore = '1' ")
            If dr.Length > 0 Then
                livPortata = 1
            End If

            ' livello di gravità
            dr = DT.Select("Punto_Numero='0200' AND Valore = '1' ")
            If dr.Length > 0 Then
                livGravita = 1
            End If

            If livPortata = 1 And livGravita = 1 Then
                iLivello = 1
            End If

        End If

        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0053" AndAlso IsNumeric(DT.Rows(i).Item("Valore")) Then
                percCapiNonConformi = CDbl(DT.Rows(i).Item("Valore").ToString.Replace(".", ","))
            End If

        Next

        ' Quando si rilevano non conformità riguardante oltre il 50% dei casi, la durata è fissata a livello ALTO
        If percCapiNonConformi > 50 Then
            iLivello = 5
        End If

        ' Libero la memoria
        DT.Dispose()
        DT = Nothing



        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Portata_B9(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer



        Dim strErr As String = ""
        Dim iLivello As Integer = 1

        Dim DT As DataTable = LeggiRisposte(7, "", "1")

        Dim i As Integer

        If Regolamento_Cod >= 5 Then
            Dim dr() As DataRow
            ' nel dt ci sono quelli con valore =1
            dr = DT.Select("Punto_Numero='0050' OR Punto_Numero='0110' ")
            If dr.Length = 1 Then
                iLivello = 1
            ElseIf dr.Length = 2 Then
                iLivello = 3
            Else
                iLivello = 5
            End If

        End If

        If Regolamento_Cod >= 2 Then
            ' CONDIZIONALITA' 2010 e 2011 (il 0390 e 0395 sono solo nella 2011, nella 2010 non ci sono, quindi vengono ignorati nel calcolo)
            ' CONDIZIONALITA' 2010 e 2011 (il 0402 e 0403 sono solo nella 2012, quindi vengono ignorati nel calcolo delle precendenti)
            ' considero solo i casi particolari
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0390" Then
                    If iLivello < 1 Then
                        iLivello = 1
                    End If
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0340" Or
                   DT.Rows(i).Item("Punto_Numero") = "0370" Or
                   DT.Rows(i).Item("Punto_Numero") = "0395" Or
                   DT.Rows(i).Item("Punto_Numero") = "0402" Then
                    If iLivello < 3 Then
                        iLivello = 3
                    End If
                End If


                If DT.Rows(i).Item("Punto_Numero") = "0350" Or
                   DT.Rows(i).Item("Punto_Numero") = "0380" Or
                   DT.Rows(i).Item("Punto_Numero") = "0403" Then
                    iLivello = 5
                End If
            Next
        ElseIf Regolamento_Cod = 1 Then

            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                    iLivello = 5
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0340" Then
                    If iLivello < 3 Then
                        iLivello = 3
                    End If
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0350" Then
                    iLivello = 5
                End If
            Next
        End If

        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Gravita_B9(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim iLivelloCP As Integer = 0   ' livello dei Casi Particolari
        Dim nParametriViolati_Liv1 As Integer = 0
        Dim nParametriViolati_Liv2 As Integer = 0
        Dim nParametriViolati_Liv3 As Integer = 0

        Dim bParam1 As Boolean = False
        Dim bParam2 As Boolean = False
        Dim bParam3 As Boolean = False
        Dim bParam4 As Boolean = False
        Dim bParam5 As Boolean = False
        Dim bParam6 As Boolean = False
        Dim bParam7 As Boolean = False
        Dim bParam8 As Boolean = False      ' condizionalità 2010 (regolamento 2)

        Dim bParam9 As Boolean = False      ' condizionalità 2011 (regolamento 3)
        Dim bParam10 As Boolean = False
        Dim bParam11 As Boolean = False

        Dim DT As DataTable = LeggiRisposte(7, "", "1")
        Dim i As Integer



        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0190" Then
                bParam1 = True
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0200" Then
                bParam2 = True
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0210" Then
                bParam3 = True
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0220" Then
                bParam4 = True
            End If


            If Regolamento_Cod = 5 Then

                ' il 220 non c'è

                If DT.Rows(i).Item("Punto_Numero") = "0230" Then
                    bParam4 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0235" Then
                    bParam5 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0240" Then
                    bParam6 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0245" Then
                    bParam7 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0250" Then
                    bParam8 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0260" Then
                    bParam9 = True
                End If

            ElseIf Regolamento_Cod = 3 Or Regolamento_Cod = 4 Then
                ' Condizionalità 2011
                If DT.Rows(i).Item("Punto_Numero") = "0230" Then
                    bParam5 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0233" Then
                    bParam6 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0236" Then
                    bParam7 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0250" Then
                    bParam8 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0260" Then
                    bParam9 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0265" Then
                    bParam10 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0267" Then
                    bParam11 = True
                End If

            Else
                ' condizionalità 2008 e 2010 (regolamento_cod 1 e 2)
                If DT.Rows(i).Item("Punto_Numero") = "0230" Then
                    bParam5 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0250" Then
                    bParam6 = True
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0260" Then
                    bParam7 = True
                End If

                ' SOLO CONDIZIONALITA' 2010
                If DT.Rows(i).Item("Punto_Numero") = "0270" Then
                    bParam8 = True
                End If


            End If

            If Regolamento_Cod >= 3 Then
                ' CONDIZIONALITA' 2011, 2012, 2013, 2014
                If DT.Rows(i).Item("Punto_Numero") = "0390" Then
                    If iLivelloCP < 1 Then
                        iLivelloCP = 1
                    End If
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0340" Or
                   DT.Rows(i).Item("Punto_Numero") = "0370" Or
                   DT.Rows(i).Item("Punto_Numero") = "0395" Or
                   DT.Rows(i).Item("Punto_Numero") = "0402" Then
                    If iLivelloCP < 3 Then
                        iLivelloCP = 3
                    End If
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0350" Or
                   DT.Rows(i).Item("Punto_Numero") = "0380" Or
                   DT.Rows(i).Item("Punto_Numero") = "0403" Then
                    iLivelloCP = 5
                End If

            ElseIf Regolamento_Cod = 2 Then
                ' CONDIZIONALITA' 2010            
                If DT.Rows(i).Item("Punto_Numero") = "0350" Then
                    If iLivelloCP < 3 Then
                        iLivelloCP = 3
                    End If
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0380" Then
                    iLivelloCP = 5
                End If
            Else
                If DT.Rows(i).Item("Punto_Numero") = "0340" Then
                    If iLivelloCP < 3 Then
                        iLivelloCP = 3
                    End If
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0350" Then
                    iLivelloCP = 5
                End If
            End If

        Next

        If Regolamento_Cod >= 5 Then

            If (bParam1 And bParam2) Or bParam3 Or
               (bParam4 And bParam5) Or (bParam5 And bParam6) Or (bParam4 And bParam6) Or
               (bParam7 Or bParam8 Or bParam9) Then
                iLivello = 5

            ElseIf bParam1 Or (bParam5 Or bParam6) Then
                iLivello = 3

            ElseIf (bParam2 Or bParam4) Then
                iLivello = 1

            End If

        ElseIf Regolamento_Cod = 3 Then

            If (bParam4 Or bParam5 Or bParam6 Or bParam7) Or
               (bParam1 And bParam2) Or (bParam1 And bParam3) Or (bParam2 And bParam3) Or
               (bParam10 And (bParam8 Or bParam9)) Then
                iLivello = 5

            ElseIf (bParam2 Or bParam3) Or
                   (bParam8 And bParam9) Or (bParam10) Then
                iLivello = 3

            ElseIf bParam1 Or (bParam8 Or bParam9) Then
                iLivello = 1

            End If

        ElseIf Regolamento_Cod = 2 Then

            If (bParam4 Or bParam5) Or
               (bParam1 And bParam2) Or (bParam1 And bParam3) Or (bParam2 And bParam3) Or
               (bParam8 And (bParam6 Or bParam7)) Then
                iLivello = 5

            ElseIf (bParam2 Or bParam3) Or
                   (bParam6 And bParam7) Or (bParam8) Then
                iLivello = 3
            ElseIf bParam1 Or (bParam6 Or bParam7) Then
                iLivello = 1
            End If


        Else

            If bParam4 Or bParam5 Or
               (bParam1 And bParam2) Or (bParam2 And bParam3) Or (bParam1 And bParam3) Then
                iLivello = 5

            ElseIf (bParam6 And bParam7) Or
                   (bParam2) Or (bParam3) Then

                iLivello = 3

            ElseIf bParam1 Or
                   (bParam6 And Not bParam7) Or
                   (Not bParam6 And bParam7) Then

                iLivello = 1

            End If

        End If    ' regolamento_cod

        ' hanno la precedenza i Casi Particolari
        If iLivelloCP > 0 Then
            iLivello = iLivelloCP
        End If


        ' libero la memoria
        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_B9(ByVal SuperPiva As String,
                              ByVal Piva As String,
                              ByVal Regolamento_Cod As Integer) As Integer



        Dim strErr As String = ""
        Dim iLivello As Integer = 3

        Dim DT As DataTable = LeggiRisposte(7, "", "1")

        Dim i As Integer


        If Regolamento_Cod >= 2 Then
            ' CONDIZIONALITA' 2010 e 2011 (il 0390 e 0395 sono solo nella 2011, nella 2010 non ci sono, quindi vengono ignorati nel calcolo)
            ' CONDIZIONALITA' 2010 e 2012 (il 0402 e 0403 sono solo nella 2012, nelle precedenti vengono ignorati)
            ' considero solo i casi particolari
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0390" Then
                    If iLivello < 1 Then
                        iLivello = 1
                    End If
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0340" Or
                   DT.Rows(i).Item("Punto_Numero") = "0370" Or
                   DT.Rows(i).Item("Punto_Numero") = "0395" Or
                   DT.Rows(i).Item("Punto_Numero") = "0402" Then
                    If iLivello < 3 Then
                        iLivello = 3
                    End If
                End If


                If DT.Rows(i).Item("Punto_Numero") = "0350" Or
                   DT.Rows(i).Item("Punto_Numero") = "0380" Or
                   DT.Rows(i).Item("Punto_Numero") = "0403" Then
                    iLivello = 5
                End If
            Next
        Else
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0340" Then
                    If iLivello < 3 Then
                        iLivello = 3
                    End If
                End If

                If DT.Rows(i).Item("Punto_Numero") = "0350" Then
                    iLivello = 5
                End If
            Next
        End If


        DT.Dispose()
        DT = Nothing




        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Gravita_Norma11(ByVal SuperPiva As String,
                                    ByVal Piva As String,
                                    ByVal Regolamento_Cod As Integer) As Integer



        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim DT As DataTable = LeggiRisposte(9, "", "1")

        ' conto il numero di parametri violati
        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0170" Or
               DT.Rows(i).Item("Punto_Numero") = "0180" Or
               DT.Rows(i).Item("Punto_Numero") = "0190" Then

                nParametri += 1
            End If

        Next

        ' imposto il livello
        Select Case nParametri
            Case 1
                iLivello = 1
            Case 2
                iLivello = 3
            Case 3
                iLivello = 5
            Case Else
                iLivello = 0
        End Select

        DT.Dispose()
        DT = Nothing




        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_Norma21(ByVal SuperPiva As String,
                                   ByVal Piva As String,
                                   ByVal Regolamento_Cod As Integer) As Integer


        Dim strErr As String = ""
        Dim iLivello As Integer = 3

        ' Nei casi di infrazione all’interno delle aree SIC e ZPS l’indicatore di durata sarà fissato al livello ALTO
        Dim DLL_AD As New Audit_Profilazione_R
        If DLL_AD.AuditZona(SuperPiva, Piva, TipiEnumerativi.enum_Zone.SIC, "", objParametri) Or
           DLL_AD.AuditZona(SuperPiva, Piva, TipiEnumerativi.enum_Zone.ZPS, "", objParametri) Then

            iLivello = 5

        End If

        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Gravita_Norma31(ByVal SuperPiva As String,
                                    ByVal Piva As String,
                                    ByVal Regolamento_Cod As Integer) As Integer



        Dim strErr As String = ""
        Dim iLivello As Integer = 0

        Dim bImpegno1Violato As Boolean = False
        Dim bImpegno2Violato As Boolean = False

        Dim DT As DataTable = LeggiRisposte(11, "", "0")

        ' conto il numero di parametri violati
        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0020" Or
               DT.Rows(i).Item("Punto_Numero") = "0030" Or
               DT.Rows(i).Item("Punto_Numero") = "0040" Then

                bImpegno1Violato = True
            End If


            If DT.Rows(i).Item("Punto_Numero") = "0060" Then

                bImpegno2Violato = True
            End If

        Next

        If bImpegno1Violato And bImpegno2Violato Then
            iLivello = 5
        Else
            iLivello = 3
        End If


        DT.Dispose()
        DT = Nothing



        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Portata_Norma41(ByVal SuperPiva As String,
                                    ByVal Piva As String,
                                    ByVal Regolamento_Cod As Integer) As Integer



        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim DT As DataTable = LeggiRisposte(12, "", "0")

        ' conto il numero di parametri violati
        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0010" Or
               DT.Rows(i).Item("Punto_Numero") = "0040" Or
               DT.Rows(i).Item("Punto_Numero") = "0050" Then

                iLivello = 5
            End If

        Next


        DT.Dispose()
        DT = Nothing



        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Gravita_Norma41(ByVal SuperPiva As String,
                                    ByVal Piva As String,
                                    ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim DT As DataTable = LeggiRisposte(12, "", "1")

        ' conto il numero di parametri violati
        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0090" Or
               DT.Rows(i).Item("Punto_Numero") = "0100" Or
               DT.Rows(i).Item("Punto_Numero") = "0110" Then

                nParametri += 1

            End If

            '
            If DT.Rows(i).Item("Punto_Numero") = "0120" Or
               DT.Rows(i).Item("Punto_Numero") = "0130" Or
               DT.Rows(i).Item("Punto_Numero") = "0140" Then

                nParametri += 3

            End If

        Next

        ' imposto il livello
        Select Case nParametri
            Case 0
                iLivello = 0
            Case 1
                iLivello = 1
            Case 2
                iLivello = 3
            Case Else
                iLivello = 5
        End Select

        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_Norma41(ByVal SuperPiva As String,
                                   ByVal Piva As String,
                                   ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim iLivelloPortata As Integer = 0
        Dim iLivelloGravita As Integer = 0

        Dim DT As DataTable = LeggiRisposte(12, "", "1")

        ' conto il numero di parametri violati
        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0060" And iLivelloPortata < 1 Then
                iLivelloPortata = 1
            End If
            If DT.Rows(i).Item("Punto_Numero") = "0070" And iLivelloPortata < 3 Then
                iLivelloPortata = 3
            End If
            If DT.Rows(i).Item("Punto_Numero") = "0080" And iLivelloPortata < 5 Then
                iLivelloPortata = 5
            End If

            '
            If DT.Rows(i).Item("Punto_Numero") = "0150" And iLivelloGravita < 1 Then
                iLivelloGravita = 1
            End If
            If DT.Rows(i).Item("Punto_Numero") = "0160" And iLivelloGravita < 3 Then
                iLivelloGravita = 3
            End If
            If DT.Rows(i).Item("Punto_Numero") = "0170" And iLivelloGravita < 5 Then
                iLivelloGravita = 5
            End If

        Next

        If iLivelloPortata = 1 And iLivelloGravita = 1 Then
            iLivello = 1
        ElseIf iLivelloPortata = 5 And iLivelloGravita = 5 Then
            iLivello = 5
        Else
            iLivello = 3
        End If

        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Gravita_Norma42(ByVal SuperPiva As String,
                                    ByVal Piva As String,
                                    ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim DT As DataTable = LeggiRisposte(13, "", "1")

        ' conto il numero di parametri violati
        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0360" Or
               DT.Rows(i).Item("Punto_Numero") = "0370" Or
               DT.Rows(i).Item("Punto_Numero") = "0380" Or
               DT.Rows(i).Item("Punto_Numero") = "0390" Or
               DT.Rows(i).Item("Punto_Numero") = "0400" Or
               DT.Rows(i).Item("Punto_Numero") = "0410" Then

                nParametri += 1

            End If

        Next

        ' imposto il livello
        Select Case nParametri
            Case 0
                iLivello = 0
            Case 1
                iLivello = 1
            Case 2
                iLivello = 3
            Case Else
                iLivello = 5
        End Select

        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Gravita_Norma43(ByVal SuperPiva As String,
                                    ByVal Piva As String,
                                    ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim DT As DataTable = LeggiRisposte(14, "", "1")

        ' conto il numero di parametri violati
        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0130" Or
               DT.Rows(i).Item("Punto_Numero") = "0140" Or
               DT.Rows(i).Item("Punto_Numero") = "0150" Then

                nParametri += 1

            End If

            If DT.Rows(i).Item("Punto_Numero") = "0160" Then

                nParametri += 3

            End If

        Next

        ' imposto il livello
        Select Case nParametri
            Case 0
                iLivello = 0
            Case 1
                iLivello = 1
            Case 2
                iLivello = 3
            Case Else
                iLivello = 5
        End Select

        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Durata_Norma43(ByVal SuperPiva As String,
                                   ByVal Piva As String,
                                   ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim DT As DataTable = LeggiRisposte(14, "0020", "0")

        iLivello = 3

        ' Il punto '0020' se violato provoca un livello alto di durata (estirpazione di olivi)
        If DT.Rows.Count > 0 Then
            iLivello = 5
        End If

        DT.Dispose()
        DT = Nothing


        Return iLivello

    End Function




    '#########################################################################################################
    Public Function Portata_Fertilizzanti(ByVal SuperPiva As String,
                                          ByVal Piva As String,
                                          ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametriViolati As Integer = 0

        Dim DT As DataTable = LeggiRisposte(27, "", "")

        ' Controllo solo i Casi particolari
        Dim i As Integer

        If Regolamento_Cod = 1 Then
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Valore") = "0" Then

                    If DT.Rows(i).Item("Punto_Numero") = "0150" Or
                       DT.Rows(i).Item("Punto_Numero") = "0160" Then
                        ' questo controllo serve per far valere sempre il livello + alto
                        ' nel caso ci sia ambiguità tra i vari controlli
                        If iLivello < 3 Then
                            iLivello = 3
                        End If

                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0120" Or
                           DT.Rows(i).Item("Punto_Numero") = "0130" Then

                        iLivello = 5

                    End If
                End If
            Next
        ElseIf Regolamento_Cod >= 2 Then

            ' CONDIZIONALITA' 2010 e successive
            ' fino al punto 220 sono della prima parte, quindi controllo che il valore sia 0, dopo controllo che il valore sia 1
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Valore") = "0" Then
                    If DT.Rows(i).Item("Punto_Numero") = "0210" Or
                       DT.Rows(i).Item("Punto_Numero") = "0130" Or
                       DT.Rows(i).Item("Punto_Numero") = "0150" Or
                       DT.Rows(i).Item("Punto_Numero") = "0160" Then
                        ' questo controllo serve per far valere sempre il livello + alto
                        ' nel caso ci sia ambiguità tra i vari controlli
                        If iLivello < 3 Then
                            iLivello = 3
                        End If

                    ElseIf DT.Rows(i).Item("Punto_Numero") = "0120" Or
                           DT.Rows(i).Item("Punto_Numero") = "0220" Or
                           DT.Rows(i).Item("Punto_Numero") = "0180" Then

                        iLivello = 5

                    End If
                End If
            Next

            ' CASI PARTICOLARI

            Dim dr() As DataRow
            dr = DT.Select("Punto_Numero='0550' AND Valore = '1' ")
            If dr.Length > 0 Then
                iLivello = 5
            End If

            dr = DT.Select("Punto_Numero='0560' AND Valore = '1' ")
            If dr.Length > 0 Then
                iLivello = 5
            End If

            ' aggiunto nella condizionalità 2014
            If Regolamento_Cod = 6 Then
                dr = DT.Select("Punto_Numero='0570' AND Valore = '1' ")
                If dr.Length > 0 Then
                    If iLivello < 3 Then
                        iLivello = 3
                    End If

                End If
            End If


        End If

        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Gravita_Fertilizzanti(ByVal SuperPiva As String,
                                          ByVal Piva As String,
                                          ByVal Regolamento_Cod As Integer) As Integer

        ' Il calcolo coincide per i regolamenti 1 e 2 tranne per i casi particolari



        Dim strErr As String = ""

        Dim iLivello As Integer = 0

        Dim iPortata As Integer = 0

        Dim iCatDimensionale As Integer = 0


        Dim DT As DataTable = LeggiRisposte(27, "", "1")

        ' calcolo la categoria dimensionale e la portata
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0450" Then
                If iCatDimensionale < 1 Then
                    iCatDimensionale = 1
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0460" Then
                If iCatDimensionale < 2 Then
                    iCatDimensionale = 2
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0470" Then
                If iCatDimensionale < 3 Then
                    iCatDimensionale = 3
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0480" Then
                If iCatDimensionale < 4 Then
                    iCatDimensionale = 4
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0490" Then
                iCatDimensionale = 5
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0400" Then
                If iPortata < 1 Then
                    iPortata = 1
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0410" Then
                If iPortata < 3 Then
                    iPortata = 3
                End If
            End If

            If DT.Rows(i).Item("Punto_Numero") = "0420" Then
                iPortata = 5
            End If

        Next

        If iCatDimensionale <> 0 And iPortata <> 0 Then

            If iCatDimensionale <= 2 Then

                If iPortata <= 3 Then
                    iLivello = 1
                Else
                    iLivello = 3
                End If

            Else

                If iPortata <= 3 Then
                    iLivello = 3
                Else
                    iLivello = 5
                End If

            End If

        End If

        Dim dr() As DataRow

        ' CASI PARTICOLARI
        If Regolamento_Cod >= 2 Then

            dr = DT.Select("Punto_Numero='0550'")
            If dr.Length > 0 Then
                iLivello = 5
            End If

            dr = DT.Select("Punto_Numero='0560'")
            If dr.Length > 0 Then
                iLivello = 5
            End If

        End If

        ' aggiunto nella condizionalità 2014
        If Regolamento_Cod = 6 Then
            dr = DT.Select("Punto_Numero='0570' AND Valore = '1' ")
            If dr.Length > 0 Then
                If iLivello < 3 Then
                    iLivello = 3
                End If

            End If
        End If



        ' libero la memoria
        DT.Dispose()
        DT = Nothing




        ' libero la memoria


        Return iLivello

    End Function



    '#########################################################################################################
    Public Function Durata_Fertilizzanti(ByVal SuperPiva As String,
                                         ByVal Piva As String,
                                         ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 3

        Dim DT As DataTable = LeggiRisposte(27, "", "0")

        ' Controllo solo i Casi particolari
        Dim i As Integer

        If Regolamento_Cod = 1 Then


            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0250" Or
                   DT.Rows(i).Item("Punto_Numero") = "0260" Or
                   DT.Rows(i).Item("Punto_Numero") = "0270" Then
                    iLivello = 5
                End If

            Next


        ElseIf Regolamento_Cod >= 2 Then
            ' CONDIZIONALITA' 2010 e successive
            Dim dr() As DataRow
            ' effetti extra-aziendali
            dr = DT.Select("Punto_Numero='0220'")
            If dr.Length > 0 Then
                iLivello = 5
            End If

            ' CASI PARTICOLARI
            dr = DT.Select("Punto_Numero='0550'")
            If dr.Length > 0 Then
                iLivello = 5
            End If

            dr = DT.Select("Punto_Numero='0560'")
            If dr.Length > 0 Then
                iLivello = 5
            End If

            If Regolamento_Cod = 6 Then
                dr = DT.Select("Punto_Numero='0570'")
                If dr.Length > 0 Then
                    If iLivello < 3 Then
                        iLivello = 3
                    End If

                End If
            End If

        End If

        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Livelli_Formulati(ByVal SuperPiva As String,
                                      ByVal Piva As String,
                                      ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametriViolati As Integer = 0

        Dim DT As DataTable = LeggiRisposte(28, "", "0")

        ' Non cambia nulla nella condizionalità 2010

        ' Controllo solo i Casi particolari
        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0020" Then
                ' questo controllo serve per far valere sempre il livello + alto
                ' nel caso ci sia ambiguità tra i vari controlli
                If iLivello < 3 Then
                    iLivello = 3
                End If

            ElseIf DT.Rows(i).Item("Punto_Numero") = "0040" Then

                iLivello = 5

            End If

        Next

        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Gravita_Norma1(ByVal SuperPiva As String,
                                   ByVal Piva As String,
                                   ByVal Regolamento_Cod As Integer) As Integer



        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim DT As DataTable = LeggiRisposte(9, "", "1")

        ' conto il numero di parametri violati
        Dim i As Integer

        ' If Regolamento_Cod = 3 Then
        If Regolamento_Cod >= 3 Then

            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0330" Or
                   DT.Rows(i).Item("Punto_Numero") = "0335" Or
                   DT.Rows(i).Item("Punto_Numero") = "0340" Or
                   DT.Rows(i).Item("Punto_Numero") = "0345" Or
                   DT.Rows(i).Item("Punto_Numero") = "0350" Or
                   DT.Rows(i).Item("Punto_Numero") = "0360" Or
                   DT.Rows(i).Item("Punto_Numero") = "0370" Or
                   DT.Rows(i).Item("Punto_Numero") = "0380" Then

                    nParametri += 1
                End If

            Next

        ElseIf Regolamento_Cod = 2 Then

            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0330" Or
                   DT.Rows(i).Item("Punto_Numero") = "0340" Or
                   DT.Rows(i).Item("Punto_Numero") = "0350" Or
                   DT.Rows(i).Item("Punto_Numero") = "0360" Or
                   DT.Rows(i).Item("Punto_Numero") = "0370" Or
                   DT.Rows(i).Item("Punto_Numero") = "0380" Or
                   DT.Rows(i).Item("Punto_Numero") = "0390" Then

                    nParametri += 1
                End If
            Next
        End If

        ' imposto il livello
        Select Case nParametri
            Case 0
                iLivello = 0
            Case 1
                iLivello = 3
            Case Else
                iLivello = 5
        End Select

        DT.Dispose()
        DT = Nothing

        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_Norma1(ByVal SuperPiva As String,
                                  ByVal Piva As String,
                                  ByVal Regolamento_Cod As Integer) As Integer

        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim DT As DataTable = LeggiRisposte(9, "", "1")

        ' Calcolo la portata
        Dim Portata As Integer = 0
        Dim dr As DataRow()
        ' livello basso di portata
        dr = DT.Select("Punto_Numero='0300'")
        ' il controllo che il valore sia 1 è già nel where della query (vale anche per gli altri livelli di portata qui sotto)
        If dr.Length > 0 Then
            Portata = 1
        End If
        ' livello medio di portata
        dr = DT.Select("Punto_Numero='0310'")
        If dr.Length > 0 Then
            Portata = 3
        End If
        ' livello alto di portata
        dr = DT.Select("Punto_Numero='0320'")
        If dr.Length > 0 Then
            Portata = 5
        End If

        ' conto il numero di parametri violati
        Dim i As Integer

        If Regolamento_Cod >= 3 Then
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0345" Or
                   DT.Rows(i).Item("Punto_Numero") = "0350" Or
                   DT.Rows(i).Item("Punto_Numero") = "0360" Or
                   DT.Rows(i).Item("Punto_Numero") = "0370" Or
                   DT.Rows(i).Item("Punto_Numero") = "0380" Or
                   DT.Rows(i).Item("Punto_Numero") = "0390" Then

                    nParametri += 1
                End If

            Next

            Dim pto111 As Boolean = False
            Dim pto114 As Boolean = False
            ' livello alto di portata pto 1.1.1
            dr = DT.Select("Punto_Numero='0330'")
            If dr.Length > 0 Then
                pto111 = True
            End If

            ' livello alto di portata pto 1.1.4
            dr = DT.Select("Punto_Numero='0335'")
            If dr.Length > 0 Then
                pto114 = True
            End If

            If nParametri >= 3 Or (Portata = 5 And pto111 And pto114) Then
                iLivello = 5
            ElseIf nParametri = 2 Or (Portata = 3 And pto111 And pto114) Then
                iLivello = 3
            ElseIf nParametri = 1 Or (Portata = 1 And pto111 And pto114) Then
                iLivello = 1
            End If

        ElseIf Regolamento_Cod = 2 Then
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0330" Or
                   DT.Rows(i).Item("Punto_Numero") = "0350" Or
                   DT.Rows(i).Item("Punto_Numero") = "0360" Or
                   DT.Rows(i).Item("Punto_Numero") = "0370" Or
                   DT.Rows(i).Item("Punto_Numero") = "0390" Then

                    nParametri += 1
                End If

            Next

            Dim pto12 As Boolean = False
            Dim pto15 As Boolean = False
            ' livello alto di portata
            dr = DT.Select("Punto_Numero='0340'")
            If dr.Length > 0 Then
                pto12 = True
            End If

            ' livello alto di portata
            dr = DT.Select("Punto_Numero='0380'")
            If dr.Length > 0 Then
                pto15 = True
            End If

            If nParametri > 2 Or pto15 Or (Portata = 5 And pto15) Then
                iLivello = 5
            ElseIf nParametri = 2 Or (Portata = 3 And pto12 And Not pto15) Then
                iLivello = 3
            ElseIf nParametri = 1 Or (Portata = 1 And pto12 And Not pto15) Then
                iLivello = 1
            End If

        End If

        DT.Dispose()
        DT = Nothing




        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Gravita_Norma2(ByVal SuperPiva As String,
                                   ByVal Piva As String,
                                   ByVal Regolamento_Cod As Integer) As Integer



        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim DT As DataTable = LeggiRisposte(9, "", "1")

        ' conto il numero di parametri violati
        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1

            If DT.Rows(i).Item("Punto_Numero") = "0020" Or
               DT.Rows(i).Item("Punto_Numero") = "0050" Or
               DT.Rows(i).Item("Punto_Numero") = "0060" Then

                nParametri += 1
            End If

        Next

        Dim dr As DataRow()
        Dim pto1a As Boolean = False
        dr = DT.Select("Punto_Numero='0030'")
        If dr.Length > 0 Then
            pto1a = True
        End If

        If pto1a Or nParametri > 1 Then
            iLivello = 5
        ElseIf nParametri = 1 Then
            iLivello = 3
        End If

        DT.Dispose()
        DT = Nothing




        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Durata_Norma2(ByVal SuperPiva As String,
                                   ByVal Piva As String,
                                   ByVal Regolamento_Cod As Integer) As Integer



        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim DT As DataTable = LeggiRisposte(9, "", "1")

        '' conto il numero di parametri violati
        'Dim i As Integer
        'For i = 0 To DT.Rows.Count - 1

        '    If DT.Rows(i).Item("Punto_Numero") = "0020" Or _
        '       DT.Rows(i).Item("Punto_Numero") = "0050" Or _
        '       DT.Rows(i).Item("Punto_Numero") = "0060" Then

        '        nParametri += 1
        '    End If

        'Next

        ' Calcolo la portata
        Dim Portata As Integer = 0
        Dim dr As DataRow()
        ' livello basso di portata
        dr = DT.Select("Punto_Numero='0200'")
        ' il controllo che il valore sia 1 è già nel where della query (vale anche per gli altri livelli di portata qui sotto)
        If dr.Length > 0 Then
            Portata = 1
        End If
        ' livello medio di portata
        dr = DT.Select("Punto_Numero='0210'")
        If dr.Length > 0 Then
            Portata = 3
        End If
        ' livello alto di portata
        dr = DT.Select("Punto_Numero='0220'")
        If dr.Length > 0 Then
            Portata = 5
        End If

        Dim pto1a As Boolean = False
        dr = DT.Select("Punto_Numero='0030'")
        If dr.Length > 0 Then
            pto1a = True
        End If

        Dim ptoExtraAziendali As Boolean = False
        dr = DT.Select("Punto_Numero='0230'")
        If dr.Length > 0 Then
            ptoExtraAziendali = True
        End If

        If Portata = 5 Or pto1a Or ptoExtraAziendali Then
            iLivello = 5
        ElseIf Portata = 3 Then
            iLivello = 3
        ElseIf Portata = 1 Then
            iLivello = 1
        End If

        DT.Dispose()
        DT = Nothing




        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Gravita_Norma4(ByVal SuperPiva As String,
                                   ByVal Piva As String,
                                   ByVal Regolamento_Cod As Integer) As Integer




        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim pto_411 As Boolean = False
        Dim pto_412 As Boolean = False
        Dim pto_421 As Boolean = False
        Dim pto_422 As Boolean = False
        Dim pto_423 As Boolean = False
        Dim pto_424 As Boolean = False
        Dim pto_431 As Boolean = False
        Dim pto_432 As Boolean = False
        Dim pto_441 As Boolean = False
        Dim pto_442 As Boolean = False
        Dim pto_451 As Boolean = False
        Dim pto_461 As Boolean = False

        Dim Carico_0 As Boolean = False
        Dim Carico_4 As Boolean = False
        Dim Carico_6 As Boolean = False
        Dim Pascoli_Magri As Boolean = False
        Dim Pascoli As Boolean = False

        ' casi particolari 2013
        Dim Sfalcio_PascoliMagri As Boolean = False
        Dim Sfalcio_Pascoli As Boolean = False
        Dim EquilibrioAlto As Boolean = False
        Dim EquilibrioAltriCasi As Boolean = False


        Dim DT As DataTable = LeggiRisposte(12, "", "1")

        ' Calcolo la portata
        Dim Portata As Integer = 0
        Dim dr As DataRow()
        ' livello basso di portata
        dr = DT.Select("Punto_Numero='0520'")
        ' il controllo che il valore sia 1 è già nel where della query (vale anche per gli altri livelli di portata qui sotto)
        If dr.Length > 0 Then
            Portata = 1
        End If
        ' livello medio di portata
        dr = DT.Select("Punto_Numero='0530'")
        If dr.Length > 0 Then
            Portata = 3
        End If
        ' livello alto di portata
        dr = DT.Select("Punto_Numero='0540'")
        If dr.Length > 0 Then
            Portata = 5
        End If

        ' infrazioni per calcolo
        dr = DT.Select("Punto_Numero='0400'")
        If dr.Length > 0 Then
            pto_411 = True
        End If

        dr = DT.Select("Punto_Numero='0410'")
        If dr.Length > 0 Then
            pto_412 = True
        End If

        dr = DT.Select("Punto_Numero='0420'")
        If dr.Length > 0 Then
            pto_421 = True
        End If

        dr = DT.Select("Punto_Numero='0430'")
        If dr.Length > 0 Then
            pto_422 = True
        End If

        dr = DT.Select("Punto_Numero='0440'")
        If dr.Length > 0 Then
            pto_423 = True
        End If

        dr = DT.Select("Punto_Numero='0450'")
        If dr.Length > 0 Then
            pto_424 = True
        End If

        dr = DT.Select("Punto_Numero='0460'")
        If dr.Length > 0 Then
            pto_431 = True
        End If

        dr = DT.Select("Punto_Numero='0470'")
        If dr.Length > 0 Then
            pto_432 = True
        End If

        dr = DT.Select("Punto_Numero='0480'")
        If dr.Length > 0 Then
            pto_441 = True
        End If

        dr = DT.Select("Punto_Numero='0490'")
        If dr.Length > 0 Then
            pto_442 = True
        End If

        dr = DT.Select("Punto_Numero='0500'")
        If dr.Length > 0 Then
            pto_451 = True
        End If

        dr = DT.Select("Punto_Numero='0510'")
        If dr.Length > 0 Then
            pto_461 = True
        End If

        dr = DT.Select("Punto_Numero='0610'")
        If dr.Length > 0 Then
            Carico_0 = True
        End If

        dr = DT.Select("Punto_Numero='0620'")
        If dr.Length > 0 Then
            Carico_4 = True
        End If

        dr = DT.Select("Punto_Numero='0630'")
        If dr.Length > 0 Then
            Carico_6 = True
        End If

        dr = DT.Select("Punto_Numero='0640'")
        If dr.Length > 0 Then
            Pascoli_Magri = True
        End If

        dr = DT.Select("Punto_Numero='0650'")
        If dr.Length > 0 Then
            Pascoli = True
        End If

        ' dal 2013 regolamento_cod=5
        dr = DT.Select("Punto_Numero='0680'")
        If dr.Length > 0 Then
            Sfalcio_PascoliMagri = True
        End If

        dr = DT.Select("Punto_Numero='0690'")
        If dr.Length > 0 Then
            Sfalcio_Pascoli = True
        End If

        dr = DT.Select("Punto_Numero='0710'")
        If dr.Length > 0 Then
            EquilibrioAlto = True
        End If

        dr = DT.Select("Punto_Numero='0720'")
        If dr.Length > 0 Then
            EquilibrioAltriCasi = True
        End If

        If (pto_411 Or pto_422 Or pto_441 Or pto_442 Or pto_451) Or
           ((pto_412 Or pto_421 Or pto_423 Or pto_424 Or pto_431 Or pto_432) And Portata = 5) Or
           Carico_0 Or Carico_6 Or Pascoli Or
           Sfalcio_Pascoli Or EquilibrioAlto Then
            iLivello = 5

        ElseIf (Portata = 3 And (pto_421 And pto_424)) Or
               (Portata <= 3 And (pto_412 Or pto_423 Or pto_431 Or pto_432)) Or
               Carico_4 Or Pascoli_Magri Or
               Sfalcio_PascoliMagri Or EquilibrioAltriCasi Then
            iLivello = 3

        ElseIf Portata = 1 And (pto_421 And pto_424) Then
            iLivello = 1
        End If

        dr = Nothing

        DT.Dispose()
        DT = Nothing



        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Durata_Norma4(ByVal SuperPiva As String,
                                  ByVal Piva As String,
                                  ByVal Regolamento_Cod As Integer) As Integer



        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim pto_411 As Boolean = False
        Dim pto_412 As Boolean = False
        Dim pto_421 As Boolean = False
        Dim pto_422 As Boolean = False
        Dim pto_423 As Boolean = False
        Dim pto_424 As Boolean = False
        Dim pto_431 As Boolean = False
        Dim pto_432 As Boolean = False
        Dim pto_441 As Boolean = False
        Dim pto_442 As Boolean = False
        Dim pto_451 As Boolean = False
        Dim pto_461 As Boolean = False

        Dim Carico_0 As Boolean = False
        Dim Carico_4 As Boolean = False
        Dim Carico_6 As Boolean = False
        Dim Pascoli_Magri As Boolean = False
        Dim Pascoli As Boolean = False

        ' casi particolari 2013
        Dim Sfalcio_PascoliMagri As Boolean = False
        Dim Sfalcio_Pascoli As Boolean = False
        Dim EquilibrioAlto As Boolean = False
        Dim EquilibrioAltriCasi As Boolean = False


        Dim DT As DataTable = LeggiRisposte(12, "", "1")

        ' Calcolo la portata
        Dim Portata As Integer = 0
        Dim dr As DataRow()
        ' livello basso di portata
        dr = DT.Select("Punto_Numero='0520'")
        ' il controllo che il valore sia 1 è già nel where della query (vale anche per gli altri livelli di portata qui sotto)
        If dr.Length > 0 Then
            Portata = 1
        End If
        ' livello medio di portata
        dr = DT.Select("Punto_Numero='0530'")
        If dr.Length > 0 Then
            Portata = 3
        End If
        ' livello alto di portata
        dr = DT.Select("Punto_Numero='0540'")
        If dr.Length > 0 Then
            Portata = 5
        End If

        ' infrazioni per calcolo
        dr = DT.Select("Punto_Numero='0400'")
        If dr.Length > 0 Then
            pto_411 = True
        End If

        dr = DT.Select("Punto_Numero='0410'")
        If dr.Length > 0 Then
            pto_412 = True
        End If

        dr = DT.Select("Punto_Numero='0420'")
        If dr.Length > 0 Then
            pto_421 = True
        End If

        dr = DT.Select("Punto_Numero='0430'")
        If dr.Length > 0 Then
            pto_422 = True
        End If

        dr = DT.Select("Punto_Numero='0440'")
        If dr.Length > 0 Then
            pto_423 = True
        End If

        dr = DT.Select("Punto_Numero='0450'")
        If dr.Length > 0 Then
            pto_424 = True
        End If

        dr = DT.Select("Punto_Numero='0460'")
        If dr.Length > 0 Then
            pto_431 = True
        End If

        dr = DT.Select("Punto_Numero='0470'")
        If dr.Length > 0 Then
            pto_432 = True
        End If

        dr = DT.Select("Punto_Numero='0480'")
        If dr.Length > 0 Then
            pto_441 = True
        End If

        dr = DT.Select("Punto_Numero='0490'")
        If dr.Length > 0 Then
            pto_442 = True
        End If

        dr = DT.Select("Punto_Numero='0500'")
        If dr.Length > 0 Then
            pto_451 = True
        End If

        dr = DT.Select("Punto_Numero='0510'")
        If dr.Length > 0 Then
            pto_461 = True
        End If

        dr = DT.Select("Punto_Numero='0610'")
        If dr.Length > 0 Then
            Carico_0 = True
        End If

        dr = DT.Select("Punto_Numero='0620'")
        If dr.Length > 0 Then
            Carico_4 = True
        End If

        dr = DT.Select("Punto_Numero='0630'")
        If dr.Length > 0 Then
            Carico_6 = True
        End If

        dr = DT.Select("Punto_Numero='0640'")
        If dr.Length > 0 Then
            Pascoli_Magri = True
        End If

        dr = DT.Select("Punto_Numero='0650'")
        If dr.Length > 0 Then
            Pascoli = True
        End If

        ' dal 2013 regolamento_cod=5
        dr = DT.Select("Punto_Numero='0680'")
        If dr.Length > 0 Then
            Sfalcio_PascoliMagri = True
        End If

        dr = DT.Select("Punto_Numero='0690'")
        If dr.Length > 0 Then
            Sfalcio_Pascoli = True
        End If

        dr = DT.Select("Punto_Numero='0710'")
        If dr.Length > 0 Then
            EquilibrioAlto = True
        End If

        dr = DT.Select("Punto_Numero='0720'")
        If dr.Length > 0 Then
            EquilibrioAltriCasi = True
        End If

        If (pto_421 And pto_423 And pto_424) Or
           ((IIf(pto_412, 1, 0) + IIf(pto_422, 1, 0) + IIf(pto_431, 1, 0) + IIf(pto_432, 1, 0)) = 2) Or
           ((IIf(pto_411, 1, 0) + IIf(pto_441, 1, 0) + IIf(pto_442, 1, 0) + IIf(pto_451, 1, 0)) = 1) Or
           Carico_0 Or Carico_6 Or Pascoli Or
           Sfalcio_Pascoli Or EquilibrioAlto Then
            iLivello = 5

        ElseIf ((IIf(pto_421, 1, 0) + IIf(pto_423, 1, 0) + IIf(pto_424, 1, 0)) = 2) Or
               ((IIf(pto_412, 1, 0) + IIf(pto_422, 1, 0) + IIf(pto_431, 1, 0) + IIf(pto_432, 1, 0)) = 1) Or
               Carico_4 Or Pascoli_Magri Or
               Sfalcio_PascoliMagri Or EquilibrioAltriCasi Then
            iLivello = 3

        ElseIf Portata = 1 And (pto_421 And pto_424) Then
            iLivello = 1
        End If

        dr = Nothing

        DT.Dispose()
        DT = Nothing



        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Portata_Norma5(ByVal SuperPiva As String,
                                   ByVal Piva As String,
                                   ByVal Regolamento_Cod As Integer) As Integer

        Dim strErr As String = ""
        'Dim iLivello As Integer = 0
        '  Dim nParametri As Integer = 0

        ' cerco quello con valore 0, ovvero quelli con valore 'NO' (appartenenti alla prima parte)
        Dim DT As DataTable = LeggiRisposte(29, "", "0")


        ' Aggiunto standard 5.3 che era il vecchio atto A2 da condizionalità 2014
        Dim iLivelloStd3 As Integer = 0
        If Regolamento_Cod >= 6 Then

            Dim Diffida As Boolean = False
            Dim Dispersione As Boolean = False

            ' CONDIZIONALITA' 2012 e 2013
            '  nParametri = 0
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0099" Or
                   DT.Rows(i).Item("Punto_Numero") = "0091" Or
                   DT.Rows(i).Item("Punto_Numero") = "0092" Or
                   DT.Rows(i).Item("Punto_Numero") = "0098" Or
                   DT.Rows(i).Item("Punto_Numero") = "0096" Then

                    iLivelloStd3 = 5
                    Exit For

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0097" Then
                    Diffida = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0088" Or
                       DT.Rows(i).Item("Punto_Numero") = "0090" Then
                    Dispersione = True
                End If
            Next

            If iLivelloStd3 <> 5 Then
                If Diffida And Dispersione Then
                    iLivelloStd3 = 5
                ElseIf Diffida Or Dispersione Then
                    iLivelloStd3 = 3
                Else
                    iLivelloStd3 = 1
                End If
            End If

            ' iLivello = Math.Max(iLivello, iLivelloStd3)

        End If

        DT.Dispose()
        DT = Nothing



        Return iLivelloStd3

    End Function

    '#########################################################################################################
    Public Function Gravita_Norma5(ByVal SuperPiva As String,
                                   ByVal Piva As String,
                                   ByVal Regolamento_Cod As Integer) As Integer

        Dim strErr As String = ""
        'Dim iLivello As Integer = 0
        '  Dim nParametri As Integer = 0

        ' cerco quello con valore 0, ovvero quelli con valore 'NO' (appartenenti alla prima parte)
        Dim DT As DataTable = LeggiRisposte(29, "", "0")


        ' Aggiunto standard 5.3 che era il vecchio atto A2 da condizionalità 2014
        Dim iLivelloStd3 As Integer = 0
        If Regolamento_Cod >= 6 Then

            Dim Diffida As Boolean = False
            Dim Dispersione As Boolean = False

            ' CONDIZIONALITA' 2012 e 2013
            '  nParametri = 0
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0099" Or
                   DT.Rows(i).Item("Punto_Numero") = "0091" Or
                   DT.Rows(i).Item("Punto_Numero") = "0092" Or
                   DT.Rows(i).Item("Punto_Numero") = "0098" Or
                   DT.Rows(i).Item("Punto_Numero") = "0096" Then

                    iLivelloStd3 = 5
                    Exit For

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0097" Then
                    Diffida = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0088" Or
                       DT.Rows(i).Item("Punto_Numero") = "0090" Then
                    Dispersione = True
                End If
            Next

            If iLivelloStd3 <> 5 Then
                If Diffida And Dispersione Then
                    iLivelloStd3 = 5
                ElseIf Diffida Or Dispersione Then
                    iLivelloStd3 = 3
                Else
                    iLivelloStd3 = 1
                End If
            End If

            ' iLivello = Math.Max(iLivello, iLivelloStd3)

        End If

        DT.Dispose()
        DT = Nothing



        Return iLivelloStd3
    End Function




    '#########################################################################################################
    Public Function Durata_Norma5(ByVal SuperPiva As String,
                                  ByVal Piva As String,
                                  ByVal Regolamento_Cod As Integer) As Integer

        Dim strErr As String = ""
        Dim iLivello As Integer = 0
        Dim nParametri As Integer = 0

        Dim pto_521 As Boolean = False
        Dim pto_522 As Boolean = False
        Dim pto_523 As Boolean = False
        Dim pto_524 As Boolean = False


        Dim DT As DataTable = LeggiRisposte(29, "", "1")

        ' Calcolo la portata
        Dim Portata As Integer = 0
        Dim dr As DataRow()

        If Regolamento_Cod < 6 Then
            ' livello basso di portata
            dr = DT.Select("Punto_Numero='0100'")
            ' il controllo che il valore sia 1 è già nel where della query (vale anche per gli altri livelli di portata qui sotto)
            If dr.Length > 0 Then
                Portata = 1
            End If
        Else
            ' livello basso di portata (per il regolamento 2014 ho cambiato il codice del livello basso di portata)
            dr = DT.Select("Punto_Numero='0105'")
            ' il controllo che il valore sia 1 è già nel where della query (vale anche per gli altri livelli di portata qui sotto)
            If dr.Length > 0 Then
                Portata = 1
            End If
        End If

        ' livello medio di portata
        dr = DT.Select("Punto_Numero='0110'")
        If dr.Length > 0 Then
            Portata = 3
        End If
        ' livello alto di portata
        dr = DT.Select("Punto_Numero='0120'")
        If dr.Length > 0 Then
            Portata = 5
        End If

        DT.Dispose()
        DT = Nothing
        DT = LeggiRisposte(29, "", "0")
        ' infrazioni per calcolo
        dr = DT.Select("Punto_Numero='0050'")
        If dr.Length > 0 Then
            pto_521 = True
        End If

        dr = DT.Select("Punto_Numero='0060'")
        If dr.Length > 0 Then
            pto_522 = True
        End If

        dr = DT.Select("Punto_Numero='0070'")
        If dr.Length > 0 Then
            pto_523 = True
        End If

        dr = DT.Select("Punto_Numero='0080'")
        If dr.Length > 0 Then
            pto_524 = True
        End If

        iLivello = 1
        If Portata = 5 Or pto_521 Then
            iLivello = 5
        ElseIf (pto_522 Or pto_523 Or pto_524) And (Portata < 5) Then
            iLivello = 3
        End If


        ' Aggiunto standard 5.3 che era il vecchio atto A2 da condizionalità 2014
        Dim iLivelloStd3 As Integer = 0
        If Regolamento_Cod >= 6 Then
            ' cerco quello con valore 0, ovvero quelli con valore 'NO' (appartenenti alla prima parte)
            DT = LeggiRisposte(2, "", "0")

            Dim Diffida As Boolean = False
            Dim Dispersione As Boolean = False

            ' CONDIZIONALITA' 2012 e 2013
            ' nParametri = 0
            For i = 0 To DT.Rows.Count - 1

                If DT.Rows(i).Item("Punto_Numero") = "0099" Or
                   DT.Rows(i).Item("Punto_Numero") = "0091" Or
                   DT.Rows(i).Item("Punto_Numero") = "0092" Or
                   DT.Rows(i).Item("Punto_Numero") = "0098" Or
                   DT.Rows(i).Item("Punto_Numero") = "0096" Then

                    iLivelloStd3 = 5
                    Exit For

                ElseIf DT.Rows(i).Item("Punto_Numero") = "0097" Then
                    Diffida = True
                ElseIf DT.Rows(i).Item("Punto_Numero") = "0088" Or
                       DT.Rows(i).Item("Punto_Numero") = "0090" Then
                    Dispersione = True
                End If
            Next

            If iLivelloStd3 <> 5 Then
                If Diffida And Dispersione Then
                    iLivelloStd3 = 5
                ElseIf Diffida Or Dispersione Then
                    iLivelloStd3 = 3
                Else
                    iLivelloStd3 = 1
                End If
            End If

            iLivello = Math.Max(iLivello, iLivelloStd3)

        End If

        dr = Nothing

        DT.Dispose()
        DT = Nothing

        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Portata_C16(ByVal SuperPiva As String,
                                ByVal Piva As String,
                                ByVal Regolamento_Cod As Integer) As Integer



        Dim strErr As String = ""
        Dim iLivello As Integer = 1

        Dim nInfrazioniA, nInfrazioniB, nInfrazioniC As Integer

        Dim DT As DataTable = LeggiRisposte(Disposizione, "", "")
        'DT = DLL_AD.AuditAppoggio_Leggi(TipiEnumerativi.enum_AuditTipi.AuditTipi_Condizionalita, Regolamento_Cod, m_strAnonymousID, m_Disp_Cod, "", "", 1, 1, strErr, m_Connessione, m_Transazione)
        Dim i As Integer

        Dim dr() As DataRow

        ' caso particolare (se mutilazioni o emoglobina sempre livello alto)
        dr = DT.Select("(Valore='1' OR Valore='2' OR Valore='3') AND (Punto_Numero = '0050' OR Punto_Numero = '0060')")
        If dr.Length > 0 Then
            iLivello = 5
        Else

            ' se è stata indicata un'infrazione (A, B o C), ma non il numero allora considero il numero di infrazioni pari a 1
            dr = DT.Select("Valore='1'")
            For i = 0 To dr.Length - 1
                If Not IsDBNull(dr(i).Item("Valore_2")) AndAlso IsNumeric(dr(i).Item("Valore_2")) Then
                    nInfrazioniA += dr(i).Item("Valore_2")
                Else
                    nInfrazioniA += 1
                End If
            Next

            dr = DT.Select("Valore='2'")
            For i = 0 To dr.Length - 1
                If Not IsDBNull(dr(i).Item("Valore_2")) AndAlso IsNumeric(dr(i).Item("Valore_2")) Then
                    nInfrazioniB += dr(i).Item("Valore_2")
                Else
                    nInfrazioniB += 1
                End If
            Next

            dr = DT.Select("Valore='3'")
            For i = 0 To dr.Length - 1
                If Not IsDBNull(dr(i).Item("Valore_2")) AndAlso IsNumeric(dr(i).Item("Valore_2")) Then
                    nInfrazioniC += dr(i).Item("Valore_2")
                Else
                    nInfrazioniC += 1
                End If
            Next

            If nInfrazioniC > 0 Or nInfrazioniA + nInfrazioniB > 4 Then
                iLivello = 5
            ElseIf nInfrazioniA + nInfrazioniB > 2 And nInfrazioniA + nInfrazioniB <= 4 Then
                iLivello = 3
            ElseIf nInfrazioniA + nInfrazioniB <= 2 Then
                iLivello = 1
            End If

        End If

        Return iLivello

    End Function

    '#########################################################################################################
    Public Function Gravita_C16(ByVal SuperPiva As String,
                                ByVal Piva As String,
                                ByVal Regolamento_Cod As Integer) As Integer



        Dim strErr As String = ""
        Dim iLivello As Integer = 1

        Dim nRequisitiA, nRequisitiB, nRequisitiC As Integer

        Dim DT As DataTable = LeggiRisposte(Disposizione, "", "")
        'DT = DLL_AD.AuditAppoggio_Leggi(TipiEnumerativi.enum_AuditTipi.AuditTipi_Condizionalita, Regolamento_Cod, m_strAnonymousID, m_Disp_Cod, "", "", 1, 1, strErr, m_Connessione, m_Transazione)

        Dim i As Integer

        Dim dr() As DataRow

        ' caso particolare (se mutilazioni o emoglobina sempre livello alto)
        dr = DT.Select("(Valore='1' OR Valore='2' OR Valore='3') AND (Punto_Numero = '0050' OR Punto_Numero = '0060')")
        If dr.Length > 0 Then
            iLivello = 5
        Else

            ' devo contare il numero di requisiti violati, non il numero di infrazioni (come nella portata)
            dr = DT.Select("Valore='1'")
            For i = 0 To dr.Length - 1
                nRequisitiA += 1
            Next

            dr = DT.Select("Valore='2'")
            For i = 0 To dr.Length - 1
                nRequisitiB += 1
            Next

            dr = DT.Select("Valore='3'")
            For i = 0 To dr.Length - 1
                nRequisitiC += 1
            Next

            If nRequisitiC > 0 Or nRequisitiA + nRequisitiB > 4 Then
                iLivello = 5
            ElseIf nRequisitiA + nRequisitiB > 2 And nRequisitiA + nRequisitiB <= 4 Then
                iLivello = 3
            ElseIf nRequisitiA + nRequisitiB <= 2 Then
                iLivello = 1
            End If

        End If

        Return iLivello

    End Function


    '#########################################################################################################
    Public Function Durata_C16(ByVal SuperPiva As String,
                               ByVal Piva As String,
                               ByVal Regolamento_Cod As Integer) As Integer



        Dim strErr As String = ""
        Dim iLivello As Integer = 1

        Dim nRequisitiA, nRequisitiB, nRequisitiC As Integer

        Dim livPortata, livGravita As Integer
        Dim DT As DataTable = LeggiRisposte(Disposizione, "", "")
        'DT = DLL_AD.AuditAppoggio_Leggi(TipiEnumerativi.enum_AuditTipi.AuditTipi_Condizionalita, Regolamento_Cod, m_strAnonymousID, m_Disp_Cod, "", "", 0, 0, strErr, m_Connessione, m_Transazione)


        Dim dr() As DataRow


        ' caso particolare (se mutilazioni o emoglobina sempre livello alto)
        dr = DT.Select("(Valore='1' OR Valore='2' OR Valore='3') AND (Punto_Numero = '0050' OR Punto_Numero = '0060')")
        If dr.Length > 0 Then
            iLivello = 5

        Else

            dr = DT.Select(" Punto_Numero ='0200' AND Valore='1'")
            If dr.Length > 0 Then
                livPortata = 1
            End If

            dr = DT.Select(" Punto_Numero ='0215' AND Valore='1'")
            If dr.Length > 0 Then
                livGravita = 1
            End If

            dr = DT.Select(" Sezione_Cod=1 AND Valore='3'")
            If dr.Length > 0 Then
                nRequisitiC = 1
            End If

            If nRequisitiC > 0 Then
                iLivello = 5
            ElseIf livPortata = 1 And livGravita = 1 Then
                iLivello = 1
            Else
                iLivello = 3
            End If

        End If

        Return iLivello

    End Function

End Class
