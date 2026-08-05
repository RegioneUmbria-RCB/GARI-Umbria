Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Module Input_Controllato

    'Le pagine in cui vengono usate le espressioni regolari per username e password sono:
    '- Autenticazione
    '- Utenti_Edit2
    '- Utenti_Impostazioni
    '- StarterKit
    '- StarterKit_Utenti_Edit


    '################################################################################################
    '#####   Tipo enumerativo per le espressioni regolari standard
    '################################################################################################

    Public Enum enum_EspressioniRegolari

        RegExp_Nessuna = 0
        RegExp_Email = 1
        RegExp_CodiceFiscale = 2
        RegExp_PartitaIVA = 3
        RegExp_Username = 4
        RegExp_Password = 5

    End Enum




    '################################################################################################
    Public Function VerificaEspressioneRegolare(ByVal StringaDaAnalizzare As String,
                                                ByVal EspressioneRegolare As String,
                                                ByVal EspressioneRegolareStandard As enum_EspressioniRegolari
                                                ) As Boolean

        Dim stringaRegExp As String

        'NOTA
        'Se la stringa di convalida è nulla, allora utilizzo una delle stringhe standard

        If EspressioneRegolare <> "" Then

            'L'espressione regolare di convalida è passata dall'utente
            stringaRegExp = EspressioneRegolare

        Else

            'Utilizzo una delle stringhe standard
            Select Case EspressioneRegolareStandard

                Case enum_EspressioniRegolari.RegExp_Nessuna

                    stringaRegExp = "."

                    '===========================================================================


                Case enum_EspressioniRegolari.RegExp_Email

                    stringaRegExp = "^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"

                    '===========================================================================


                Case enum_EspressioniRegolari.RegExp_CodiceFiscale
                    '
                    '   AAABBB22C33d456e
                    '
                    stringaRegExp = "([a-zA-Z]{6})([0-9]{2})([a-zA-Z]{1})([0-9]{2})([a-zA-Z]{1})([0-9]{3})([a-zA-Z]{1})"
                    '
                    '   [\w{6}\d{2}\w{1}\d{2}\w{1}\d{3}\w{1}]
                    '
                    '===========================================================================


                Case enum_EspressioniRegolari.RegExp_PartitaIVA
                    '
                    '   01234567890
                    '
                    stringaRegExp = "([0-9]{11})"

                    '===========================================================================


                Case enum_EspressioniRegolari.RegExp_Username

                    stringaRegExp = EXPREG_USERNAME

                    '===========================================================================

                Case enum_EspressioniRegolari.RegExp_Password

                    stringaRegExp = EXPREG_PASSWORD

                    '===========================================================================

            End Select

        End If

        Return Regex.IsMatch(StringaDaAnalizzare, stringaRegExp, RegexOptions.None, TimeSpan.FromSeconds(3))

    End Function


    '################################################################################################
    Public Sub TestRegExp()

        'Descrizione: 
        'Questa è una routine di test per la funzione "VerificaEspressioneRegolare"
        'Viene mantenuta come esempio sull'uso della funzione ...

        Dim pippo As Boolean

        'Codice Fiscale

        pippo = VerificaEspressioneRegolare("AAABBB33C44F123H", "", enum_EspressioniRegolari.RegExp_CodiceFiscale)

        pippo = VerificaEspressioneRegolare("AAA9BB33C44F123H", "", enum_EspressioniRegolari.RegExp_CodiceFiscale)

        pippo = VerificaEspressioneRegolare("aaaBBB33C49F123H", "", enum_EspressioniRegolari.RegExp_CodiceFiscale)

        pippo = VerificaEspressioneRegolare("AAABBB33C44F1232", "", enum_EspressioniRegolari.RegExp_CodiceFiscale)

        'Partita IVA

        pippo = VerificaEspressioneRegolare("01234567890", "", enum_EspressioniRegolari.RegExp_PartitaIVA)

        pippo = VerificaEspressioneRegolare("0123456789", "", enum_EspressioniRegolari.RegExp_PartitaIVA)

        pippo = VerificaEspressioneRegolare("0123A567890", "", enum_EspressioniRegolari.RegExp_PartitaIVA)

        'Il 12-esimo carattere lo ignora ...
        pippo = VerificaEspressioneRegolare("012345678902", "", enum_EspressioniRegolari.RegExp_PartitaIVA)

    End Sub



    '######################################################################################################
    Public Function Agro_CheckPartitaIVA(ByVal sCode As String) As Boolean
        '==========================================================================
        '  DESCRIZIONE :
        '     Procedura di controllo congruità partita iva:
        '        sCode    = Partita Iva da controllare


        Dim app10 As String
        Dim valTmp As Integer
        Dim i As Integer
        Dim rstTmp As Integer
        Dim valMod As Integer

        valTmp = 0
        app10 = Left$(sCode, 10)

        For i = 1 To 10 Step 2
            valTmp = valTmp + Val(Mid$(app10, i, 1))
        Next

        For i = 2 To 10 Step 2
            rstTmp = Val(Mid$(app10, i, 1)) * 2
            If rstTmp > 9 Then
                rstTmp = rstTmp - 9
            End If
            valTmp = valTmp + rstTmp
        Next

        valMod = valTmp Mod 10
        If valMod = 0 Then
            If Right$(sCode, 1) = "0" Then
                Return True
            Else
                Return False
            End If
        Else
            valMod = 10 - valMod
            If Val(Right$(sCode, 1)) = valMod Then
                Return True
            Else
                Return False
            End If
        End If

    End Function



    '#############################################################################
    Public Function Verifica_ValiditaInizio(ByRef Str_Errore As String,
                                            ByVal Str_ValiditaInizio As String
                                            ) As Date

        If Str_ValiditaInizio = "" Then
            Return CDate("01/01/1900")
        Else

            If IsDate(Str_ValiditaInizio) Then
                Return CDate(Str_ValiditaInizio)
            Else

                Str_Errore = "Il valore inserito come data di inizio non è una data!"
                Return Nothing

            End If

        End If

    End Function


    '#############################################################################
    Public Function Verifica_ValiditaFine(ByRef Str_Errore As String,
                                          ByVal Str_ValiditaFine As String
                                          ) As Date

        If Str_ValiditaFine = "" Then
            Return CDate("31/12/2100")
        Else

            If IsDate(Str_ValiditaFine) Then
                Return CDate(Str_ValiditaFine)
            Else

                Str_Errore = "Il valore inserito come data di fine non è una data!"
                Return Nothing

            End If

        End If

    End Function


    '################################################################################
    Public Function Verifica_StringaVuota(ByVal StringaBlankHtml) As String

        If StringaBlankHtml = "&nbsp;" Then
            Return ""
        Else
            Return StringaBlankHtml
        End If

    End Function

End Module
