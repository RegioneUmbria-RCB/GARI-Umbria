Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Xml
Imports System.Security.Cryptography.Xml

Namespace common.Security.Saml
    Public Class SamlSignedXml
        Inherits SignedXml

        Private _referenceAttributeId As String = ""

        Public Sub New(ByVal document As XmlDocument, ByVal referenceAttributeId As String)
            MyBase.New(document)
            _referenceAttributeId = referenceAttributeId
        End Sub

        Public Overrides Function GetIdElement(ByVal document As XmlDocument, ByVal idValue As String) As XmlElement
            Return CType(document.SelectSingleNode(String.Format("//*[@{0}='{1}']", _referenceAttributeId, idValue)), XmlElement)
        End Function
    End Class
End Namespace
