Imports System
Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions


Public Class Agro_Math

    <Obsolete("Usare la funzione ArrotondaVal_0")>
    Public Shared Function RoundNumber_ParteIntera(ByVal numero As Decimal) As Decimal
        Return RoundNumber(numero, 0)
    End Function

    <Obsolete("Usare la funzione ArrotondaVal_1")>
    Public Shared Function RoundNumber_1Decimale(ByVal numero As Decimal) As Decimal
        Return RoundNumber(numero, 1)
    End Function

    <Obsolete("Usare la funzione ArrotondaVal_2")>
    Public Shared Function RoundNumber_2Decimali(ByVal numero As Decimal) As Decimal
        Return RoundNumber(numero, 2)
    End Function

    <Obsolete("Usare la funzione ArrotondaVal_3")>
    Public Shared Function RoundNumber_3Decimali(ByVal numero As Decimal) As Decimal
        Return RoundNumber(numero, 3)
    End Function

    <Obsolete("Usare la funzione ArrotondaVal_4")>
    Public Shared Function RoundNumber_4Decimali(ByVal numero As Decimal) As Decimal
        Return RoundNumber(numero, 4)
    End Function

    <Obsolete("Usare la funzione ArrotondaVal")>
    Public Shared Function RoundNumber(ByVal numero As Decimal, ByVal numeroDecimali As Integer) As Decimal

        'controllo validita parametri
        If numeroDecimali < 0 Then
            Throw New Exception("Il parametro NumeroDecimali deve essere un intero positivo o 0")
        End If

        Dim segno As Integer = Math.Sign(numero)
        Dim numeroAbs As Decimal = Math.Abs(numero)

        If numeroAbs < 0.0000001 Then
            numeroAbs = 0
        End If


        'converto il numero in stringa
        Dim numeroStr As String = CStr(numeroAbs)
        Dim numeroArray() As String = numeroStr.Split({","c})

        Dim parteIntera As String = numeroArray(0)

        'se non ho la virgola ritorno la parte intera
        If numeroArray.Length = 1 Then
            Return ReimpostaSegno(CDec(parteIntera), segno)
        End If


        Dim parteDecimale As String = numeroArray(1)

        'se la parte decimale e' minore o uguale al numero di decimali voluti ritorno il numero originale
        If numeroDecimali >= parteDecimale.Length Then
            Return ReimpostaSegno(numeroAbs, segno)
        End If


        Dim decimaliVoluti As String = parteDecimale.Substring(0, numeroDecimali)
        If numeroDecimali = 0 Then
            'se numerodecimali=0 allora decimali voluti e' stringa vuota, quindi per evitare errori la pongo a 0
            decimaliVoluti = "0"
        End If

        Dim cifraArrotondamento As Integer = CInt(parteDecimale.Substring(numeroDecimali, 1))


        If (cifraArrotondamento >= 5) Then
            'se la cifra e' maggiore o uguale a 5 arrotondo al superiore
            If numeroDecimali = 0 Then
                'se il numero decimali voluti e' 0 allora devo arrotondare la parte intera
                parteIntera = CStr(CInt(parteIntera) + 1)
            Else

                'altrimenti  arrotondo il numero ParteDecimale
                decimaliVoluti = CStr(CInt(decimaliVoluti) + 1)

                Select Case decimaliVoluti.Length

                    Case numeroDecimali
                        'ok, non bisogna fare altro

                    Case Is > numeroDecimali
                        'arrotondando devo aggiungere 1 alla parte intera, ad esempio 0,9995. 3 cifre volute-> 999+1=10000 
                        'quindi avrei 4 cifre, devo aggiungere una unità
                        parteIntera = CStr(CInt(parteIntera) + 1)
                        decimaliVoluti = Right(decimaliVoluti, numeroDecimali)

                    Case Is < numeroDecimali
                        'se il numero di cifre è minore allora avevo degli 0 all'inizio del numero
                        'che facendo cint() sono spariti, ad es cint(004)=4
                        'quindi concateno gli zeri
                        decimaliVoluti = decimaliVoluti.PadLeft(numeroDecimali, CChar("0"))

                End Select

            End If
        Else
            'cifraArrotondamento < 5

        End If

        Dim numeroRitorno As Decimal = ReimpostaSegno(CDec(parteIntera & "," & decimaliVoluti), segno)

        Return numeroRitorno

    End Function

    Private Shared Function ReimpostaSegno(ByVal valoreAbs As Decimal, ByVal segno As Integer) As Decimal
        Return segno * valoreAbs
    End Function

    ' funzione usata nelle stampe
    '################################################################
    Public Shared Function Arrotonda(ByVal valore As Decimal, ByVal tipoArrotondamento As Integer) As Decimal

        Select Case tipoArrotondamento

            Case -1 'nessuno

                Return valore

            Case Else 'da 0 = unità a 4 = 4 decimali

                'in questo caso TipoArrotondamento corrisponde al numero di cifre decimali
                Return Math.Round(valore, tipoArrotondamento)

        End Select

    End Function

    '################################################################
    Public Shared Function ArrotondaVal(ByVal valore As Decimal, ByVal cifreArrotondamento As Integer, Optional FormatDecimal As Boolean = False) As Decimal

        Select Case cifreArrotondamento

            Case -1 'nessuno

                valore = CDec(valore)

            Case Else 'da 0 = unità a 4 = 4 decimali

                'in questo caso TipoArrotondamento corrisponde al numero di cifre decimali
                'mode = AwayFromZero: sostanzialmente arrotondamento "normale", da 0 a 4 arrotondato per difetto, da 5 a 9 arrotondato per eccesso
                'https://msdn.microsoft.com/en-us/library/9s0xa85y(v=vs.110).aspx

                valore = Decimal.Round(CDec(valore), cifreArrotondamento, MidpointRounding.AwayFromZero)

                'Importante: se con Decimal.Round non viene specificato un mode, il default è "ToEven" (cioè arrotondamento bancario)
                'https://msdn.microsoft.com/en-us/library/6be1edhb(v=vs.110).aspx

                'Se FormatDecimal è true aggiunge decimali in base a quante cifre
                'volevo arrotondare il valore
                If FormatDecimal Then
                    valore = CDec(FormatVal(CDec(valore), cifreArrotondamento))
                End If

        End Select

        Return valore

    End Function

    '################################################################
    Public Shared Function FormatVal(ByVal valore As Decimal, ByVal cifreFormat As Integer) As String

        Dim newFormatVal As String = String.Empty

        Select Case cifreFormat

            Case -1 'nessuno

                newFormatVal = CStr(valore)

            Case 0

                newFormatVal = Format(valore, "#,###,##0")

            Case 1

                newFormatVal = Format(valore, "#,###,##0.0")

            Case 2

                newFormatVal = Format(valore, "#,###,##0.00")

            Case 3

                newFormatVal = Format(valore, "#,###,##0.000")

            Case 4

                newFormatVal = Format(valore, "#,###,##0.0000")

        End Select

        Return newFormatVal

    End Function

    '################################################################
    Public Shared Function ArrotondaVal_Nothing(ByVal valore As Decimal?, ByVal cifre As Integer,Optional FormatDecimal As Boolean = False) As Decimal?
        If IsNothing(valore) Then
            Return Nothing
        Else
            Return ArrotondaVal(CDec(valore), cifre)
        End If
    End Function

    '################################################################
    Public Shared Function ArrotondaVal_0(ByVal valore As Decimal, Optional FormatDecimal As Boolean = False) As Decimal

        Return ArrotondaVal(CDec(valore), 0, FormatDecimal)

    End Function

    '################################################################
    Public Shared Function ArrotondaVal_1(ByVal valore As Decimal, Optional FormatDecimal As Boolean = False) As Decimal

        Return ArrotondaVal(CDec(valore), 1, FormatDecimal)

    End Function

    '################################################################
    Public Shared Function ArrotondaVal_2(ByVal valore As Decimal, Optional FormatDecimal As Boolean = False) As Decimal

        Return ArrotondaVal(CDec(valore), 2, FormatDecimal)

    End Function

    '################################################################
    Public Shared Function ArrotondaVal_3(ByVal valore As Decimal, Optional FormatDecimal As Boolean = False) As Decimal

        Return ArrotondaVal(CDec(valore), 3, FormatDecimal)

    End Function

    '################################################################
    Public Shared Function ArrotondaVal_4(ByVal valore As Decimal, Optional FormatDecimal As Boolean = False) As Decimal

        Return ArrotondaVal(CDec(valore), 4, FormatDecimal)

    End Function

    '################################################################
    Public Shared Function ArrotondaVal_6(ByVal valore As Decimal, Optional FormatDecimal As Boolean = False) As Decimal

        Return ArrotondaVal(CDec(valore), 6, FormatDecimal)

    End Function

    Private Shared ReadOnly rxScientific As New Regex("^(?<sign>-?)(?<head>\d+)(\.(?<tail>\d*?)0*)?E(?<exponent>[+\-]\d+)$", RegexOptions.IgnoreCase Or RegexOptions.ExplicitCapture Or RegexOptions.CultureInvariant, TimeSpan.FromSeconds(3))

    Public Shared Function ToFloatingPointString(value As Decimal) As String
        Return ToFloatingPointString(value, NumberFormatInfo.CurrentInfo)
    End Function

    Public Shared Function ToFloatingPointString(value As Decimal, formatInfo As NumberFormatInfo) As String

        Dim result As String = value.ToString()
        Dim resultXMatch As String = value.ToString().Replace(",", ".")

        'Dim result As String = value.ToString("r", NumberFormatInfo.CurrentInfo)
        'Dim resultXMatch As String = value.ToString("r", NumberFormatInfo.CurrentInfo).Replace(",", ".")


        Dim match As Match = rxScientific.Match(resultXMatch)
        If match.Success Then
            'Debug.WriteLine("Found scientific format: {0} => [{1}] [{2}] [{3}] [{4}]", result, match.Groups("sign"), match.Groups("head"), match.Groups("tail"), match.Groups("exponent"))
            Dim exponent As Integer = Integer.Parse(match.Groups("exponent").Value, NumberStyles.[Integer], NumberFormatInfo.CurrentInfo)
            Dim builder As New StringBuilder(result.Length + Math.Abs(exponent))
            builder.Append(match.Groups("sign").Value)
            If exponent >= 0 Then
                builder.Append(match.Groups("head").Value)
                Dim tail As String = match.Groups("tail").Value
                If exponent < tail.Length Then
                    builder.Append(tail, 0, exponent)
                    builder.Append(formatInfo.NumberDecimalSeparator)
                    builder.Append(tail, exponent, tail.Length - exponent)
                Else
                    builder.Append(tail)
                    builder.Append("0"c, exponent - tail.Length)
                End If
            Else
                builder.Append("0"c)
                builder.Append(formatInfo.NumberDecimalSeparator)
                builder.Append("0"c, (-exponent) - 1)
                builder.Append(match.Groups("head").Value)
                builder.Append(match.Groups("tail").Value)
            End If
            result = builder.ToString()
        Else

            result = value.ToString

        End If
        Return result
    End Function


    '#####################################################################################################
    'converte la qta da kg a quintali e arrotonda a 2 decimali
    Public Shared Function Converti_daKG_aQL_ArrotondaVal2(ByVal Qta As Decimal) As Decimal

        Qta = Qta / 100

        Qta = ArrotondaVal_2(Qta)

        Return Qta

    End Function

End Class

