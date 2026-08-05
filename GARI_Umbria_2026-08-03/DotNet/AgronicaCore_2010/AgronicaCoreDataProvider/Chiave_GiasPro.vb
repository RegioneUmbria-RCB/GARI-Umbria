Public Class Chiave_GiasPro



    '============================================================================================================
    '============================================================================================================
    '  PARAMETRO         DESCRIZIONE                                               LUNGHEZZA  MAX VAL
    'UserName
    'DataAttivazione     Data di attivazione                                              8   31129999 -> data di partenza del contratto
    'Durata              Durata in mesi del contratto                                     2         99 -> mesi di contratto dalla data di partenza
    'Ordine              Numero di ordine dell'utente per gestire le sequenze             4       2000 -> 2000 utenti
    'Versione            BitMask dei moduli attivabili                                    7    8388608 -> 23 moduli
    'Aziende             N° massimo di aziende                                            4       9999 -> 9999 aziende
    'Sup                 Sup Max per azienda in ha                                        1          9 -> 9 classi di sup.
    '                                                                             ____________
    '                                                                                    26
    '                                                                                   + 6           -> 2 seq di 3 cifre di check digit
    '                                                                             ____________
    '                                                                                    32
    '
    '============================================================================================================
    '
    '   ------------------- CLASSE
    '   0 =   10 Ha
    '   1 =   50 Ha
    '   2 =  100 Ha
    '   3 =  500 Ha
    '   4 = 1000 Ha
    '   5 = 2000 Ha
    '   6 = 3000 Ha
    '   7 = 4000 Ha
    '   8 = 5000 Ha
    '   9 = Nessun Limite
    '
    '   -------------------- MODULI
    '    1 = Gestione Magazzini
    '    2 = Gestione Catasto
    '    4 = Gestione Stampe
    '    8 = Quaderno di Campagna
    '   16 = Contabilita' Pro
    '
    '============================================================================================================



    '############################################################################################################
    Public Function ChiaveGiasPro_Codifica( _
                                ByVal UserName As String, _
                                ByVal DataAttivazione As Date, _
                                ByVal Durata As Integer, _
                                ByVal Ordine As Integer, _
                                ByVal Versione As Integer, _
                                ByVal Aziende As Integer, _
                                ByVal Sup As Integer) _
                                As String
        '-----------------------------------------------------------
        ' Procedura di Codifica
        '(c) Agronica SRL - Stefano Flamigni - 30/03/2001 - v1.0
        '-----------------------------------------------------------

        '----- Variabili
        Dim Mask As Long
        Dim sMask As String
        Dim sStart As String
        Dim sEnd As String
        Dim Pari As Long
        Dim Dispari As Long
        Dim i As Integer
        Dim v1 As Long
        Dim v2 As Long

        '-----

        UserName = UCase(UserName)
        Mask = 0
        For i = 1 To Len(UserName)
            Mask = Mask + Asc(Mid(UserName, i, 1))
        Next
        Mask = Mask Mod 999999
        sMask = CStr(Mask)
        sStart = Format(DataAttivazione, "ddMMyyyy") & Format(Durata, "00") & Format(Ordine, "0000") & Format(Versione, "0000000") & Format(Aziende, "0000") & Format(Sup, "0")
        Pari = 0
        Dispari = 0

        For i = 1 To Len(sStart)
            If (i Mod 2) = 0 Then
                Pari = Pari + Val(Mid(sStart, i, 1))
            Else
                Dispari = Dispari + Val(Mid(sStart, i, 1))
            End If
        Next
        sStart = sStart & Format(Dispari, "000") & Format(Pari, "000")

        sEnd = ""

        For i = 1 To Len(sStart)

            v1 = Val(Mid(sStart, i, 1))
            v2 = Val(Mid(sMask, 1 + ((i - 1) Mod Len(sMask)), 1))

            sEnd = sEnd + (CStr((v1 + v2) Mod 10))
        Next

        Return Left(sEnd, 8) & "-" & Mid(sEnd, 9, 8) & "-" & Mid(sEnd, 17, 8) & "-" & Right(sEnd, 8)


    End Function








    '######################################################################################################
    Public Function ChiaveGiasPro_Verifica( _
                                ByVal UserName As String, _
                                ByVal key As String, _
                                ByRef DataAttivazione As Date, _
                                ByRef Durata As Integer, _
                                ByRef b1 As Integer, _
                                ByRef l1 As Integer, _
                                ByRef b2 As Integer, _
                                ByRef l2 As Integer, _
                                ByRef Versione As Integer) _
                                As Boolean
        '-----------------------------------------------------------
        ' Procedura di DeCodifica
        '(c) Agronica SRL - Stefano Flamigni - 30/03/2001 - v1.0
        '-----------------------------------------------------------

        '----- Variabili
        Dim Mask As Long
        Dim sMask As String
        Dim sStart As String
        Dim sEnd As String
        Dim Pari As Long
        Dim Dispari As Long
        Dim Valore As Long
        Dim v1 As Long
        Dim v2 As Long
        Dim i As Integer

        '-----

        If Len(key) = 26 Then
            UserName = UCase(UserName)
            Mask = 0
            For i = 1 To Len(UserName)
                Mask = Mask + Asc(Mid(UserName, i, 1))
            Next
            Mask = Mask Mod 999999
            sMask = CStr(Mask)

            sEnd = Left(key, 8) & Mid(key, 10, 8) & Right(key, 8)

            'Debug.Print("sEnd: " & sEnd)
            'Debug.Print("sMask: " & sMask)

            sStart = ""
            For i = 1 To Len(sEnd)
                v1 = Val(Mid(sEnd, i, 1))
                v2 = Val(Mid(sMask, 1 + ((i - 1) Mod Len(sMask)), 1))
                Valore = v1 - v2
                If Valore >= 0 Then
                    sStart = sStart & Valore
                Else
                    sStart = sStart & (Valore + 10)
                End If
            Next

            'Debug.Print("sStart: " & sStart)

            Pari = 0
            Dispari = 0

            For i = 1 To Len(sStart) - 4
                If (i Mod 2) = 0 Then
                    Pari = Pari + Val(Mid(sStart, i, 1))
                Else
                    Dispari = Dispari + Val(Mid(sStart, i, 1))
                End If
            Next

            DataAttivazione = CDate(Format(CDate(Left(sStart, 2) & "/" & Mid(sStart, 3, 2) & "/" & Mid(sStart, 5, 4)), "dd/MM/yyyy"))
            Durata = Val(Mid(sStart, 9, 2))
            b1 = Val(Mid(sStart, 11, 2))
            l1 = Val(Mid(sStart, 13, 2))
            b2 = Val(Mid(sStart, 15, 2))
            l2 = Val(Mid(sStart, 17, 2))
            Versione = Val(Mid(sStart, 19, 2))

            If (Val(Mid(sStart, 21, 2)) = Dispari) AndAlso (Val(Mid(sStart, 23, 2)) = Pari) Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function






    '######################################################################################################
    Public Shared Function ChiaveGiasPro_Decodifica( _
                                ByVal UserName As String, _
                                ByVal key As String, _
                                ByRef DataAttivazione As Date, _
                                ByRef Durata As Integer, _
                                ByRef Ordine As Integer, _
                                ByRef Versione As Integer, _
                                ByRef Aziende As Integer, _
                                ByRef Sup As Integer) _
                                As Boolean
        '-----------------------------------------------------------
        ' Procedura di DeCodifica
        '(c) Agronica SRL - Stefano Flamigni - 24/09/2001 - v2.0
        '-----------------------------------------------------------

        '----- Variabili
        Dim Mask As Long
        Dim sMask As String
        Dim sStart As String
        Dim sEnd As String
        Dim Pari As Long
        Dim Dispari As Long
        Dim Valore As Long
        Dim v1 As Long
        Dim v2 As Long
        Dim i As Integer

        '-----

        If Len(key) = 35 Then
            UserName = UCase(UserName)
            Mask = 0
            For i = 1 To Len(UserName)
                Mask = Mask + Asc(Mid(UserName, i, 1))
            Next
            Mask = Mask Mod 999999
            sMask = CStr(Mask)

            sEnd = Left(key, 8) & Mid(key, 10, 8) & Mid(key, 19, 8) & Right(key, 8)

            'Debug.Print("sEnd: " & sEnd)
            'Debug.Print("sMask: " & sMask)

            sStart = ""
            For i = 1 To Len(sEnd)
                v1 = Val(Mid(sEnd, i, 1))
                v2 = Val(Mid(sMask, 1 + ((i - 1) Mod Len(sMask)), 1))
                Valore = v1 - v2
                If Valore >= 0 Then
                    sStart = sStart & Valore
                Else
                    sStart = sStart & (Valore + 10)
                End If
            Next

            'Debug.Print("sStart: " & sStart)

            Pari = 0
            Dispari = 0

            For i = 1 To Len(sStart) - 6
                If (i Mod 2) = 0 Then
                    Pari = Pari + Val(Mid(sStart, i, 1))
                Else
                    Dispari = Dispari + Val(Mid(sStart, i, 1))
                End If
            Next

            DataAttivazione = CDate(Format(CDate(Left(sStart, 2) & "/" & Mid(sStart, 3, 2) & "/" & Mid(sStart, 5, 4)), "dd/MM/yyyy"))
            Durata = Val(Mid(sStart, 9, 2))
            Ordine = Val(Mid(sStart, 11, 4))
            Versione = Val(Mid(sStart, 15, 7))
            Aziende = Val(Mid(sStart, 22, 4))
            Sup = Val(Mid(sStart, 26, 1))

            If (Val(Mid(sStart, 27, 3)) = Dispari) AndAlso (Val(Mid(sStart, 30, 3)) = Pari) Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function









End Class
