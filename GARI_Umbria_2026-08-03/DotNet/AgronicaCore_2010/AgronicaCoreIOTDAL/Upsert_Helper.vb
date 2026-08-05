Imports System.Text
Public Class Upsert_Helper
    Private ReadOnly sql As StringBuilder
    Private comma As String
    Private counter As Integer

    Public ReadOnly Property QuerySql As String
        Get
            Return sql.ToString()
        End Get
    End Property

    Public ReadOnly Property Count As Integer
        Get
            Return counter
        End Get
    End Property

    Public Sub New()
        sql = New StringBuilder
        comma = ""
        counter = 0
    End Sub
    Public Sub Add(ByVal ParamArray args() As String)
        If args.Length <= 0 Then
            Return
        End If

        Dim str As String = ""
        Dim c As String = ""
        For i As Integer = 0 To UBound(args, 1)
            str &= c & args(i)
            c = ", "
        Next

        sql.AppendLine(comma & "(" & str & ")")
        comma = ", "
        counter += 1
    End Sub
    Public Function GetAndReset() As String

        Dim retstr As String = sql.ToString()

        sql.Clear()
        comma = ""
        counter = 0

        Return retstr
    End Function
End Class
