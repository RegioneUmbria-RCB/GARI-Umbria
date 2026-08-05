'Namespace Utility_NS



Public Class Messages_Str
    Private _Messaggi As List(Of String)

    Sub New()
        _Messaggi = New List(Of String)
    End Sub

    Public Sub add(ByVal Messaggio As String)
        Messaggi.Add(Messaggio)
    End Sub

    Public Sub deleteAll()
        _Messaggi = New List(Of String)
    End Sub

    Public Function getLast()
        Return _Messaggi(_Messaggi.Count - 1)
    End Function

    Public Function getFirst()
        Return _Messaggi(0)
    End Function

    Public Function getAllHtml()
        Dim res As String = ""
        For i = 0 To _Messaggi.Count - 1
            res = res & _Messaggi(i) & " ; </br> "
        Next
        Return res
    End Function

    Public ReadOnly Property Messaggi() As List(Of String)
        Get
            Return _Messaggi
        End Get
    End Property

End Class




'End Namespace

