Public Class AzaleaCode

    Public Shared Function Azalea_UPC_A_checkDigit(ByVal UPCnumber As String) As String
        ' UPCTools 16may12 jwhiting
        ' Copyright 2012 Azalea Software, Inc. All rights reserved. www.azalea.com

        ' Calculating the UPC version A checkdigit in Excel
        ' Your input, UPCnumber, is a string consisting of an 11-, 12-, or 14-digit number.

        Dim checkDigitSubtotal As Integer  ' a check digit throwaway

        ' Possible valid input includes 11-digits (no check digit), 12-digits (with check digit), or 14-digits (GTIN).
        Select Case Len(UPCnumber)
            Case 11
                ' We're going to do the UPC check digit calculation because the input doesn't have it.
            Case 12
                ' Let's strip your check digit and calculate ours from scratch. (Nothing personal.)
                UPCnumber = Left(UPCnumber, 11)
            Case 14
                ' GTIN input, but we're going to strip the leading 2 and the last characters to extract the 11-digit input without check digit.
                UPCnumber = Mid(UPCnumber, 3, 11)
            Case Else
                ' Your error handling goes here...
        End Select

        ' Now we need to do the UPC A check digit calculation.
        ' Add up the numbers in the odd positions left to right. Multiple the result by 3.
        ' Add up the numbers in the even positions. Now add the first subtotal to the second.
        ' The UPC barcode check digit is the single digit number makes the total a multiple of 10.
        checkDigitSubtotal = (Val(Left(UPCnumber, 1))) + (Val(Mid(UPCnumber, 3, 1))) + (Val(Mid(UPCnumber, 5, 1))) + (Val(Mid(UPCnumber, 7, 1))) + (Val(Mid(UPCnumber, 9, 1))) + (Val(Right(UPCnumber, 1)))
        checkDigitSubtotal = (3 * checkDigitSubtotal) + (Val(Mid(UPCnumber, 2, 1))) + (Val(Mid(UPCnumber, 4, 1))) + (Val(Mid(UPCnumber, 6, 1))) + (Val(Mid(UPCnumber, 8, 1))) + (Val(Mid(UPCnumber, 10, 1)))

        Return Right(Str(300 - checkDigitSubtotal), 1)

        ' Excel: B1=Azalea_UPC_A_checkDigit(A1)
        ' Or put another way, yourContainer.text=Azalea_UPC_A_checkDigit(yourInputString)

    End Function

    Public Shared Function Azalea_ISBN13(ByVal ISBNnumber As String, ByVal Price As String) As String
        ' UPCTools 27may15 jwhiting
        ' Copyright 2015 Azalea Software, Inc. All rights reserved. www.azalea.com

        ' Creating an ISBN-13 barcode in Excel
        ' ISBNnumber is a 13-digit ISBN number and Price is the retail price used in the 5-digit supplemental code.
        ' ISBNnumber has no hyphens or spaces. Strip them out if necessary to make an 11-digit string.

        Dim ISBN As String                  ' a bucket for the ISBN string
        Dim checkDigitSubtotal As Integer   ' a check digit subtotal
        Dim checkDigit As String            ' a check digit throwaway
        Dim temp As String                  ' a temporary placeholder

        ' drop the ISBN check digit
        ISBN = Left(ISBNnumber, 12)

        ' Let's do the EAN-13 check digit calculation
        checkDigitSubtotal = 3 * (Val(Mid(ISBN, 2, 1)) + Val(Mid(ISBN, 4, 1)) + Val(Mid(ISBN, 6, 1)) + Val(Mid(ISBN, 8, 1)) + Val(Mid(ISBN, 10, 1)) + Val(Right(ISBN, 1)))
        checkDigitSubtotal = checkDigitSubtotal + Val(Left(ISBN, 1)) + Val(Mid(ISBN, 3, 1)) + Val(Mid(ISBN, 5, 1)) + Val(Mid(ISBN, 7, 1)) + Val(Mid(ISBN, 9, 1)) + Val(Mid(ISBN, 11, 1))
        checkDigit = Right(Str(300 - checkDigitSubtotal), 1)

        ' Build the output string.
        ' Does the ISBN number begin with 978 or 979?
        If Mid(ISBN, 3, 1) = "8" Then
            temp = Chr(203) & "|xHS" & EvenBar(Mid(ISBN, 4, 1))
        Else
            temp = Chr(203) & "|xHT" & EvenBar(Mid(ISBN, 4, 1))
        End If
        temp = temp + OddBar(Mid(ISBN, 5, 1))
        temp = temp + EvenBar(Mid(ISBN, 6, 1))
        temp = temp + OddBar(Mid(ISBN, 7, 1)) & "y" & Right(ISBN, 5) + checkDigit & "zv"

        ' Calculate the 5-digit supplemental check digit
        checkDigitSubtotal = 3 * (Val(Left(Price, 1)) + Val(Mid(Price, 3, 1)) + Val(Right(Price, 1)))
        checkDigitSubtotal = checkDigitSubtotal + (9 * (Val(Mid(Price, 2, 1)) + Val(Mid(Price, 4, 1))))
        checkDigit = Right(Str(checkDigitSubtotal), 1)

        ' The rest of the output depends upon the checkDigit value.
        Select Case checkDigit
            Case "0" ' EEEOO
                temp = temp + EvenS(Left(Price, 1)) & ":" & EvenS(Mid(Price, 2, 1))
                Return (temp & ":" & OddS(Mid(Price, 3, 1)) & ":" & OddS(Mid(Price, 4, 1)) & ":" & OddS(Right(Price, 1)))
            Case "1" ' EOEOO
                temp = temp + EvenS(Left(Price, 1)) & ":" & OddS(Mid(Price, 2, 1))
                Return (temp & ":" & EvenS(Mid(Price, 3, 1)) & ":" & OddS(Mid(Price, 4, 1)) & ":" & OddS(Right(Price, 1)))
            Case "2" ' EOOEO
                temp = temp + EvenS(Left(Price, 1)) & ":" & OddS(Mid(Price, 2, 1))
                Return (temp & ":" & OddS(Mid(Price, 3, 1)) & ":" & EvenS(Mid(Price, 4, 1)) & ":" & OddS(Right(Price, 1)))
            Case "3" ' EOOOE
                temp = temp + EvenS(Left(Price, 1)) & ":" & OddS(Mid(Price, 2, 1))
                Return (temp & ":" & OddS(Mid(Price, 3, 1)) & ":" & OddS(Mid(Price, 4, 1)) & ":" & EvenS(Right(Price, 1)))
            Case "4" ' OEEOO
                temp = temp + OddS(Left(Price, 1)) & ":" & EvenS(Mid(Price, 2, 1))
                Return (temp & ":" & EvenS(Mid(Price, 3, 1)) & ":" & OddS(Mid(Price, 4, 1)) & ":" & OddS(Right(Price, 1)))
            Case "5" ' OOEEO
                temp = temp + OddS(Left(Price, 1)) & ":" & OddS(Mid(Price, 2, 1))
                Return (temp & ":" & EvenS(Mid(Price, 3, 1)) & ":" & EvenS(Mid(Price, 4, 1)) & ":" & OddS(Right(Price, 1)))
            Case "6" ' OOOEE
                temp = temp + OddS(Left(Price, 1)) & ":" & OddS(Mid(Price, 2, 1))
                Return (temp & ":" & OddS(Mid(Price, 3, 1)) & ":" & EvenS(Mid(Price, 4, 1)) & ":" & EvenS(Right(Price, 1)))
            Case "7" ' OEOEO
                temp = temp + OddS(Left(Price, 1)) & ":" & EvenS(Mid(Price, 2, 1))
                Return (temp & ":" & OddS(Mid(Price, 3, 1)) & ":" & EvenS(Mid(Price, 4, 1)) & ":" & OddS(Right(Price, 1)))
            Case "8" ' OEOOE
                temp = temp + OddS(Left(Price, 1)) & ":" & EvenS(Mid(Price, 2, 1))
                Return (temp & ":" & OddS(Mid(Price, 3, 1)) & ":" & OddS(Mid(Price, 4, 1)) & ":" & EvenS(Right(Price, 1)))
            Case "9" ' OOEOE
                temp = temp + OddS(Left(Price, 1)) & ":" & OddS(Mid(Price, 2, 1))
                Return (temp & ":" & EvenS(Mid(Price, 3, 1)) & ":" & OddS(Mid(Price, 4, 1)) & ":" & EvenS(Right(Price, 1)))
        End Select

        ' The output, Azalea_ISBN13, is formatted using one of Azalea Software's UPC fonts.
        ' Excel: B1=Azalea_ISBN13(A1)

        ' Or put another way, yourContainer.text=Azalea_ISBN13(yourInputString)

    End Function

    Private Shared Function OddS(ByVal theString As String) As String
        ' UPCTools 27may15 jwhiting
        ' Copyright 2015 Azalea Software, Inc. All rights reserved. www.azalea.com
        Select Case theString
            Case "0"
                Return "!"
            Case "1"
                Return Chr(34)
            Case "2"
                Return "#"
            Case "3"
                Return "$"
            Case "4"
                Return "%"
            Case "5"
                Return "&"
            Case "6"
                Return "'"
            Case "7"
                Return "("
            Case "8"
                Return ")"
            Case "9"
                Return "*"
        End Select

    End Function

    Private Shared Function EvenS(ByVal theString As String) As String
        ' UPCTools 27may15 jwhiting
        ' Copyright 2015 Azalea Software, Inc. All rights reserved. www.azalea.com
        Select Case theString
            Case "0"
                Return "+"
            Case "1"
                Return ","
            Case "2"
                Return "-"
            Case "3"
                Return "."
            Case "4"
                Return "/"
            Case "5"
                Return ";"
            Case "6"
                Return "<"
            Case "7"
                Return "="
            Case "8"
                Return ">"
            Case "9"
                Return "^"
        End Select
    End Function


    Public Shared Function Azalea_UPC_E(ByVal UPCnumber As String) As String
        ' UPCTools 16may12 jwhiting
        ' Copyright 2012 Azalea Software, Inc. All rights reserved. www.azalea.com

        ' Creating a UPC version E in Excel
        ' The input, UPCnumber, is either the 6 digits from an existing UPC version E symbol,
        ' or a 10-digit product number with an assumed first digit of "0".
        ' If your input is 11 digits, strip the leading "0": UPCnumber = Left(UPCnumber, 10)


        Dim temp1 As String             ' a temporary placeholder
        Dim temp2 As String             ' another temporary placeholder
        Dim final As String             ' output without the check digit or start bar
        Dim CheckDigit As Integer       ' the check digit itself

        ' Is the input a 6-digit string or a 10-digit string?
        If Len(UPCnumber) = 6 Then      ' OK, we've got a 6-digit input string from an existing version E symbol.
            Select Case Val(Right(Trim(UPCnumber), 1))
                Case 0
                    temp1 = Left(UPCnumber, 2) + "00000" + Mid(UPCnumber, 3, 3)
                Case 1
                    temp1 = Left(UPCnumber, 2) + "10000" + Mid(UPCnumber, 3, 3)
                Case 2
                    temp1 = Left(UPCnumber, 2) + "20000" + Mid(UPCnumber, 3, 3)
                Case 3
                    temp1 = Left(UPCnumber, 3) + "00000" + Mid(UPCnumber, 4, 2)
                Case 4
                    temp1 = Left(UPCnumber, 4) + "00000" + Mid(UPCnumber, 5, 1)
                Case 5 To 9
                    temp1 = Left(UPCnumber, 5) + "0000" + Right(UPCnumber, 1)
            End Select
        Else                            ' The input is already 10 digits long.
            temp1 = UPCnumber
        End If

        ' Let's assemble final from manufacturer number and item number according to version E rules. (Arcane terms I know.)
        If Mid(temp1, 5, 1) <> "0" Then
            final = Left(temp1, 5) + Right(temp1, 1)
        ElseIf Mid(temp1, 5, 1) = "0" And Mid(temp1, 4, 1) <> "0" Then
            final = Left(temp1, 4) + Right(temp1, 1) + "4"
        ElseIf Mid(temp1, 4, 2) = "00" And Mid(temp1, 3, 1) > "2" Then
            final = Left(temp1, 3) + Right(temp1, 2) + "3"
        ElseIf Mid(temp1, 4, 2) = "00" And Mid(temp1, 3, 1) < "3" Then
            final = Left(temp1, 2) + Right(temp1, 3) + Mid(temp1, 3, 1)
        End If

        ' Time to do the check digit calculation.
        ' Add up the numbers in the odd positions left to right. Multiple the result by 3.
        ' Add up the numbers in the even positions. Now add the first subtotal to the second.
        ' The UPC barcode check digit is the single digit number makes the total a multiple of 10.
        CheckDigit = 3 * (Val(Mid(temp1, 2, 1)) + Val(Mid(temp1, 4, 1)) + Val(Mid(temp1, 6, 1)) + Val(Mid(temp1, 8, 1)) + Val(Mid(temp1, 10, 1)))
        CheckDigit = CheckDigit + Val(Mid(temp1, 1, 1)) + Val(Mid(temp1, 3, 1)) + Val(Mid(temp1, 5, 1)) + Val(Mid(temp1, 7, 1)) + Val(Mid(temp1, 9, 1))
        CheckDigit = 10 - (CheckDigit Mod 10)
        If CheckDigit = 10 Then CheckDigit = 0

        ' Assemble output string with the assumed first digit on "0" and the left guard bars.
        ' The first number, left(final,1)), always has even parity
        temp2 = "U|x" & EvenBar(Left(final, 1))

        ' The variuos parity options for mid(final, 2, 4) depend on the check digit value.
        Select Case CheckDigit
            Case "0"  ' EEOOO
                temp2 = temp2 + EvenBar(Mid(final, 2, 1))
                temp2 = temp2 + EvenBar(Mid(final, 3, 1))
                temp2 = temp2 + OddBar(Mid(final, 4, 1))
                temp2 = temp2 + OddBar(Mid(final, 5, 1))
                temp2 = temp2 + OddBar(Mid(final, 6, 1))
                Return (temp2 & "wU")
            Case "1"  ' EOEOO
                temp2 = temp2 + EvenBar(Mid(final, 2, 1))
                temp2 = temp2 + OddBar(Mid(final, 3, 1))
                temp2 = temp2 + EvenBar(Mid(final, 4, 1))
                temp2 = temp2 + OddBar(Mid(final, 5, 1))
                temp2 = temp2 + OddBar(Mid(final, 6, 1))
                Return (temp2 & "w[")
            Case "2"  ' EOOEO
                temp2 = temp2 + EvenBar(Mid(final, 2, 1))
                temp2 = temp2 + OddBar(Mid(final, 3, 1))
                temp2 = temp2 + OddBar(Mid(final, 4, 1))
                temp2 = temp2 + EvenBar(Mid(final, 5, 1))
                temp2 = temp2 + OddBar(Mid(final, 6, 1))
                Return (temp2 & "wV")
            Case "3"  ' EOOOE
                temp2 = temp2 + EvenBar(Mid(final, 2, 1))
                temp2 = temp2 + OddBar(Mid(final, 3, 1))
                temp2 = temp2 + OddBar(Mid(final, 4, 1))
                temp2 = temp2 + OddBar(Mid(final, 5, 1))
                temp2 = temp2 + EvenBar(Mid(final, 6, 1))
                Return (temp2 & "wW")
            Case "4"  ' OEEOO
                temp2 = temp2 + OddBar(Mid(final, 2, 1))
                temp2 = temp2 + EvenBar(Mid(final, 3, 1))
                temp2 = temp2 + EvenBar(Mid(final, 4, 1))
                temp2 = temp2 + OddBar(Mid(final, 5, 1))
                temp2 = temp2 + OddBar(Mid(final, 6, 1))
                Return (temp2 & "wX")
            Case "5"  ' OOEEO
                temp2 = temp2 + OddBar(Mid(final, 2, 1))
                temp2 = temp2 + OddBar(Mid(final, 3, 1))
                temp2 = temp2 + EvenBar(Mid(final, 4, 1))
                temp2 = temp2 + EvenBar(Mid(final, 5, 1))
                temp2 = temp2 + OddBar(Mid(final, 6, 1))
                Return (temp2 & "wY")
            Case "6"  ' OOOEE
                temp2 = temp2 + OddBar(Mid(final, 2, 1))
                temp2 = temp2 + OddBar(Mid(final, 3, 1))
                temp2 = temp2 + OddBar(Mid(final, 4, 1))
                temp2 = temp2 + EvenBar(Mid(final, 5, 1))
                temp2 = temp2 + EvenBar(Mid(final, 6, 1))
                Return (temp2 & "wZ")
            Case "7"  ' OEOEO
                temp2 = temp2 + OddBar(Mid(final, 2, 1))
                temp2 = temp2 + EvenBar(Mid(final, 3, 1))
                temp2 = temp2 + OddBar(Mid(final, 4, 1))
                temp2 = temp2 + EvenBar(Mid(final, 5, 1))
                temp2 = temp2 + OddBar(Mid(final, 6, 1))
                Return (temp2 & "wu")
            Case "8"  ' OEOOE
                temp2 = temp2 + OddBar(Mid(final, 2, 1))
                temp2 = temp2 + EvenBar(Mid(final, 3, 1))
                temp2 = temp2 + OddBar(Mid(final, 4, 1))
                temp2 = temp2 + OddBar(Mid(final, 5, 1))
                temp2 = temp2 + EvenBar(Mid(final, 6, 1))
                Return (temp2 & "w\")
            Case "9"  ' OOEOE
                temp2 = temp2 + OddBar(Mid(final, 2, 1))
                temp2 = temp2 + OddBar(Mid(final, 3, 1))
                temp2 = temp2 + EvenBar(Mid(final, 4, 1))
                temp2 = temp2 + OddBar(Mid(final, 5, 1))
                temp2 = temp2 + EvenBar(Mid(final, 6, 1))
                Return (temp2 & "w]")
        End Select

        ' The output, Azalea_UPC_E, needs to be formatted in one of Azalea Software's UPC fonts.
        ' We recommend UPCTallThin at 73 points.

        ' Excel: B1=Azalea_UPC_E(A1)
        ' Or put another way, yourContainer.text=Azalea_UPC_E(yourInputString)

    End Function



    Public Shared Function Azalea_UPC_A(ByVal UPCnumber As String) As String
        ' UPCTools 16may12 jwhiting
        ' Copyright 2012 Azalea Software, Inc. All rights reserved. www.azalea.com

        ' Creating a UPC version A in Excel
        ' Your input, UPCnumber, is a string consisting of an 11-, 12-, or 14-digit number.
        ' company prefix + product ID, company prefix + product ID + check digit, or GTIN (Global Trade Identification Number)

        Dim checkDigitSubtotal As Integer  ' a check digit throwaway
        Dim i As Integer                   ' our loop counter
        Dim checkDigit As String           ' the check digit itself
        Dim temp As String                 ' a temporary placeholder

        ' Possible valid input includes 11-digits (no check digit), 12-digits (with check digit), or 14-digits (GTIN).
        Select Case Len(UPCnumber)
            Case 11
                ' We're going to do the UPC check digit calculation because the input doesn't have it.
            Case 12
                ' Let's strip your check digit and calculate it from scratch.
                UPCnumber = Left(UPCnumber, 11)
            Case 14
                ' GTIN input, but we're going to strip the leading 2 and the last characters to extract the 11-digit input without check digit.
                UPCnumber = Mid(UPCnumber, 3, 11)
            Case Else
                ' Your error handling goes here...
        End Select

        ' Build the output string beginning with the first human-readable digit, a spacer, the left guard bars, and the bars for the first character.
        Select Case Left(UPCnumber, 1)
            Case "0"
                temp = "U|xa"
            Case "1"
                temp = "[|xb"
            Case "2"
                temp = "V|xc"
            Case "3"
                temp = "W|xd"
            Case "4"
                temp = "X|xe"
            Case "5"
                temp = "Y|xf"
            Case "6"
                temp = "Z|xg"
            Case "7"
                temp = "u|xh"
            Case "8"
                temp = "\|xi"
            Case "9"
                temp = "]|xj"
        End Select

        ' Continue with the 5 numbers in the left notch.
        For i = 2 To 6 Step 1
            temp = temp + Chr(65 + (Val(Mid(UPCnumber, i, 1))))
        Next i

        ' Now we need to do the UPC A check digit calculation.
        ' Add up the numbers in the odd positions left to right. Multiple the result by 3.
        ' Add up the numbers in the even positions. Now add the first subtotal to the second.
        ' The UPC barcode check digit is the single digit number makes the total a multiple of 10.
        checkDigitSubtotal = (Val(Left(UPCnumber, 1))) + (Val(Mid(UPCnumber, 3, 1))) + (Val(Mid(UPCnumber, 5, 1))) + (Val(Mid(UPCnumber, 7, 1))) + (Val(Mid(UPCnumber, 9, 1))) + (Val(Right(UPCnumber, 1)))
        checkDigitSubtotal = (3 * checkDigitSubtotal) + (Val(Mid(UPCnumber, 2, 1))) + (Val(Mid(UPCnumber, 4, 1))) + (Val(Mid(UPCnumber, 6, 1))) + (Val(Mid(UPCnumber, 8, 1))) + (Val(Mid(UPCnumber, 10, 1)))
        checkDigit = Right(Str(300 - checkDigitSubtotal), 1)

        ' Continue to build the output string by adding the center guard bars, 5 digits in the right notch, check digit and right guard bars.
        temp = temp & "y" & Right(UPCnumber, 5) + Chr(107 + (Val(checkDigit))) & "z"

        ' Lastly let's add the human readable for the check digit in the lower right corner.
        Select Case checkDigit
            Case "0"
                Return (temp & "U")
            Case "1"
                Return (temp & "[")
            Case "2"
                Return (temp & "V")
            Case "3"
                Return (temp & "W")
            Case "4"
                Return (temp & "X")
            Case "5"
                Return (temp & "Y")
            Case "6"
                Return (temp & "Z")
            Case "7"
                Return (temp & "u")
            Case "8"
                Return (temp & "\")
            Case "9"
                Return (temp & "]")
        End Select

        ' The output, Azalea_UPC_A, needs to be formatted in one of Azalea Software's UPC fonts.
        ' We recommend UPCTallThin at 73 points.

        ' Excel: B1=Azalea_UPC_A(A1)
        ' Or put another way, yourContainer.text=Azalea_UPC_A(yourInputString)

    End Function



    Public Shared Function AzaleaSoftware_EAN8(ByVal EAN As String) As String
        ' UPCTools 16may12 jwhiting
        ' Copyright 2012 Azalea Software, Inc. All rights reserved. www.azalea.com
        ' The input, EAN, is a 7-digit string to be converted into an EAN-8 barcode.

        Dim checkDigitSubtotal As Integer ' reset-able throw-away variable
        Dim checkDigit As String      ' check digit itself
        Dim i As Integer          ' just a counter
        Dim temp As String            ' placeholder

        ' do the check digit
        checkDigitSubtotal = 3 * (Val(Left(EAN, 1)) + Val(Mid(EAN, 3, 1)) + Val(Mid(EAN, 5, 1)) + Val(Right(EAN, 1)))
        checkDigitSubtotal = checkDigitSubtotal + Val(Mid(EAN, 2, 1)) + Val(Mid(EAN, 4, 1)) + Val(Mid(EAN, 6, 1))
        checkDigit = Right(Str(300 - checkDigitSubtotal), 1)

        ' left guard bar
        temp = "|x"

        ' build the symbol using set A
        For i = 1 To 4
            temp = temp + OddBar(Mid(EAN, i, 1))
        Next 'I

        ' center guard bar & right half of symbol using set C (0-9), checkDigit & right guard bar
        Return (temp & "y" & Mid(EAN, 5, 3) + checkDigit & "z")

        ' The output, AzaleaSoftware_EAN8, is formatted using one of Azalea Software's UPC fonts.
        ' For example, B1=AzaleaSoftware_EAN8(A1)
        ' Or put another way, yourContainer.text=AzaleaSoftware_EAN8(yourInputString)

    End Function



    Public Shared Function Azalea_EAN13(ByVal EAN As String) As String
        ' UPCTools 28oct13 jwhiting
        ' Copyright 2013 Azalea Software, Inc. All rights reserved. www.azalea.com

        ' Creating an EAN-13 in Excel
        ' Your input, EAN, is a string consisting of a 12-digit number.

        Dim checkDigitSubtotal As Integer     ' a check digit subtotal
        Dim checkDigit As String              ' the check digit itself
        Dim temp As String                    ' a temporary placeholder

        ' Calculate the EAN-13 check digit.
        checkDigitSubtotal = 3 * (Val(Mid(EAN, 2, 1)) + Val(Mid(EAN, 4, 1)) + Val(Mid(EAN, 6, 1)) + Val(Mid(EAN, 8, 1)) + Val(Mid(EAN, 10, 1)) + Val(Right(EAN, 1)))
        checkDigitSubtotal = checkDigitSubtotal + Val(Left(EAN, 1)) + Val(Mid(EAN, 3, 1)) + Val(Mid(EAN, 5, 1)) + Val(Mid(EAN, 7, 1)) + Val(Mid(EAN, 9, 1)) + Val(Mid(EAN, 11, 1))
        checkDigit = Right(Str(300 - checkDigitSubtotal), 1)

        ' output string = the 1st character's human-readable, L guard bars & odd parity of 1st digit
        Select Case Left(EAN, 1)
            Case "0"
                temp = "¶" & "|x" & OddBar(Mid(EAN, 2, 1))
            Case "1"
                temp = "·" & "|x" & OddBar(Mid(EAN, 2, 1))
            Case "2"
                temp = "Ä" & "|x" & OddBar(Mid(EAN, 2, 1))
            Case "3"
                temp = "Å" & "|x" & OddBar(Mid(EAN, 2, 1))
            Case "4"
                temp = "Æ" & "|x" & OddBar(Mid(EAN, 2, 1))
            Case "5"
                temp = "Ç" & "|x" & OddBar(Mid(EAN, 2, 1))
            Case "6"
                temp = "È" & "|x" & OddBar(Mid(EAN, 2, 1))
            Case "7"
                temp = "É" & "|x" & OddBar(Mid(EAN, 2, 1))
            Case "8"
                temp = "Ê" & "|x" & OddBar(Mid(EAN, 2, 1))
            Case "9"
                temp = "Ë" & "|x" & OddBar(Mid(EAN, 2, 1))
        End Select

        ' Build the remainder of left half of symbol's parity is based on 1st digit
        Select Case Left(EAN, 1)
            Case "0"  ' OOOOO
                temp = temp + OddBar(Mid(EAN, 3, 1))
                temp = temp + OddBar(Mid(EAN, 4, 1))
                temp = temp + OddBar(Mid(EAN, 5, 1))
                temp = temp + OddBar(Mid(EAN, 6, 1))
                temp = temp + OddBar(Mid(EAN, 7, 1))
            Case "1"  ' OEOEE
                temp = temp + OddBar(Mid(EAN, 3, 1))
                temp = temp + EvenBar(Mid(EAN, 4, 1))
                temp = temp + OddBar(Mid(EAN, 5, 1))
                temp = temp + EvenBar(Mid(EAN, 6, 1))
                temp = temp + EvenBar(Mid(EAN, 7, 1))
            Case "2"  ' OEEOE
                temp = temp + OddBar(Mid(EAN, 3, 1))
                temp = temp + EvenBar(Mid(EAN, 4, 1))
                temp = temp + EvenBar(Mid(EAN, 5, 1))
                temp = temp + OddBar(Mid(EAN, 6, 1))
                temp = temp + EvenBar(Mid(EAN, 7, 1))
            Case "3"  ' OEEEO
                temp = temp + OddBar(Mid(EAN, 3, 1))
                temp = temp + EvenBar(Mid(EAN, 4, 1))
                temp = temp + EvenBar(Mid(EAN, 5, 1))
                temp = temp + EvenBar(Mid(EAN, 6, 1))
                temp = temp + OddBar(Mid(EAN, 7, 1))
            Case "4"  ' EOOEE
                temp = temp + EvenBar(Mid(EAN, 3, 1))
                temp = temp + OddBar(Mid(EAN, 4, 1))
                temp = temp + OddBar(Mid(EAN, 5, 1))
                temp = temp + EvenBar(Mid(EAN, 6, 1))
                temp = temp + EvenBar(Mid(EAN, 7, 1))
            Case "5"  ' EEOOE
                temp = temp + EvenBar(Mid(EAN, 3, 1))
                temp = temp + EvenBar(Mid(EAN, 4, 1))
                temp = temp + OddBar(Mid(EAN, 5, 1))
                temp = temp + OddBar(Mid(EAN, 6, 1))
                temp = temp + EvenBar(Mid(EAN, 7, 1))
            Case "6"  ' EEEOO
                temp = temp + EvenBar(Mid(EAN, 3, 1))
                temp = temp + EvenBar(Mid(EAN, 4, 1))
                temp = temp + EvenBar(Mid(EAN, 5, 1))
                temp = temp + OddBar(Mid(EAN, 6, 1))
                temp = temp + OddBar(Mid(EAN, 7, 1))
            Case "7"  ' EOEOE
                temp = temp + EvenBar(Mid(EAN, 3, 1))
                temp = temp + OddBar(Mid(EAN, 4, 1))
                temp = temp + EvenBar(Mid(EAN, 5, 1))
                temp = temp + OddBar(Mid(EAN, 6, 1))
                temp = temp + EvenBar(Mid(EAN, 7, 1))
            Case "8"  ' EOEEO
                temp = temp + EvenBar(Mid(EAN, 3, 1))
                temp = temp + OddBar(Mid(EAN, 4, 1))
                temp = temp + EvenBar(Mid(EAN, 5, 1))
                temp = temp + EvenBar(Mid(EAN, 6, 1))
                temp = temp + OddBar(Mid(EAN, 7, 1))
            Case "9"  ' EEOEO
                temp = temp + EvenBar(Mid(EAN, 3, 1))
                temp = temp + EvenBar(Mid(EAN, 4, 1))
                temp = temp + OddBar(Mid(EAN, 5, 1))
                temp = temp + EvenBar(Mid(EAN, 6, 1))
                temp = temp + OddBar(Mid(EAN, 7, 1))
        End Select

        ' Add the center guard bars & the build right half of symbol using set C (0-9)
        Return (temp & "y" & Mid(EAN, 8, 5) + checkDigit & "z")

        ' The output, Azalea_EAN13, needs to be formatted in one of Azalea Software's UPC fonts.
        ' We recommend UPCTallThin at 73 points.

        ' Excel: B1=Azalea_EAN13(A1)
        ' Or put another way, yourContainer.text=Azalea_EAN13(yourInputString)


    End Function

    Private Shared Function OddBar(ByVal theString As String) As String
        ' UPCTools 28oct13 jwhiting
        ' Copyright 2013 Azalea Software, Inc. All rights reserved. www.azalea.com
        Return Chr(65 + (Val(theString)))
    End Function

    Private Shared Function EvenBar(ByVal theString As String) As String
        ' UPCTools 28oct13 jwhiting
        ' Copyright 2013 Azalea Software, Inc. All rights reserved. www.azalea.com
        Return Chr(75 + Val(theString))
    End Function




    Public Shared Function Azalea_Code_128_C(ByVal yourData As String) As String
        ' C128Tools 29may12 jwhiting
        ' Copyright 2012 Azalea Software, Inc. All rights reserved. www.azalea.com

        ' Creating a Code 128 code set C barcode using C128Tools.
        ' Your input, yourData, is a string to be encoded as a Code 128 code set C symbol.
        ' yourData must be the Code 128 code set C character set. Input error checking is your responsibility.

        Dim temp As String                 ' a temporary placeholder
        Dim checkDigitSubtotal As Integer  ' a check digit throwaway
        Dim i As Integer                   ' our loop counter
        Dim fontString As String           ' a temporary placeholder
        Dim chunk As String                ' loop chunk

        ' seed the variables
        fontString = Chr(183)                   ' code set C start glyph
        checkDigitSubtotal = 105                ' code set C start checkdigit value
        temp = yourData                         ' 2 character chunks

        ' pad odd length input with a leading zero goesHere PRN

        ' Calculate the Code 128 mod 103 check digit
        For i = 1 To Len(yourData) / 2
            chunk = Left$(temp, 2)
            checkDigitSubtotal = checkDigitSubtotal + Val(chunk) * i
            Select Case Val(chunk)
                Case 0
                    fontString = fontString + Chr(206)
                Case 1 To 93
                    fontString = fontString & Chr(Val(chunk) + 32)
                Case Is > 93
                    fontString = fontString & Chr(Val(chunk) + 103)
            End Select
            temp = Right$(temp, Len(temp) - 2)
        Next i
        checkDigitSubtotal = checkDigitSubtotal Mod 103

        ' Put together the final output string
        ' code set C start bar, the encoded string, check digit, stop bar
        Select Case checkDigitSubtotal
            Case 0
                Return (fontString & Chr(206) & Chr(196))
            Case 1 To 93
                Return (fontString & Chr(checkDigitSubtotal + 32) & Chr(196))
            Case Is > 93
                Return (fontString & Chr(checkDigitSubtotal + 103) & Chr(196))
        End Select

    End Function



    Public Shared Function Azalea_Code_128_B(ByVal yourData As String) As String
        ' C128Tools 29may12 jwhiting
        ' Copyright 2012 Azalea Software, Inc. All rights reserved. www.azalea.com

        ' Creating a Code 128 code set B barcode using C128Tools.
        ' Your input, yourData, is a string to be encoded as a Code 128 code set B symbol.
        ' yourData must be the Code 128 code set B character set. Input error checking is your responsibility.

        Dim temp As String                 ' a temporary placeholder
        Dim chunk As String                ' loop chunk
        Dim i As Integer                   ' our loop counter
        Dim checkDigitSubtotal As Integer  ' a check digit throwaway
        Dim e As Integer                   ' a placeholder variable

        ' seed the variables
        temp = Chr(182)                   ' code set B start glyph
        checkDigitSubtotal = 104          ' code set B start checkdigit value

        ' map the input string into the C128Tools character set
        For i = 1 To Len(yourData) Step 1
            chunk = Mid(yourData, i, 1)
            Select Case Asc(chunk)
                ' map from above ASCII 200 to the actual character assignments
                Case Is > 200
                    temp = temp & Chr(Asc(chunk) - 35)
                    ' The space character is at ASCII 194 because TrueType
                    ' doesn't allow a glyph in the ASCII 32 slot
                Case Is = 32
                    temp = temp & Chr(206)
                Case Else
                    temp = temp & chunk
            End Select
        Next i

        ' Calculate the Code 128 mod 103 check digit
        For i = 1 To Len(yourData)
            e = Asc(Mid(yourData, i, 1)) - 32
            If e <> 142 Then
                checkDigitSubtotal = checkDigitSubtotal + (e * i)
            End If
        Next i
        checkDigitSubtotal = checkDigitSubtotal Mod 103

        ' Put together the final output string
        ' code set A start bar, the encoded string, check digit, stop bar
        Select Case checkDigitSubtotal
            Case 0
                Return (temp & Chr(206) & Chr(196))
            Case 1 To 93
                Return (temp & Chr(checkDigitSubtotal + 32) & Chr(196))
            Case Is > 93
                Return (temp & Chr(checkDigitSubtotal + 103) & Chr(196))
        End Select

    End Function

    Public Shared Function Azalea_Code_128_A(ByVal yourData As String) As String
        ' C128Tools 29may12 jwhiting
        ' Copyright 2012 Azalea Software, Inc. All rights reserved. www.azalea.com

        ' Creating a Code 128 code set A barcode using C128Tools.
        ' Your input, yourData, is a string to be encoded as a Code 128 code set A symbol.
        ' yourData must be the Code 128 code set A character set. Input error checking is your responsibility.

        Dim temp As String                 ' a temporary placeholder
        Dim chunk As String                ' loop chunk
        Dim i As Integer                   ' our loop counter
        Dim checkDigitSubtotal As Integer  ' a check digit throwaway
        Dim e As Integer                   ' a placeholder variable

        ' seed the variables
        temp = Chr(181)                   ' code set A start glyph
        checkDigitSubtotal = 103                ' code set A start checkdigit value

        ' map the input string into the C128Tools character set
        For i = 1 To Len(yourData) Step 1
            chunk = Mid(yourData, i, 1)
            Select Case Asc(chunk)
                ' map from above ASCII 182 placeholders to the font's character assignments
                Case Is > 95
                    temp = temp & Chr(Asc(chunk) - 66)
                Case Is = 32 ' move the space character
                    temp = temp & Chr(206)
                Case Else
                    temp = temp & chunk
            End Select
        Next i

        ' Calculate the Code 128 mod 103 check digit
        For i = 1 To Len(yourData)
            e = Asc(Mid(yourData, i, 1)) - 32
            If e <> 142 Then
                checkDigitSubtotal = checkDigitSubtotal + (e * i)
            End If
        Next i
        checkDigitSubtotal = checkDigitSubtotal Mod 103

        ' Put together the final output string
        ' code set A start bar, the encoded string, check digit, stop bar
        Select Case checkDigitSubtotal
            Case 0
                Return (temp & Chr(206) & Chr(196))
            Case 1 To 93
                Return (temp & Chr(checkDigitSubtotal + 32) & Chr(196))
            Case Is > 93
                Return (temp & Chr(checkDigitSubtotal + 103) & Chr(196))
        End Select

    End Function


    Public Shared Function Azalea_GS1_128_A_embedFNC1(ByVal yourData As String) As String
        ' C128Tools 31may12 jwhiting
        ' Copyright 2012 Azalea Software, Inc. All rights reserved. www.azalea.com

        ' Create a GS1-128 code set A barcode in Excel
        ' Input error checking is your responsibility.

        Dim temp As String                 ' a temporary placeholder
        Dim chunk As String                ' loop chunk
        Dim i As Integer                   ' our loop counter
        Dim checkDigitSubtotal As Integer  ' a check digit throwaway
        Dim e As Integer                   ' a placeholder variable

        ' seed the variables
        temp = Chr(181) & Chr(205)         ' code set A start & FNC1
        checkDigitSubtotal = 205           ' code set A start (103) + FNC1 (102) checkdigit values

        ' map the input string into the C128Tools character set
        For i = 1 To Len(yourData) Step 1
            chunk = Mid(yourData, i, 1)
            Select Case Asc(chunk)
                Case Is = 205 ' embedded FNC1
                    temp = temp & Chr(205)
                    ' map from above ASCII 182 placeholders to the font's character assignments
                Case Is > 95
                    temp = temp & Chr(Asc(chunk) - 66)
                Case Else
                    temp = temp & chunk
            End Select
        Next i

        ' Calculate the Code 128 mod 103 check digit
        For i = 1 To Len(yourData)
            e = Asc(Mid(yourData, i, 1)) - 32
            If e <> 142 Then
                checkDigitSubtotal = checkDigitSubtotal + (e * (i + 1))
            End If
        Next i
        checkDigitSubtotal = checkDigitSubtotal Mod 103

        ' Put together the final output string
        ' code set A start bar, FNC1, inputString, check digit, stop bar
        Select Case checkDigitSubtotal
            Case 0
                ' Mark Presley bug fix 19jan09
                Return temp & Chr(206) & Chr(196)
            Case 1 To 93
                Return temp & Chr(checkDigitSubtotal + 32) & Chr(196)
            Case Is > 93
                Return temp & Chr(checkDigitSubtotal + 103) & Chr(196)
        End Select

    End Function




    Public Shared Function Azalea_GS1_128_A(ByVal yourData As String) As String
        ' C128Tools 31may12 jwhiting
        ' Copyright 2012 Azalea Software, Inc. All rights reserved. www.azalea.com

        ' Create a GS1-128 code set A barcode in Excel
        ' Input error checking is your responsibility.

        yourData = yourData.ToUpper


        Dim temp As String                 ' a temporary placeholder
        Dim chunk As String                ' loop chunk
        Dim i As Integer                   ' our loop counter
        Dim checkDigitSubtotal As Integer  ' a check digit throwaway
        Dim e As Integer                   ' a placeholder variable

        ' seed the variables
        temp = Chr(181) & Chr(205)         ' code set A start & FNC1
        checkDigitSubtotal = 205           ' code set A start (103) + FNC1 (102) checkdigit values

        ' map the input string into the C128Tools character set
        For i = 1 To Len(yourData) Step 1
            chunk = Mid(yourData, i, 1)
            Select Case Asc(chunk)
                ' map from above ASCII 182 placeholders to the font's character assignments
                Case Is > 95
                    temp = temp & Chr(Asc(chunk) - 66)
                Case Else
                    temp = temp & chunk
            End Select
        Next i

        ' Calculate the Code 128 mod 103 check digit
        For i = 1 To Len(yourData)
            e = Asc(Mid(yourData, i, 1)) - 32
            If e <> 142 Then
                checkDigitSubtotal = checkDigitSubtotal + (e * (i + 1))
            End If
        Next i
        checkDigitSubtotal = checkDigitSubtotal Mod 103

        ' Put together the final output string
        ' code set A start bar, FNC1, inputString, check digit, stop bar
        Select Case checkDigitSubtotal
            Case 0
                ' Mark Presley bug fix 19jan09
                Return temp & Chr(206) & Chr(196)
            Case 1 To 93
                Return temp & Chr(checkDigitSubtotal + 32) & Chr(196)
            Case Is > 93
                Return temp & Chr(checkDigitSubtotal + 103) & Chr(196)
        End Select

    End Function



    Public Shared Function Azalea_GS1_128_B(ByVal yourData As String) As String
        ' C128Tools 31may12 jwhiting
        ' Copyright 2012 Azalea Software, Inc. All rights reserved. www.azalea.com

        ' Create a GS1-128 code set B barcode in Excel
        ' Input error checking is your responsibility.

        Dim temp As String                 ' a temporary placeholder
        Dim chunk As String                ' loop chunk
        Dim i As Integer                   ' our loop counter
        Dim checkDigitSubtotal As Integer  ' a check digit throwaway
        Dim e As Integer                   ' a placeholder variable

        ' seed the variables
        temp = Chr(182) & Chr(205)         ' code set B start & FNC1
        checkDigitSubtotal = 206           ' code set B start (104) + FNC1 (102) checkdigit values

        ' map the input string into the C128Tools character set
        For i = 1 To Len(yourData) Step 1
            chunk = Mid(yourData, i, 1)
            Select Case Asc(chunk)
                ' map from above ASCII 200 to the actual character assignments
                Case Is > 200
                    temp = temp & Chr(Asc(chunk) - 35)
                Case Else
                    temp = temp & chunk
            End Select
        Next i

        ' Calculate the Code 128 mod 103 check digit
        For i = 1 To Len(yourData)
            e = Asc(Mid(yourData, i, 1)) - 32
            If e <> 142 Then
                checkDigitSubtotal = checkDigitSubtotal + (e * (i + 1))
            End If
        Next i
        checkDigitSubtotal = checkDigitSubtotal Mod 103

        ' Put together the final output string
        ' code set B start bar, FNC1, inputString, check digit, stop bar
        Select Case checkDigitSubtotal
            Case 0
                ' Mark Presley bug fix 19jan09
                Return temp & Chr(206) & Chr(196)
            Case 1 To 93
                Return temp & Chr(checkDigitSubtotal + 32) & Chr(196)
            Case Is > 93
                Return temp & Chr(checkDigitSubtotal + 103) & Chr(196)
        End Select

    End Function



    Public Shared Function Azalea_GS1_128_C(ByVal yourData As String) As String
        ' C128Tools 31may12 jwhiting
        ' Copyright 2012 Azalea Software, Inc. All rights reserved. www.azalea.com

        ' Create a GS1-128 code set C barcode in Excel
        ' Input error checking is your responsibility.

        Dim fontString As String           ' the output string
        Dim temp As String                 ' a temporary placeholder
        Dim checkDigitSubtotal As Integer  ' a check digit throwaway
        Dim i As Integer                   ' the loop counter
        Dim chunk As String                ' loop chunk

        ' seed the variables
        fontString = Chr(183) & Chr(205)   ' code set C start & FNC1
        checkDigitSubtotal = 207           ' code set C start (105) + FNC1 (102) checkdigit values

        ' checkDigitSubtotal & fontString are both OK to here

        ' do check digit calculation over yourData, assuming it's an even number of digits long
        '  Vanni, 14/01/2014 15:03:13: 
        temp = yourData

        If Len(yourData) Mod 2 <> 0 Then
            Throw New Exception("Si può usare la codifica C solo se il numero di cifre è pari.")
        End If

        For i = 1 To Len(yourData) / 2
            chunk = Left$(temp, 2)
            checkDigitSubtotal = checkDigitSubtotal + Val(chunk) * (i + 1)
            Select Case Val(chunk)
                Case 0
                    fontString = fontString + Chr(206)
                Case 1 To 93
                    fontString = fontString & Chr(Val(chunk) + 32)
                Case Is > 93
                    fontString = fontString & Chr(Val(chunk) + 103)
            End Select
            temp = Right$(temp, Len(temp) - 2)
        Next i
        checkDigitSubtotal = checkDigitSubtotal Mod 103

        ' Put together the final output string
        ' code set C start bar, the encoded string, check digit, stop bar
        Select Case checkDigitSubtotal
            Case 0
                ' Mark Presley bug fix 19jan09
                Return fontString & Chr(206) & Chr(196)
            Case 1 To 93
                Return fontString & Chr(checkDigitSubtotal + 32) & Chr(196)
            Case Is > 93
                Return fontString & Chr(checkDigitSubtotal + 103) & Chr(196)
        End Select

    End Function

End Class
