Public Class DALBase : Inherits AgronicaCoreDataProvider.DataProvider
    Public Function RaiseDAlException(ByVal ex As Exception, ByVal nomeProcedura As String) As Exception
        Dim messaggio = ex.Message.ToString & If(Not IsNothing(ex.InnerException), " (" & ex.InnerException.Message.ToString & ")", "")
        Return New Exception(String.Format("{0} : {1}{2}", nomeProcedura, messaggio, "</br>"))
    End Function

End Class
