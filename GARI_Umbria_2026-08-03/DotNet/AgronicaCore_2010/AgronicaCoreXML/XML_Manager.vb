Public Class XML_Manager


    Public Shared Function Agro_If(ByVal Elemento As Object, ByVal ElementoDefault As Object) As Object

        If CStr(Elemento) <> "#" Then
            Return Elemento
        Else
            Return ElementoDefault
        End If

    End Function


End Class
