Imports System.IO
Imports System.Text

Public Class StringToFormula
    Private _operators As String() = {"-", "+", "/", "*", "^"}
    Private _operations As Func(Of Double, Double, Double)() = {Function(a1, a2) a1 - a2, Function(a1, a2) a1 + a2, Function(a1, a2) a1 / a2, Function(a1, a2) a1 * a2, Function(a1, a2) Math.Pow(a1, a2)}

    Public Function Eval(ByVal expression As String) As Double
        Dim tokens As List(Of String) = getTokens(expression)
        Dim operandStack As Stack(Of Double) = New Stack(Of Double)()
        Dim operatorStack As Stack(Of String) = New Stack(Of String)()
        Dim tokenIndex As Integer = 0

        Dim lastToken As String = ""
        While tokenIndex < tokens.Count
            Dim token As String = tokens(tokenIndex)

            If token = "(" Then
                Dim subExpr As String = getSubExpression(tokens, tokenIndex)
                operandStack.Push(Eval(subExpr))
                Continue While
            End If

            If token = ")" Then
                Throw New ArgumentException("Mis-matched parentheses in expression")
            End If

            If Array.IndexOf(_operators, token) >= 0 Then

                While operatorStack.Count > 0 AndAlso Array.IndexOf(_operators, token) < Array.IndexOf(_operators, operatorStack.Peek())
                    Dim op As String = operatorStack.Pop()
                    Dim arg2 As Double = operandStack.Pop()
                    Dim arg1 As Double = operandStack.Pop()
                    operandStack.Push(_operations(Array.IndexOf(_operators, op))(arg1, arg2))
                End While

                operatorStack.Push(token)
            Else
                operandStack.Push(Double.Parse(token))
            End If

            lastToken = token
            tokenIndex += 1
        End While

        While operatorStack.Count > 0
            Dim op As String = operatorStack.Pop()
            Dim arg2 As Double = operandStack.Pop()
            Dim arg1 As Double = operandStack.Pop()
            operandStack.Push(_operations(Array.IndexOf(_operators, op))(arg1, arg2))
        End While

        Return operandStack.Pop()
    End Function

    Private Function getSubExpression(ByVal tokens As List(Of String), ByRef index As Integer) As String
        Dim subExpr As StringBuilder = New StringBuilder()
        Dim parenlevels As Integer = 1
        index += 1

        While index < tokens.Count AndAlso parenlevels > 0
            Dim token As String = tokens(index)

            If tokens(index) = "(" Then
                parenlevels += 1
            End If

            If tokens(index) = ")" Then
                parenlevels -= 1
            End If

            If parenlevels > 0 Then
                subExpr.Append(token)
            End If

            index += 1
        End While

        If (parenlevels > 0) Then
            Throw New ArgumentException("Mis-matched parentheses in expression")
        End If

        Return subExpr.ToString()
    End Function

    Private Function getTokens(ByVal expression As String) As List(Of String)
        Dim operators As String = "()^*/+-"
        Dim tokens As List(Of String) = New List(Of String)()
        Dim sb As StringBuilder = New StringBuilder()

        For Each c As Char In expression.Replace(" ", String.Empty)

            If operators.IndexOf(c) >= 0 Then

                If (sb.Length > 0) Then
                    tokens.Add(sb.ToString())
                    sb.Length = 0
                End If

                tokens.Add(c)
            Else
                sb.Append(c)
            End If
        Next

        tokens.Add(sb.ToString())
        Return tokens
    End Function
End Class


Public Class Stringhe
    Public Shared Function RemoveTags(txt)
        'memorizza il testo in un buffer temporaneo
        Dim tmpTxt
        tmpTxt = txt

        'esci se viene passata una stringa nulla (che è diverso da stringa di lunghezza 0)
        If IsNothing(tmpTxt) Then
            Return ""
        End If

        Dim pos1, pos2
        'inizia il ciclo di ricerca...
        Do
            'cerca il prossimo inizio di tag
            pos1 = InStr(tmpTxt, "<")
            'se non lo trovi esci dal ciclo di ricerca (non ci sono più tag da eliminare)
            If pos1 = 0 Then
                Exit Do
            Else
                'se lo trovi, cerca il simbolo di chiusura del tag
                pos2 = InStr(pos1, tmpTxt, ">")
                'se non lo trovi esci dal ciclo di ricerca
                If pos2 = 0 Then
                    Exit Do
                Else
                    'elimina il tag determinato da pos1 e pos2
                    tmpTxt = Left(tmpTxt, pos1 - 1) & Mid(tmpTxt, pos2 + 1)
                End If
            End If
        Loop
        'restituisci il testo "depurato" dai tag HTML
        Return tmpTxt

    End Function

    Public Shared Function EliminaCaratteriSpecialiFile(ByVal strNomeFile As String) As String
        
        'Dim str As String = strNomeFile
        Dim i As Integer
        Dim carattereInvalido As Char

        Try

            If Not String.IsNullOrEmpty(strNomeFile) Then

                Dim systemInvalidFileChars As Char() = Path.GetInvalidFileNameChars()

                'Sarebbero validi come caratteri nel nome file, ma danno fastidio  in fase di visualizzazione sul web
                Dim extraInvalidFileChars As Char() = New Char() {",", "'", ":", "+"}

                Dim invalidFileChars = systemInvalidFileChars.Union(extraInvalidFileChars).Distinct().ToArray()

                'modificata by maga il 24/07/2015
                For i = 0 To invalidFileChars.Length - 1

                    carattereInvalido = invalidFileChars(i)

                    strNomeFile = strNomeFile.Replace(carattereInvalido, "")

                Next
                
                'str = str.Replace(" ", "")
                'str = str.Replace("'", "")
                'str = str.Replace("""", "")
                'str = str.Replace("\", "")
                'str = str.Replace("/", "")
                'str = str.Replace("*", "")
                'str = str.Replace(":", "")
                'str = str.Replace(".", "")
                'str = str.Replace("+", "")
                'str = str.Replace("|", "")
                'str = str.Replace(">", "")
                'str = str.Replace("<", "")

            End If

        Catch ex As Exception

        End Try

        Return strNomeFile

    End Function

    Public Shared Function SeparatoreDecimale() As String
        Return Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator
    End Function

    Public Shared Function TroncaStringa(ByVal testo As String, ByVal numCaratteri As Integer) As String

        If String.IsNullOrEmpty(testo) Then
            Return String.Empty
        End If

        If testo.Length > numCaratteri Then
            Return testo.Substring(0, numCaratteri)
        Else
            Return testo
        End If

    End Function

End Class
