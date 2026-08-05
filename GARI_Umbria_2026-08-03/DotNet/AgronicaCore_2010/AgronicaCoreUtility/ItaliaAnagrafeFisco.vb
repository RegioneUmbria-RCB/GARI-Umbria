Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class ItaliaAnagrafeFisco

    Private ArrayCharPosizioneDispari() As Integer = {1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23}
    Private ArrayChar() As String = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"}
    Private ArrayPari As New ArrayList
    Private ArrayDispari As New ArrayList

    ''' <summary>
    ''' verifica validità del codice fiscale con carattere di controllo.
    ''' </summary>
    ''' <param name="CodiceFiscale"></param>
    ''' <returns></returns>
    Public Function CodiceFiscaleValido_CarattereControllo(ByVal CodiceFiscale As String) As Boolean

        If CodiceFiscale.Length <> 16 Then
            Return False
        End If

        Dim risultato As Integer = 0
        Dim sommapari As Integer = 0
        Dim sommadispari As Integer = 0
        Try
            If CodiceFiscale.ToString <> "" Then
                For i As Integer = 0 To CodiceFiscale.Length - 2 Step 1
                    'Vi ricordo che l'array parte da zero ma noi dobbiamo calcolare come se fosse 1 
                    'e quindi Dispari si trovano nella posizione Pari e i Pari si trovano nella posizione Dispari
                    If i Mod (2) = 1 Then
                        'Indice Dispari - Posizione Dispari)
                        ArrayPari.Add(CodiceFiscale.Substring(i, 1).ToString)
                    Else
                        'Indice Pari - Posizione Pari
                        ArrayDispari.Add(CodiceFiscale.Substring(i, 1).ToString)
                    End If
                Next
                For i As Integer = 0 To ArrayPari.Count - 1 Step 1
                    If IsNumeric(ArrayPari(i).ToString) Then
                        sommapari = sommapari + ArrayPari(i).ToString
                        'Console.WriteLine(ArrayPari(i).ToString & " ArrayPari(" & i & ").ToString = " & ArrayPari(i).ToString)
                    Else
                        For j As Integer = 0 To ArrayChar.Length - 1 Step 1
                            If (UCase(ArrayPari(i).ToString) = ArrayChar(j).ToString) Then
                                sommapari = sommapari + j
                                'Console.WriteLine(ArrayPari(i).ToString & " ArrayPari(" & i & ").ToString = " & j)
                                Exit For
                            End If
                        Next
                    End If
                Next
                For i As Integer = 0 To ArrayDispari.Count - 1 Step 1
                    If IsNumeric(ArrayDispari(i).ToString) Then
                        sommadispari = sommadispari + ArrayCharPosizioneDispari(ArrayDispari(i).ToString).ToString
                        'Console.WriteLine(ArrayDispari(i).ToString & " ArrayDispari(" & i & ").ToString = " & ArrayCharPosizioneDispari(ArrayDispari(i).ToString).ToString)
                    Else
                        For j As Integer = 0 To ArrayChar.Length - 1 Step 1
                            If (UCase(ArrayDispari(i).ToString) = ArrayChar(j).ToString) Then
                                sommadispari = sommadispari + ArrayCharPosizioneDispari(j).ToString
                                'Console.WriteLine(ArrayDispari(i).ToString & " ArrayCharPosizioneDispari(" & j & ").ToString = " & ArrayCharPosizioneDispari(j).ToString)
                                Exit For
                            End If
                        Next
                    End If
                Next
            End If
            risultato = 0
            ArrayPari.Clear()
            ArrayDispari.Clear()
            risultato = (sommapari + sommadispari) Mod 26 'Restituisce il Resto con il comando Mod 
            'Console.WriteLine(ArrayChar(risultato).ToString & " = " & CodiceFiscale.Substring(CodiceFiscale.Length - 1, 1).ToString)
            'Controllo che la lettera che si trova nella posizione risultato o resto corrisponde a l'ultima lettera del codice fiscale passato
            If (ArrayChar(risultato).ToString = UCase(CodiceFiscale.Substring(CodiceFiscale.Length - 1, 1).ToString)) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            'Errore generato da qualche numero e carattere strano
            Return False
        End Try
    End Function


    '##############################################################################################
    ''' <summary>
    ''' Verifica validità Partita Iva, sia con Regular Expression che con calcolo carattere di controllo,
    ''' accetta anche piva con prefisso nazionale
    ''' </summary>
    ''' <param name="Piva">Partita Iva da validare</param>
    ''' <returns>True se è valido, altrimenti False</returns>
    Public Shared Function IsPivaValida(ByVal Piva As String) As Boolean

        Dim n_Val As Integer
        Dim n_Som1 As Integer
        Dim n_Som2 As Integer
        Dim lcv As Integer


        If Piva = "" Then
            Return False
        End If


        'Gestione prefisso nazionale sulla partita IVA

        If Not IsNumeric(Mid(Trim(Piva), 1, 2)) Then
            Piva = Mid(Trim(Piva), 3, Piva.Length)
        ElseIf Not IsNumeric(Mid(Trim(Piva), 1, 3)) Then
            Piva = Mid(Trim(Piva), 4, Piva.Length)
        End If

        If Not AgronicaCoreDataProvider.UtilityProvider.VerificaEspressioneRegolare(Piva, "", enum_EspressioniRegolari.RegExp_PartitaIVA) Then '"^[0-9]{11}$")" "^\d{11}$"
            Return False
        End If

        For lcv = 1 To 9 Step 2
            n_Val = Val(Mid$(Piva, lcv, 1))
            n_Som1 = n_Som1 + n_Val
            n_Val = Val(Mid$(Piva, lcv + 1, 1))
            n_Som1 = n_Som1 + Int((n_Val * 2) / 10) + ((n_Val * 2) Mod 10)
        Next lcv

        n_Som2 = 10 - (n_Som1 Mod 10)

        If n_Som2 = 10 Then
            n_Som2 = 0
        End If

        n_Val = Val(Mid$(Piva, 11, 1))

        If n_Som2 = n_Val Then
            Return True
        Else
            'CheckPartitaIva = n_Som2 + 48
            Return False
        End If

    End Function
End Class
