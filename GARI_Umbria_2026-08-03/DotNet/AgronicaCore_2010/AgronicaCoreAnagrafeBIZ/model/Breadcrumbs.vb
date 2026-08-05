Public Class BreadcrumbParams
    Public Piva As String
    Public Sa_Cod As String
    Public Campo_Cod As String
End Class

Public Enum BreadcrumbLevel
    Centro = 2
    Campi = 3
End Enum

Public Class BreadCrumbItem
    Public text As String
    Public title As String
    Public disabled As Boolean
    Public icon As String
    Public iconClass As String
    Public imageUrl As String

End Class

Public Class Breadcrumb
    Public item As BreadCrumbItem
    Public level As BreadcrumbLevel
    Public treeElemPendingSelection As String
    Public gridRowId As String
End Class

