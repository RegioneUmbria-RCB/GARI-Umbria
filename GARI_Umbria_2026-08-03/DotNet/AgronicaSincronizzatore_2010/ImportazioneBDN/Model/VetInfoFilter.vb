Namespace VetInfoModel
    Public Class VetInfoFilter
        Public field As String
        Public op As String
        Public value1 As String
        Public value2 As String

        Public Sub New()

        End Sub

        Public Sub New(ByVal field As String, ByVal op As String, ByVal value1 As String, Optional ByVal value2 As String = "")
            If Not OpVetInfoFilter.IsOperatorHandled(op) Then Throw New ArgumentException($"The operator is not handled: {op}")
            If value2 <> "" AndAlso op <> OpVetInfoFilter.BETWEEN Then Throw New ArgumentException("value2 should be empty if the operator is not BETWEEN.")

            Me.field = field
            Me.op = op
			Me.value1 = value1
            Me.value2 = value2
        End Sub

    End Class

    Public Class VetInfoPagination

        Public Sub New()
            recordsPerPage = 50
            page = 1
        End Sub

        Public recordsPerPage As Integer
        Public page As Integer
        Public sortBy As String
        Public sortType As String
    End Class

    Public Class OpVetInfoFilter
        Public Shadows Const EQUALS As String = "EQUALS"
        Public Const LESSTHAN As String = "LESSTHAN"
        Public Const MORETHAN As String = "MORETHAN"
        Public Const BETWEEN As String = "BETWEEN"

        Public Shared Function IsOperatorHandled(ByVal op As String) As Boolean
            Return {EQUALS, LESSTHAN, MORETHAN, BETWEEN}.Contains(op)
        End Function

    End Class

End Namespace
