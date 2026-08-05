Module Utility

    ''################################################################
    'Public Function Arrotonda(ByVal Valore As Double, ByVal TipoArrotondamento As Integer) As Double

    '    Select Case TipoArrotondamento

    '        Case -1 'nessuno

    '            Return Valore

    '        Case Else 'da 0 = unità a 4 = 4 decimali

    '            'in questo caso TipoArrotondamento corrisponde al numero di cifre decimali
    '            Return Math.Round(Valore, TipoArrotondamento)

    '    End Select

    'End Function

    ''################################################################
    'Public Function Arrotonda_Unita(ByVal Valore As Double) As Double

    '    Valore = Valore + 0.5
    '    Valore = Int(Valore)

    '    Return Valore

    'End Function


    ''################################################################
    ''da testare
    'Public Function Arrotonda_1Decimale(ByVal Valore As Double) As Double

    '    Valore = Valore + 0.050000000000000003

    '    Valore = Valore * 10

    '    Valore = Int(Valore)

    '    Valore = Valore / 10

    '    Return Valore

    'End Function

    '################################################################
    Public Function Arrotonda_2Decimali(ByVal Valore As Double) As Double

        Valore = Valore + 0.0050000000000000001

        Valore = Valore * 100

        Valore = Int(Valore)

        Valore = Valore / 100

        Return Valore

    End Function

    ''################################################################
    'Public Function Arrotonda_3Decimali(ByVal Valore As Double) As Double

    '    Valore = Valore + 0.00050000000000000001

    '    Valore = Valore * 1000

    '    Valore = Int(Valore)

    '    Valore = Valore / 1000

    '    Return Valore

    'End Function

    ''################################################################
    ''da testare
    'Public Function Arrotonda_4Decimali(ByVal Valore As Double) As Double

    '    Valore = Valore + 0.000050000000000000002

    '    Valore = Valore * 10000

    '    Valore = Int(Valore)

    '    Valore = Valore / 10000

    '    Return Valore

    'End Function

End Module
