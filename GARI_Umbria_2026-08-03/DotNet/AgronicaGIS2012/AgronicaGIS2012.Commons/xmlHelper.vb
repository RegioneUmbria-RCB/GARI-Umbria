Public Class xmlHelper

#Region "Utility xml"
    Public Shared Function RemoveNamespace(xdoc As XDocument, listaDiNamespaceDaPreservare As List(Of String)) As XDocument

        If listaDiNamespaceDaPreservare Is Nothing Then
            listaDiNamespaceDaPreservare = New List(Of String)
        End If

        For Each e As XElement In xdoc.Root.Descendants()
            If e.Name.[Namespace] <> XNamespace.None AndAlso Not listaDiNamespaceDaPreservare.Contains(e.Name.NamespaceName.ToString) Then
                e.Name = XNamespace.None.GetName(e.Name.LocalName)
            End If
            If e.Attributes().Where(Function(a) a.IsNamespaceDeclaration OrElse a.Name.[Namespace] <> XNamespace.None).Any() Then
                e.ReplaceAttributes(e.Attributes().[Select](Function(a) If(a.IsNamespaceDeclaration, Nothing, If(a.Name.[Namespace] <> XNamespace.None, New XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value), a))))
            End If
        Next
        Return xdoc
    End Function
#End Region
    Public Shared Sub RemoveNamespace2(ByRef xdoc As XDocument, listaDiNamespaceDaPreservare As List(Of String))
        If listaDiNamespaceDaPreservare Is Nothing Then
            listaDiNamespaceDaPreservare = New List(Of String)
        End If

        For Each e As XElement In xdoc.Root.Descendants()
            If e.Name.[Namespace] <> XNamespace.None AndAlso Not listaDiNamespaceDaPreservare.Contains(e.Name.NamespaceName.ToString) Then
                e.Name = XNamespace.None.GetName(e.Name.LocalName)
            End If
            If e.Attributes().Where(Function(a) a.IsNamespaceDeclaration OrElse a.Name.[Namespace] <> XNamespace.None).Any() Then
                e.ReplaceAttributes(e.Attributes().[Select](Function(a) If(a.IsNamespaceDeclaration, Nothing, If(a.Name.[Namespace] <> XNamespace.None, New XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value), a))))
            End If
        Next

    End Sub

    Public Shared Sub SetDefaultNamespace(ByRef element As XElement, ByVal newXmlns As XNamespace)
        Dim currentXmlns = element.GetDefaultNamespace()
        If currentXmlns = newXmlns Then
            Return
        End If

        For Each descendant In element.DescendantsAndSelf().Where(Function(e) e.Name.[Namespace] = currentXmlns)
            '!important
            descendant.Name = newXmlns.GetName(descendant.Name.LocalName)
        Next
    End Sub

End Class
