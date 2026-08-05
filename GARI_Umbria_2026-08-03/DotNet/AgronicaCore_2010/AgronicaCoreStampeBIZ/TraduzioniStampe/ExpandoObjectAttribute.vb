<System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple:=True, Inherited:=False)>
Public Class ExpandoObjectAttribute : Inherits Attribute

    Public ReportName As String
    Public SectionName As String
    Public IsSubreport As Boolean

    Public Sub New(ByVal reportName As String, ByVal sectionName As String, ByVal isSubreport As Boolean)
        Me.ReportName = reportName
        Me.SectionName = sectionName
        Me.IsSubreport = isSubreport
    End Sub

End Class
