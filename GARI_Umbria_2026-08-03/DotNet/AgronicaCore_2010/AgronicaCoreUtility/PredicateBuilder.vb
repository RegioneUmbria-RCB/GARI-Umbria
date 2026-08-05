Imports System.Linq.Expressions
Imports System.Runtime.CompilerServices

'Public Module PredicateBuilder
'    Public Function [True](Of T)() As Expression(Of Func(Of T, Boolean))
'        Return Function(f) True
'    End Function

'    Public Function [False](Of T)() As Expression(Of Func(Of T, Boolean))
'        Return Function(f) False
'    End Function

'    <Extension()>
'Public Function [Or](Of T)(ByVal expr1 As Expression(Of Func(Of T, Boolean)), ByVal expr2 As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
'    Dim invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast(Of Expression)())
'    Return Expression.Lambda(Of Func(Of T, Boolean))(Expression.[Or](expr1.Body, invokedExpr), expr1.Parameters)
'End Function

'    <Extension()>
'    Public Function [And](Of T)(ByVal expr1 As Expression(Of Func(Of T, Boolean)), ByVal expr2 As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
'        Dim invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast(Of Expression)())
'        Return Expression.Lambda(Of Func(Of T, Boolean))(Expression.[And](expr1.Body, invokedExpr), expr1.Parameters)
'    End Function
'End Module

Public Module PredicateBuilder
    Public Function [True](Of T)() As Expression(Of Func(Of T, Boolean))
        Return Function(param) True
    End Function

    Public Function [False](Of T)() As Expression(Of Func(Of T, Boolean))
        Return Function(param) False
    End Function

    Public Function Create(Of T)(ByVal predicate As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
        Return predicate
    End Function

    <Extension()>
    Public Function [And](Of T)(ByVal first As Expression(Of Func(Of T, Boolean)), ByVal second As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
        Return first.Compose(second, AddressOf Expression.[AndAlso])
    End Function

    <System.Runtime.CompilerServices.Extension()> _
    Public Function [Or](Of T)(ByVal first As Expression(Of Func(Of T, Boolean)), ByVal second As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
        Return first.Compose(second, AddressOf Expression.[OrElse])
    End Function

    <Extension()>
    Public Function [Not](Of T)(ByVal expression As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
        Dim negated As UnaryExpression = Expression.[Not](expression.Body)
        Return Expression.Lambda(Of Func(Of T, Boolean))(negated, expression.Parameters)
    End Function

    <Extension()>
    Private Function Compose(Of T)(ByVal first As Expression(Of T), ByVal second As Expression(Of T), ByVal merge As Func(Of Expression, Expression, Expression)) As Expression(Of T)
        Dim map = first.Parameters.[Select](Function(f, i) New With {f, Key .s = second.Parameters(i)}).ToDictionary(Function(p) p.s, Function(p) p.f)
        Dim secondBody As Expression = ParameterRebinder.ReplaceParameters(map, second.Body)
        Return Expression.Lambda(Of T)(merge(first.Body, secondBody), first.Parameters)
    End Function

    Class ParameterRebinder
        Inherits ExpressionVisitor

        ReadOnly map As Dictionary(Of ParameterExpression, ParameterExpression)

        Private Sub New(ByVal map As Dictionary(Of ParameterExpression, ParameterExpression))
            Me.map = If(map, New Dictionary(Of ParameterExpression, ParameterExpression)())
        End Sub

        Public Shared Function ReplaceParameters(ByVal map As Dictionary(Of ParameterExpression, ParameterExpression), ByVal exp As Expression) As Expression
            Return New ParameterRebinder(map).Visit(exp)
        End Function

        Protected Overrides Function VisitParameter(ByVal p As ParameterExpression) As Expression
            Dim replacement As ParameterExpression

            If map.TryGetValue(p, replacement) Then
                p = replacement
            End If

            Return MyBase.VisitParameter(p)
        End Function
    End Class
End Module
