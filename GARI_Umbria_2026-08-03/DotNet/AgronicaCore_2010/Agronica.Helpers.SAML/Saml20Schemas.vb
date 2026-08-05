Namespace Italia.Spid.Authentication.IdP
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("Signature", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class SignatureType
        Private signedInfoField As SignedInfoType
        Private signatureValueField As SignatureValueType
        Private keyInfoField As KeyInfoType
        Private objectField As ObjectType()
        Private idField As String

        Public Property SignedInfo As SignedInfoType
            Get
                Return Me.signedInfoField
            End Get
            Set(ByVal value As SignedInfoType)
                Me.signedInfoField = value
            End Set
        End Property

        Public Property SignatureValue As SignatureValueType
            Get
                Return Me.signatureValueField
            End Get
            Set(ByVal value As SignatureValueType)
                Me.signatureValueField = value
            End Set
        End Property

        Public Property KeyInfo As KeyInfoType
            Get
                Return Me.keyInfoField
            End Get
            Set(ByVal value As KeyInfoType)
                Me.keyInfoField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("Object")>
        Public Property Objects As ObjectType()
            Get
                Return Me.objectField
            End Get
            Set(ByVal value As ObjectType())
                Me.objectField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property Id As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("SignedInfo", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class SignedInfoType
        Private canonicalizationMethodField As CanonicalizationMethodType
        Private signatureMethodField As SignatureMethodType
        Private referenceField As ReferenceType()
        Private idField As String

        Public Property CanonicalizationMethod As CanonicalizationMethodType
            Get
                Return Me.canonicalizationMethodField
            End Get
            Set(ByVal value As CanonicalizationMethodType)
                Me.canonicalizationMethodField = value
            End Set
        End Property

        Public Property SignatureMethod As SignatureMethodType
            Get
                Return Me.signatureMethodField
            End Get
            Set(ByVal value As SignatureMethodType)
                Me.signatureMethodField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("Reference")>
        Public Property Reference As ReferenceType()
            Get
                Return Me.referenceField
            End Get
            Set(ByVal value As ReferenceType())
                Me.referenceField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property Id As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("CanonicalizationMethod", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class CanonicalizationMethodType
        Private anyField As System.Xml.XmlNode()
        Private algorithmField As String

        <System.Xml.Serialization.XmlTextAttribute()>
        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Any As System.Xml.XmlNode()
            Get
                Return Me.anyField
            End Get
            Set(ByVal value As System.Xml.XmlNode())
                Me.anyField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Algorithm As String
            Get
                Return Me.algorithmField
            End Get
            Set(ByVal value As String)
                Me.algorithmField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    Partial Public Class X509IssuerSerialType
        Private x509IssuerNameField As String
        Private x509SerialNumberField As String

        Public Property X509IssuerName As String
            Get
                Return Me.x509IssuerNameField
            End Get
            Set(ByVal value As String)
                Me.x509IssuerNameField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute(DataType:="integer")>
        Public Property X509SerialNumber As String
            Get
                Return Me.x509SerialNumberField
            End Get
            Set(ByVal value As String)
                Me.x509SerialNumberField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("SignatureMethod", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class SignatureMethodType
        Private hMACOutputLengthField As String
        Private anyField As System.Xml.XmlNode()
        Private algorithmField As String

        <System.Xml.Serialization.XmlElementAttribute(DataType:="integer")>
        Public Property HMACOutputLength As String
            Get
                Return Me.hMACOutputLengthField
            End Get
            Set(ByVal value As String)
                Me.hMACOutputLengthField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlTextAttribute()>
        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Any As System.Xml.XmlNode()
            Get
                Return Me.anyField
            End Get
            Set(ByVal value As System.Xml.XmlNode())
                Me.anyField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Algorithm As String
            Get
                Return Me.algorithmField
            End Get
            Set(ByVal value As String)
                Me.algorithmField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("Reference", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class ReferenceType
        Private transformsField As TransformType()
        Private digestMethodField As DigestMethodType
        Private digestValueField As Byte()
        Private idField As String
        Private uRIField As String
        Private typeField As String

        <System.Xml.Serialization.XmlArrayItemAttribute("Transform", IsNullable:=False)>
        Public Property Transforms As TransformType()
            Get
                Return Me.transformsField
            End Get
            Set(ByVal value As TransformType())
                Me.transformsField = value
            End Set
        End Property

        Public Property DigestMethod As DigestMethodType
            Get
                Return Me.digestMethodField
            End Get
            Set(ByVal value As DigestMethodType)
                Me.digestMethodField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>
        Public Property DigestValue As Byte()
            Get
                Return Me.digestValueField
            End Get
            Set(ByVal value As Byte())
                Me.digestValueField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property Id As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property URI As String
            Get
                Return Me.uRIField
            End Get
            Set(ByVal value As String)
                Me.uRIField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Type As String
            Get
                Return Me.typeField
            End Get
            Set(ByVal value As String)
                Me.typeField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("Transform", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class TransformType
        Private itemsField As Object()
        Private textField As String()
        Private algorithmField As String

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        <System.Xml.Serialization.XmlElementAttribute("XPath", GetType(String))>
        Public Property Items As Object()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As Object())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlTextAttribute()>
        Public Property Text As String()
            Get
                Return Me.textField
            End Get
            Set(ByVal value As String())
                Me.textField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Algorithm As String
            Get
                Return Me.algorithmField
            End Get
            Set(ByVal value As String)
                Me.algorithmField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("DigestMethod", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class DigestMethodType
        Private anyField As System.Xml.XmlNode()
        Private algorithmField As String

        <System.Xml.Serialization.XmlTextAttribute()>
        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Any As System.Xml.XmlNode()
            Get
                Return Me.anyField
            End Get
            Set(ByVal value As System.Xml.XmlNode())
                Me.anyField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Algorithm As String
            Get
                Return Me.algorithmField
            End Get
            Set(ByVal value As String)
                Me.algorithmField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("SignatureValue", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class SignatureValueType
        Private idField As String
        Private valueField As Byte()

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property Id As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlTextAttribute(DataType:="base64Binary")>
        Public Property Value As Byte()
            Get
                Return Me.valueField
            End Get
            Set(ByVal value As Byte())
                Me.valueField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("KeyInfo", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class KeyInfoType
        Private itemsField As Object()
        Private itemsElementNameField As ItemsChoiceType2()
        Private textField As String()
        Private idField As String

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        <System.Xml.Serialization.XmlElementAttribute("KeyName", GetType(String))>
        <System.Xml.Serialization.XmlElementAttribute("KeyValue", GetType(KeyValueType))>
        <System.Xml.Serialization.XmlElementAttribute("MgmtData", GetType(String))>
        <System.Xml.Serialization.XmlElementAttribute("PGPData", GetType(PGPDataType))>
        <System.Xml.Serialization.XmlElementAttribute("RetrievalMethod", GetType(RetrievalMethodType))>
        <System.Xml.Serialization.XmlElementAttribute("SPKIData", GetType(SPKIDataType))>
        <System.Xml.Serialization.XmlElementAttribute("X509Data", GetType(X509DataType))>
        <System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")>
        Public Property Items As Object()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As Object())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("ItemsElementName")>
        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property ItemsElementName As ItemsChoiceType2()
            Get
                Return Me.itemsElementNameField
            End Get
            Set(ByVal value As ItemsChoiceType2())
                Me.itemsElementNameField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlTextAttribute()>
        Public Property Text As String()
            Get
                Return Me.textField
            End Get
            Set(ByVal value As String())
                Me.textField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property Id As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("KeyValue", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class KeyValueType
        Private itemField As Object
        Private textField As String()

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        <System.Xml.Serialization.XmlElementAttribute("DSAKeyValue", GetType(DSAKeyValueType))>
        <System.Xml.Serialization.XmlElementAttribute("RSAKeyValue", GetType(RSAKeyValueType))>
        Public Property Item As Object
            Get
                Return Me.itemField
            End Get
            Set(ByVal value As Object)
                Me.itemField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlTextAttribute()>
        Public Property Text As String()
            Get
                Return Me.textField
            End Get
            Set(ByVal value As String())
                Me.textField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("DSAKeyValue", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class DSAKeyValueType
        Private pField As Byte()
        Private qField As Byte()
        Private gField As Byte()
        Private yField As Byte()
        Private jField As Byte()
        Private seedField As Byte()
        Private pgenCounterField As Byte()

        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>
        Public Property P As Byte()
            Get
                Return Me.pField
            End Get
            Set(ByVal value As Byte())
                Me.pField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>
        Public Property Q As Byte()
            Get
                Return Me.qField
            End Get
            Set(ByVal value As Byte())
                Me.qField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>
        Public Property G As Byte()
            Get
                Return Me.gField
            End Get
            Set(ByVal value As Byte())
                Me.gField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>
        Public Property Y As Byte()
            Get
                Return Me.yField
            End Get
            Set(ByVal value As Byte())
                Me.yField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>
        Public Property J As Byte()
            Get
                Return Me.jField
            End Get
            Set(ByVal value As Byte())
                Me.jField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>
        Public Property Seed As Byte()
            Get
                Return Me.seedField
            End Get
            Set(ByVal value As Byte())
                Me.seedField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>
        Public Property PgenCounter As Byte()
            Get
                Return Me.pgenCounterField
            End Get
            Set(ByVal value As Byte())
                Me.pgenCounterField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("RSAKeyValue", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class RSAKeyValueType
        Private modulusField As Byte()
        Private exponentField As Byte()

        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>
        Public Property Modulus As Byte()
            Get
                Return Me.modulusField
            End Get
            Set(ByVal value As Byte())
                Me.modulusField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>
        Public Property Exponent As Byte()
            Get
                Return Me.exponentField
            End Get
            Set(ByVal value As Byte())
                Me.exponentField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("PGPData", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class PGPDataType
        Private itemsField As Object()
        Private itemsElementNameField As ItemsChoiceType1()

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        <System.Xml.Serialization.XmlElementAttribute("PGPKeyID", GetType(Byte()), DataType:="base64Binary")>
        <System.Xml.Serialization.XmlElementAttribute("PGPKeyPacket", GetType(Byte()), DataType:="base64Binary")>
        <System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")>
        Public Property Items As Object()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As Object())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("ItemsElementName")>
        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property ItemsElementName As ItemsChoiceType1()
            Get
                Return Me.itemsElementNameField
            End Get
            Set(ByVal value As ItemsChoiceType1())
                Me.itemsElementNameField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#", IncludeInSchema:=False)>
    Public Enum ItemsChoiceType1
        <System.Xml.Serialization.XmlEnumAttribute("##any:")>
        Item
        PGPKeyID
        PGPKeyPacket
    End Enum

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("RetrievalMethod", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class RetrievalMethodType
        Private transformsField As TransformType()
        Private uRIField As String
        Private typeField As String

        <System.Xml.Serialization.XmlArrayItemAttribute("Transform", IsNullable:=False)>
        Public Property Transforms As TransformType()
            Get
                Return Me.transformsField
            End Get
            Set(ByVal value As TransformType())
                Me.transformsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property URI As String
            Get
                Return Me.uRIField
            End Get
            Set(ByVal value As String)
                Me.uRIField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Type As String
            Get
                Return Me.typeField
            End Get
            Set(ByVal value As String)
                Me.typeField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("SPKIData", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class SPKIDataType
        Private sPKISexpField As Byte()()
        Private anyField As System.Xml.XmlElement

        <System.Xml.Serialization.XmlElementAttribute("SPKISexp", DataType:="base64Binary")>
        Public Property SPKISexp As Byte()()
            Get
                Return Me.sPKISexpField
            End Get
            Set(ByVal value As Byte()())
                Me.sPKISexpField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Any As System.Xml.XmlElement
            Get
                Return Me.anyField
            End Get
            Set(ByVal value As System.Xml.XmlElement)
                Me.anyField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("X509Data", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class X509DataType
        Private itemsField As Object()
        Private itemsElementNameField As ItemsChoiceType()

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        <System.Xml.Serialization.XmlElementAttribute("X509CRL", GetType(Byte()), DataType:="base64Binary")>
        <System.Xml.Serialization.XmlElementAttribute("X509Certificate", GetType(Byte()), DataType:="base64Binary")>
        <System.Xml.Serialization.XmlElementAttribute("X509IssuerSerial", GetType(X509IssuerSerialType))>
        <System.Xml.Serialization.XmlElementAttribute("X509SKI", GetType(Byte()), DataType:="base64Binary")>
        <System.Xml.Serialization.XmlElementAttribute("X509SubjectName", GetType(String))>
        <System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")>
        Public Property Items As Object()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As Object())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("ItemsElementName")>
        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property ItemsElementName As ItemsChoiceType()
            Get
                Return Me.itemsElementNameField
            End Get
            Set(ByVal value As ItemsChoiceType())
                Me.itemsElementNameField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#", IncludeInSchema:=False)>
    Public Enum ItemsChoiceType
        <System.Xml.Serialization.XmlEnumAttribute("##any:")>
        Item
        X509CRL
        X509Certificate
        X509IssuerSerial
        X509SKI
        X509SubjectName
    End Enum

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#", IncludeInSchema:=False)>
    Public Enum ItemsChoiceType2
        <System.Xml.Serialization.XmlEnumAttribute("##any:")>
        Item
        KeyName
        KeyValue
        MgmtData
        PGPData
        RetrievalMethod
        SPKIData
        X509Data
    End Enum

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("Object", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class ObjectType
        Private anyField As System.Xml.XmlNode()
        Private idField As String
        Private mimeTypeField As String
        Private encodingField As String

        <System.Xml.Serialization.XmlTextAttribute()>
        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Any As System.Xml.XmlNode()
            Get
                Return Me.anyField
            End Get
            Set(ByVal value As System.Xml.XmlNode())
                Me.anyField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property Id As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property MimeType As String
            Get
                Return Me.mimeTypeField
            End Get
            Set(ByVal value As String)
                Me.mimeTypeField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Encoding As String
            Get
                Return Me.encodingField
            End Get
            Set(ByVal value As String)
                Me.encodingField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("Transforms", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class TransformsType
        Private transformField As TransformType()

        <System.Xml.Serialization.XmlElementAttribute("Transform")>
        Public Property Transform As TransformType()
            Get
                Return Me.transformField
            End Get
            Set(ByVal value As TransformType())
                Me.transformField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("Manifest", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class ManifestType
        Private referenceField As ReferenceType()
        Private idField As String

        <System.Xml.Serialization.XmlElementAttribute("Reference")>
        Public Property Reference As ReferenceType()
            Get
                Return Me.referenceField
            End Get
            Set(ByVal value As ReferenceType())
                Me.referenceField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property Id As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("SignatureProperties", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class SignaturePropertiesType
        Private signaturePropertyField As SignaturePropertyType()
        Private idField As String

        <System.Xml.Serialization.XmlElementAttribute("SignatureProperty")>
        Public Property SignatureProperty As SignaturePropertyType()
            Get
                Return Me.signaturePropertyField
            End Get
            Set(ByVal value As SignaturePropertyType())
                Me.signaturePropertyField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property Id As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
    <System.Xml.Serialization.XmlRootAttribute("SignatureProperty", [Namespace]:="http://www.w3.org/2000/09/xmldsig#", IsNullable:=False)>
    Partial Public Class SignaturePropertyType
        Private itemsField As System.Xml.XmlElement()
        Private textField As String()
        Private targetField As String
        Private idField As String

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Items As System.Xml.XmlElement()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As System.Xml.XmlElement())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlTextAttribute()>
        Public Property Text As String()
            Get
                Return Me.textField
            End Get
            Set(ByVal value As String())
                Me.textField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Target As String
            Get
                Return Me.targetField
            End Get
            Set(ByVal value As String)
                Me.targetField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property Id As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
    <System.Xml.Serialization.XmlRootAttribute("CipherData", [Namespace]:="http://www.w3.org/2001/04/xmlenc#", IsNullable:=False)>
    Partial Public Class CipherDataType
        Private itemField As Object

        <System.Xml.Serialization.XmlElementAttribute("CipherReference", GetType(CipherReferenceType))>
        <System.Xml.Serialization.XmlElementAttribute("CipherValue", GetType(Byte()), DataType:="base64Binary")>
        Public Property Item As Object
            Get
                Return Me.itemField
            End Get
            Set(ByVal value As Object)
                Me.itemField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
    <System.Xml.Serialization.XmlRootAttribute("CipherReference", [Namespace]:="http://www.w3.org/2001/04/xmlenc#", IsNullable:=False)>
    Partial Public Class CipherReferenceType
        Private itemField As TransformsType1
        Private uRIField As String

        <System.Xml.Serialization.XmlElementAttribute("Transforms")>
        Public Property Item As TransformsType1
            Get
                Return Me.itemField
            End Get
            Set(ByVal value As TransformsType1)
                Me.itemField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property URI As String
            Get
                Return Me.uRIField
            End Get
            Set(ByVal value As String)
                Me.uRIField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute(TypeName:="TransformsType", [Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
    Partial Public Class TransformsType1
        Private transformField As TransformType()

        <System.Xml.Serialization.XmlElementAttribute("Transform", [Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
        Public Property Transform As TransformType()
            Get
                Return Me.transformField
            End Get
            Set(ByVal value As TransformType())
                Me.transformField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
    <System.Xml.Serialization.XmlRootAttribute("EncryptedData", [Namespace]:="http://www.w3.org/2001/04/xmlenc#", IsNullable:=False)>
    Partial Public Class EncryptedDataType
        Inherits EncryptedType
    End Class

    <System.Xml.Serialization.XmlIncludeAttribute(GetType(EncryptedKeyType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(EncryptedDataType))>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
    Partial Public MustInherit Class EncryptedType
        Private encryptionMethodField As EncryptionMethodType
        Private keyInfoField As KeyInfoType
        Private cipherDataField As CipherDataType
        Private encryptionPropertiesField As EncryptionPropertiesType
        Private idField As String
        Private typeField As String
        Private mimeTypeField As String
        Private encodingField As String

        Public Property EncryptionMethod As EncryptionMethodType
            Get
                Return Me.encryptionMethodField
            End Get
            Set(ByVal value As EncryptionMethodType)
                Me.encryptionMethodField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
        Public Property KeyInfo As KeyInfoType
            Get
                Return Me.keyInfoField
            End Get
            Set(ByVal value As KeyInfoType)
                Me.keyInfoField = value
            End Set
        End Property

        Public Property CipherData As CipherDataType
            Get
                Return Me.cipherDataField
            End Get
            Set(ByVal value As CipherDataType)
                Me.cipherDataField = value
            End Set
        End Property

        Public Property EncryptionProperties As EncryptionPropertiesType
            Get
                Return Me.encryptionPropertiesField
            End Get
            Set(ByVal value As EncryptionPropertiesType)
                Me.encryptionPropertiesField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property Id As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Type As String
            Get
                Return Me.typeField
            End Get
            Set(ByVal value As String)
                Me.typeField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property MimeType As String
            Get
                Return Me.mimeTypeField
            End Get
            Set(ByVal value As String)
                Me.mimeTypeField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Encoding As String
            Get
                Return Me.encodingField
            End Get
            Set(ByVal value As String)
                Me.encodingField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
    Partial Public Class EncryptionMethodType
        Private keySizeField As String
        Private oAEPparamsField As Byte()
        Private anyField As System.Xml.XmlNode()
        Private algorithmField As String

        <System.Xml.Serialization.XmlElementAttribute(DataType:="integer")>
        Public Property KeySize As String
            Get
                Return Me.keySizeField
            End Get
            Set(ByVal value As String)
                Me.keySizeField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>
        Public Property OAEPparams As Byte()
            Get
                Return Me.oAEPparamsField
            End Get
            Set(ByVal value As Byte())
                Me.oAEPparamsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlTextAttribute()>
        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Any As System.Xml.XmlNode()
            Get
                Return Me.anyField
            End Get
            Set(ByVal value As System.Xml.XmlNode())
                Me.anyField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Algorithm As String
            Get
                Return Me.algorithmField
            End Get
            Set(ByVal value As String)
                Me.algorithmField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
    <System.Xml.Serialization.XmlRootAttribute("EncryptionProperties", [Namespace]:="http://www.w3.org/2001/04/xmlenc#", IsNullable:=False)>
    Partial Public Class EncryptionPropertiesType
        Private encryptionPropertyField As EncryptionPropertyType()
        Private idField As String

        <System.Xml.Serialization.XmlElementAttribute("EncryptionProperty")>
        Public Property EncryptionProperty As EncryptionPropertyType()
            Get
                Return Me.encryptionPropertyField
            End Get
            Set(ByVal value As EncryptionPropertyType())
                Me.encryptionPropertyField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property Id As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
    <System.Xml.Serialization.XmlRootAttribute("EncryptionProperty", [Namespace]:="http://www.w3.org/2001/04/xmlenc#", IsNullable:=False)>
    Partial Public Class EncryptionPropertyType
        Private itemsField As System.Xml.XmlElement()
        Private textField As String()
        Private targetField As String
        Private idField As String
        Private anyAttrField As System.Xml.XmlAttribute()

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Items As System.Xml.XmlElement()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As System.Xml.XmlElement())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlTextAttribute()>
        Public Property Text As String()
            Get
                Return Me.textField
            End Get
            Set(ByVal value As String())
                Me.textField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Target As String
            Get
                Return Me.targetField
            End Get
            Set(ByVal value As String)
                Me.targetField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property Id As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAnyAttributeAttribute()>
        Public Property AnyAttr As System.Xml.XmlAttribute()
            Get
                Return Me.anyAttrField
            End Get
            Set(ByVal value As System.Xml.XmlAttribute())
                Me.anyAttrField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
    <System.Xml.Serialization.XmlRootAttribute("EncryptedKey", [Namespace]:="http://www.w3.org/2001/04/xmlenc#", IsNullable:=False)>
    Partial Public Class EncryptedKeyType
        Inherits EncryptedType

        Private referenceListField As ReferenceList
        Private carriedKeyNameField As String
        Private recipientField As String

        Public Property ReferenceList As ReferenceList
            Get
                Return Me.referenceListField
            End Get
            Set(ByVal value As ReferenceList)
                Me.referenceListField = value
            End Set
        End Property

        Public Property CarriedKeyName As String
            Get
                Return Me.carriedKeyNameField
            End Get
            Set(ByVal value As String)
                Me.carriedKeyNameField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property Recipient As String
            Get
                Return Me.recipientField
            End Get
            Set(ByVal value As String)
                Me.recipientField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True, [Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
    <System.Xml.Serialization.XmlRootAttribute([Namespace]:="http://www.w3.org/2001/04/xmlenc#", IsNullable:=False)>
    Partial Public Class ReferenceList
        Private itemsField As ReferenceType1()
        Private itemsElementNameField As ItemsChoiceType3()

        <System.Xml.Serialization.XmlElementAttribute("DataReference", GetType(ReferenceType1))>
        <System.Xml.Serialization.XmlElementAttribute("KeyReference", GetType(ReferenceType1))>
        <System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")>
        Public Property Items As ReferenceType1()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As ReferenceType1())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("ItemsElementName")>
        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property ItemsElementName As ItemsChoiceType3()
            Get
                Return Me.itemsElementNameField
            End Get
            Set(ByVal value As ItemsChoiceType3())
                Me.itemsElementNameField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute(TypeName:="ReferenceType", [Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
    Partial Public Class ReferenceType1
        Private anyField As System.Xml.XmlElement()
        Private uRIField As String

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Any As System.Xml.XmlElement()
            Get
                Return Me.anyField
            End Get
            Set(ByVal value As System.Xml.XmlElement())
                Me.anyField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property URI As String
            Get
                Return Me.uRIField
            End Get
            Set(ByVal value As String)
                Me.uRIField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2001/04/xmlenc#", IncludeInSchema:=False)>
    Public Enum ItemsChoiceType3
        DataReference
        KeyReference
    End Enum

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
    <System.Xml.Serialization.XmlRootAttribute("AgreementMethod", [Namespace]:="http://www.w3.org/2001/04/xmlenc#", IsNullable:=False)>
    Partial Public Class AgreementMethodType
        Private kANonceField As Byte()
        Private anyField As System.Xml.XmlNode()
        Private originatorKeyInfoField As KeyInfoType
        Private recipientKeyInfoField As KeyInfoType
        Private algorithmField As String

        <System.Xml.Serialization.XmlElementAttribute("KA-Nonce", DataType:="base64Binary")>
        Public Property KANonce As Byte()
            Get
                Return Me.kANonceField
            End Get
            Set(ByVal value As Byte())
                Me.kANonceField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlTextAttribute()>
        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Any As System.Xml.XmlNode()
            Get
                Return Me.anyField
            End Get
            Set(ByVal value As System.Xml.XmlNode())
                Me.anyField = value
            End Set
        End Property

        Public Property OriginatorKeyInfo As KeyInfoType
            Get
                Return Me.originatorKeyInfoField
            End Get
            Set(ByVal value As KeyInfoType)
                Me.originatorKeyInfoField = value
            End Set
        End Property

        Public Property RecipientKeyInfo As KeyInfoType
            Get
                Return Me.recipientKeyInfoField
            End Get
            Set(ByVal value As KeyInfoType)
                Me.recipientKeyInfoField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Algorithm As String
            Get
                Return Me.algorithmField
            End Get
            Set(ByVal value As String)
                Me.algorithmField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("BaseID", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public MustInherit Class BaseIDAbstractType
        Private nameQualifierField As String
        Private sPNameQualifierField As String

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property NameQualifier As String
            Get
                Return Me.nameQualifierField
            End Get
            Set(ByVal value As String)
                Me.nameQualifierField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property SPNameQualifier As String
            Get
                Return Me.sPNameQualifierField
            End Get
            Set(ByVal value As String)
                Me.sPNameQualifierField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("NameID", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class NameIDType
        Private nameQualifierField As String
        Private sPNameQualifierField As String
        Private formatField As String
        Private sPProvidedIDField As String
        Private valueField As String

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property NameQualifier As String
            Get
                Return Me.nameQualifierField
            End Get
            Set(ByVal value As String)
                Me.nameQualifierField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property SPNameQualifier As String
            Get
                Return Me.sPNameQualifierField
            End Get
            Set(ByVal value As String)
                Me.sPNameQualifierField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Format As String
            Get
                Return Me.formatField
            End Get
            Set(ByVal value As String)
                Me.formatField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property SPProvidedID As String
            Get
                Return Me.sPProvidedIDField
            End Get
            Set(ByVal value As String)
                Me.sPProvidedIDField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlTextAttribute()>
        Public Property Value As String
            Get
                Return Me.valueField
            End Get
            Set(ByVal value As String)
                Me.valueField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("EncryptedID", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class EncryptedElementType
        Private encryptedDataField As EncryptedDataType
        Private encryptedKeyField As EncryptedKeyType()

        <System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
        Public Property EncryptedData As EncryptedDataType
            Get
                Return Me.encryptedDataField
            End Get
            Set(ByVal value As EncryptedDataType)
                Me.encryptedDataField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("EncryptedKey", [Namespace]:="http://www.w3.org/2001/04/xmlenc#")>
        Public Property EncryptedKey As EncryptedKeyType()
            Get
                Return Me.encryptedKeyField
            End Get
            Set(ByVal value As EncryptedKeyType())
                Me.encryptedKeyField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("Assertion", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class AssertionType
        Private issuerField As NameIDType
        Private signatureField As SignatureType
        Private subjectField As SubjectType
        Private conditionsField As ConditionsType
        Private adviceField As AdviceType
        Private itemsField As StatementAbstractType()
        Private versionField As String
        Private idField As String
        Private issueInstantField As System.DateTime

        Public Property Issuer As NameIDType
            Get
                Return Me.issuerField
            End Get
            Set(ByVal value As NameIDType)
                Me.issuerField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
        Public Property Signature As SignatureType
            Get
                Return Me.signatureField
            End Get
            Set(ByVal value As SignatureType)
                Me.signatureField = value
            End Set
        End Property

        Public Property Subject As SubjectType
            Get
                Return Me.subjectField
            End Get
            Set(ByVal value As SubjectType)
                Me.subjectField = value
            End Set
        End Property

        Public Property Conditions As ConditionsType
            Get
                Return Me.conditionsField
            End Get
            Set(ByVal value As ConditionsType)
                Me.conditionsField = value
            End Set
        End Property

        Public Property Advice As AdviceType
            Get
                Return Me.adviceField
            End Get
            Set(ByVal value As AdviceType)
                Me.adviceField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("AttributeStatement", GetType(AttributeStatementType))>
        <System.Xml.Serialization.XmlElementAttribute("AuthnStatement", GetType(AuthnStatementType))>
        <System.Xml.Serialization.XmlElementAttribute("AuthzDecisionStatement", GetType(AuthzDecisionStatementType))>
        <System.Xml.Serialization.XmlElementAttribute("Statement", GetType(StatementAbstractType))>
        Public Property Items As StatementAbstractType()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As StatementAbstractType())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property Version As String
            Get
                Return Me.versionField
            End Get
            Set(ByVal value As String)
                Me.versionField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property ID As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property IssueInstant As System.DateTime
            Get
                Return Me.issueInstantField
            End Get
            Set(ByVal value As System.DateTime)
                Me.issueInstantField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("Subject", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class SubjectType
        Private itemsField As Object()

        <System.Xml.Serialization.XmlElementAttribute("BaseID", GetType(BaseIDAbstractType))>
        <System.Xml.Serialization.XmlElementAttribute("EncryptedID", GetType(EncryptedElementType))>
        <System.Xml.Serialization.XmlElementAttribute("NameID", GetType(NameIDType))>
        <System.Xml.Serialization.XmlElementAttribute("SubjectConfirmation", GetType(SubjectConfirmationType))>
        Public Property Items As Object()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As Object())
                Me.itemsField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("SubjectConfirmation", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class SubjectConfirmationType
        Private itemField As Object
        Private subjectConfirmationDataField As SubjectConfirmationDataType
        Private methodField As String

        <System.Xml.Serialization.XmlElementAttribute("BaseID", GetType(BaseIDAbstractType))>
        <System.Xml.Serialization.XmlElementAttribute("EncryptedID", GetType(EncryptedElementType))>
        <System.Xml.Serialization.XmlElementAttribute("NameID", GetType(NameIDType))>
        Public Property Item As Object
            Get
                Return Me.itemField
            End Get
            Set(ByVal value As Object)
                Me.itemField = value
            End Set
        End Property

        Public Property SubjectConfirmationData As SubjectConfirmationDataType
            Get
                Return Me.subjectConfirmationDataField
            End Get
            Set(ByVal value As SubjectConfirmationDataType)
                Me.subjectConfirmationDataField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Method As String
            Get
                Return Me.methodField
            End Get
            Set(ByVal value As String)
                Me.methodField = value
            End Set
        End Property
    End Class

    <System.Xml.Serialization.XmlIncludeAttribute(GetType(KeyInfoConfirmationDataType))>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("SubjectConfirmationData", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class SubjectConfirmationDataType
        Private textField As String()

        <System.Xml.Serialization.XmlTextAttribute()>
        Public Property Text As String()
            Get
                Return Me.textField
            End Get
            Set(ByVal value As String())
                Me.textField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    Partial Public Class KeyInfoConfirmationDataType
        Inherits SubjectConfirmationDataType
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("Conditions", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class ConditionsType
        Private itemsField As ConditionAbstractType()
        Private notBeforeField As String
        Private notBeforeFieldSpecified As Boolean
        Private notOnOrAfterField As String
        Private notOnOrAfterFieldSpecified As Boolean

        <System.Xml.Serialization.XmlElementAttribute("AudienceRestriction", GetType(AudienceRestrictionType))>
        <System.Xml.Serialization.XmlElementAttribute("Condition", GetType(ConditionAbstractType))>
        <System.Xml.Serialization.XmlElementAttribute("OneTimeUse", GetType(OneTimeUseType))>
        <System.Xml.Serialization.XmlElementAttribute("ProxyRestriction", GetType(ProxyRestrictionType))>
        Public Property Items As ConditionAbstractType()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As ConditionAbstractType())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property NotBefore As String
            Get
                Return Me.notBeforeField
            End Get
            Set(ByVal value As String)
                Me.notBeforeField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property NotBeforeSpecified As Boolean
            Get
                Return Me.notBeforeFieldSpecified
            End Get
            Set(ByVal value As Boolean)
                Me.notBeforeFieldSpecified = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property NotOnOrAfter As String
            Get
                Return Me.notOnOrAfterField
            End Get
            Set(ByVal value As String)
                Me.notOnOrAfterField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property NotOnOrAfterSpecified As Boolean
            Get
                Return Me.notOnOrAfterFieldSpecified
            End Get
            Set(ByVal value As Boolean)
                Me.notOnOrAfterFieldSpecified = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("AudienceRestriction", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class AudienceRestrictionType
        Inherits ConditionAbstractType

        Private audienceField As String()

        <System.Xml.Serialization.XmlElementAttribute("Audience", DataType:="anyURI")>
        Public Property Audience As String()
            Get
                Return Me.audienceField
            End Get
            Set(ByVal value As String())
                Me.audienceField = value
            End Set
        End Property
    End Class

    <System.Xml.Serialization.XmlIncludeAttribute(GetType(ProxyRestrictionType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(OneTimeUseType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(AudienceRestrictionType))>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("Condition", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public MustInherit Class ConditionAbstractType
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("OneTimeUse", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class OneTimeUseType
        Inherits ConditionAbstractType
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("ProxyRestriction", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class ProxyRestrictionType
        Inherits ConditionAbstractType

        Private audienceField As String()
        Private countField As String

        <System.Xml.Serialization.XmlElementAttribute("Audience", DataType:="anyURI")>
        Public Property Audience As String()
            Get
                Return Me.audienceField
            End Get
            Set(ByVal value As String())
                Me.audienceField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="nonNegativeInteger")>
        Public Property Count As String
            Get
                Return Me.countField
            End Get
            Set(ByVal value As String)
                Me.countField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("Advice", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class AdviceType
        Private itemsField As Object()
        Private itemsElementNameField As ItemsChoiceType4()

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        <System.Xml.Serialization.XmlElementAttribute("Assertion", GetType(AssertionType))>
        <System.Xml.Serialization.XmlElementAttribute("AssertionIDRef", GetType(String), DataType:="NCName")>
        <System.Xml.Serialization.XmlElementAttribute("AssertionURIRef", GetType(String), DataType:="anyURI")>
        <System.Xml.Serialization.XmlElementAttribute("EncryptedAssertion", GetType(EncryptedElementType))>
        <System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")>
        Public Property Items As Object()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As Object())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("ItemsElementName")>
        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property ItemsElementName As ItemsChoiceType4()
            Get
                Return Me.itemsElementNameField
            End Get
            Set(ByVal value As ItemsChoiceType4())
                Me.itemsElementNameField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IncludeInSchema:=False)>
    Public Enum ItemsChoiceType4
        <System.Xml.Serialization.XmlEnumAttribute("##any:")>
        Item
        Assertion
        AssertionIDRef
        AssertionURIRef
        EncryptedAssertion
    End Enum

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("AttributeStatement", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class AttributeStatementType
        Inherits StatementAbstractType

        Private itemsField As Object()

        <System.Xml.Serialization.XmlElementAttribute("Attribute", GetType(AttributeType))>
        <System.Xml.Serialization.XmlElementAttribute("EncryptedAttribute", GetType(EncryptedElementType))>
        Public Property Items As Object()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As Object())
                Me.itemsField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("Attribute", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class AttributeType
        Private attributeValueField As Object()
        Private nameField As String
        Private nameFormatField As String
        Private friendlyNameField As String
        Private anyAttrField As System.Xml.XmlAttribute()

        <System.Xml.Serialization.XmlElementAttribute("AttributeValue", IsNullable:=True)>
        Public Property AttributeValue As Object()
            Get
                Return Me.attributeValueField
            End Get
            Set(ByVal value As Object())
                Me.attributeValueField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property Name As String
            Get
                Return Me.nameField
            End Get
            Set(ByVal value As String)
                Me.nameField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property NameFormat As String
            Get
                Return Me.nameFormatField
            End Get
            Set(ByVal value As String)
                Me.nameFormatField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property FriendlyName As String
            Get
                Return Me.friendlyNameField
            End Get
            Set(ByVal value As String)
                Me.friendlyNameField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAnyAttributeAttribute()>
        Public Property AnyAttr As System.Xml.XmlAttribute()
            Get
                Return Me.anyAttrField
            End Get
            Set(ByVal value As System.Xml.XmlAttribute())
                Me.anyAttrField = value
            End Set
        End Property
    End Class

    <System.Xml.Serialization.XmlIncludeAttribute(GetType(AttributeStatementType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(AuthzDecisionStatementType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(AuthnStatementType))>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("Statement", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public MustInherit Class StatementAbstractType
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("AuthnStatement", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class AuthnStatementType
        Inherits StatementAbstractType

        Private subjectLocalityField As SubjectLocalityType
        Private authnContextField As AuthnContextType
        Private authnInstantField As System.DateTime
        Private sessionIndexField As String
        Private sessionNotOnOrAfterField As System.DateTime
        Private sessionNotOnOrAfterFieldSpecified As Boolean

        Public Property SubjectLocality As SubjectLocalityType
            Get
                Return Me.subjectLocalityField
            End Get
            Set(ByVal value As SubjectLocalityType)
                Me.subjectLocalityField = value
            End Set
        End Property

        Public Property AuthnContext As AuthnContextType
            Get
                Return Me.authnContextField
            End Get
            Set(ByVal value As AuthnContextType)
                Me.authnContextField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property AuthnInstant As System.DateTime
            Get
                Return Me.authnInstantField
            End Get
            Set(ByVal value As System.DateTime)
                Me.authnInstantField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property SessionIndex As String
            Get
                Return Me.sessionIndexField
            End Get
            Set(ByVal value As String)
                Me.sessionIndexField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property SessionNotOnOrAfter As System.DateTime
            Get
                Return Me.sessionNotOnOrAfterField
            End Get
            Set(ByVal value As System.DateTime)
                Me.sessionNotOnOrAfterField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property SessionNotOnOrAfterSpecified As Boolean
            Get
                Return Me.sessionNotOnOrAfterFieldSpecified
            End Get
            Set(ByVal value As Boolean)
                Me.sessionNotOnOrAfterFieldSpecified = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("SubjectLocality", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class SubjectLocalityType
        Private addressField As String
        Private dNSNameField As String

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property Address As String
            Get
                Return Me.addressField
            End Get
            Set(ByVal value As String)
                Me.addressField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property DNSName As String
            Get
                Return Me.dNSNameField
            End Get
            Set(ByVal value As String)
                Me.dNSNameField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("AuthnContext", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class AuthnContextType
        Private itemsField As Object()
        Private itemsElementNameField As ItemsChoiceType5()
        Private authenticatingAuthorityField As String()

        <System.Xml.Serialization.XmlElementAttribute("AuthnContextClassRef", GetType(String), DataType:="anyURI")>
        <System.Xml.Serialization.XmlElementAttribute("AuthnContextDecl", GetType(Object))>
        <System.Xml.Serialization.XmlElementAttribute("AuthnContextDeclRef", GetType(String), DataType:="anyURI")>
        <System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")>
        Public Property Items As Object()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As Object())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("ItemsElementName")>
        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property ItemsElementName As ItemsChoiceType5()
            Get
                Return Me.itemsElementNameField
            End Get
            Set(ByVal value As ItemsChoiceType5())
                Me.itemsElementNameField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("AuthenticatingAuthority", DataType:="anyURI")>
        Public Property AuthenticatingAuthority As String()
            Get
                Return Me.authenticatingAuthorityField
            End Get
            Set(ByVal value As String())
                Me.authenticatingAuthorityField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IncludeInSchema:=False)>
    Public Enum ItemsChoiceType5
        AuthnContextClassRef
        AuthnContextDecl
        AuthnContextDeclRef
    End Enum

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("AuthzDecisionStatement", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class AuthzDecisionStatementType
        Inherits StatementAbstractType

        Private actionField As ActionType()
        Private evidenceField As EvidenceType
        Private resourceField As String
        Private decisionField As DecisionType

        <System.Xml.Serialization.XmlElementAttribute("Action")>
        Public Property Action As ActionType()
            Get
                Return Me.actionField
            End Get
            Set(ByVal value As ActionType())
                Me.actionField = value
            End Set
        End Property

        Public Property Evidence As EvidenceType
            Get
                Return Me.evidenceField
            End Get
            Set(ByVal value As EvidenceType)
                Me.evidenceField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Resource As String
            Get
                Return Me.resourceField
            End Get
            Set(ByVal value As String)
                Me.resourceField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property Decision As DecisionType
            Get
                Return Me.decisionField
            End Get
            Set(ByVal value As DecisionType)
                Me.decisionField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("Action", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class ActionType
        Private namespaceField As String
        Private valueField As String

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property [Namespace] As String
            Get
                Return Me.namespaceField
            End Get
            Set(ByVal value As String)
                Me.namespaceField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlTextAttribute()>
        Public Property Value As String
            Get
                Return Me.valueField
            End Get
            Set(ByVal value As String)
                Me.valueField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    <System.Xml.Serialization.XmlRootAttribute("Evidence", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable:=False)>
    Partial Public Class EvidenceType
        Private itemsField As Object()
        Private itemsElementNameField As ItemsChoiceType6()

        <System.Xml.Serialization.XmlElementAttribute("Assertion", GetType(AssertionType))>
        <System.Xml.Serialization.XmlElementAttribute("AssertionIDRef", GetType(String), DataType:="NCName")>
        <System.Xml.Serialization.XmlElementAttribute("AssertionURIRef", GetType(String), DataType:="anyURI")>
        <System.Xml.Serialization.XmlElementAttribute("EncryptedAssertion", GetType(EncryptedElementType))>
        <System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")>
        Public Property Items As Object()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As Object())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("ItemsElementName")>
        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property ItemsElementName As ItemsChoiceType6()
            Get
                Return Me.itemsElementNameField
            End Get
            Set(ByVal value As ItemsChoiceType6())
                Me.itemsElementNameField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", IncludeInSchema:=False)>
    Public Enum ItemsChoiceType6
        Assertion
        AssertionIDRef
        AssertionURIRef
        EncryptedAssertion
    End Enum

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
    Public Enum DecisionType
        Permit
        Deny
        Indeterminate
    End Enum

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("Extensions", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class ExtensionsType
        Private anyField As System.Xml.XmlElement()

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Any As System.Xml.XmlElement()
            Get
                Return Me.anyField
            End Get
            Set(ByVal value As System.Xml.XmlElement())
                Me.anyField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("Status", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class StatusType
        Private statusCodeField As StatusCodeType
        Private statusMessageField As String
        Private statusDetailField As StatusDetailType

        Public Property StatusCode As StatusCodeType
            Get
                Return Me.statusCodeField
            End Get
            Set(ByVal value As StatusCodeType)
                Me.statusCodeField = value
            End Set
        End Property

        Public Property StatusMessage As String
            Get
                Return Me.statusMessageField
            End Get
            Set(ByVal value As String)
                Me.statusMessageField = value
            End Set
        End Property

        Public Property StatusDetail As StatusDetailType
            Get
                Return Me.statusDetailField
            End Get
            Set(ByVal value As StatusDetailType)
                Me.statusDetailField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("StatusCode", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class StatusCodeType
        Private statusCodeField As StatusCodeType
        Private valueField As String

        Public Property StatusCode As StatusCodeType
            Get
                Return Me.statusCodeField
            End Get
            Set(ByVal value As StatusCodeType)
                Me.statusCodeField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Value As String
            Get
                Return Me.valueField
            End Get
            Set(ByVal value As String)
                Me.valueField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("StatusDetail", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class StatusDetailType
        Private anyField As System.Xml.XmlElement()

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Any As System.Xml.XmlElement()
            Get
                Return Me.anyField
            End Get
            Set(ByVal value As System.Xml.XmlElement())
                Me.anyField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("AssertionIDRequest", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class AssertionIDRequestType
        Inherits RequestAbstractType

        Private assertionIDRefField As String()

        <System.Xml.Serialization.XmlElementAttribute("AssertionIDRef", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", DataType:="NCName")>
        Public Property AssertionIDRef As String()
            Get
                Return Me.assertionIDRefField
            End Get
            Set(ByVal value As String())
                Me.assertionIDRefField = value
            End Set
        End Property
    End Class

    <System.Xml.Serialization.XmlIncludeAttribute(GetType(NameIDMappingRequestType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(LogoutRequestType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(ManageNameIDRequestType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(ArtifactResolveType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(AuthnRequestType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(SubjectQueryAbstractType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(AuthzDecisionQueryType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(AttributeQueryType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(AuthnQueryType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(AssertionIDRequestType))>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    Partial Public MustInherit Class RequestAbstractType
        Private issuerField As NameIDType
        Private signatureField As SignatureType
        Private extensionsField As ExtensionsType
        Private idField As String
        Private versionField As String
        Private issueInstantField As String
        Private destinationField As String
        Private consentField As String

        <System.Xml.Serialization.XmlElementAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Issuer As NameIDType
            Get
                Return Me.issuerField
            End Get
            Set(ByVal value As NameIDType)
                Me.issuerField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
        Public Property Signature As SignatureType
            Get
                Return Me.signatureField
            End Get
            Set(ByVal value As SignatureType)
                Me.signatureField = value
            End Set
        End Property

        Public Property Extensions As ExtensionsType
            Get
                Return Me.extensionsField
            End Get
            Set(ByVal value As ExtensionsType)
                Me.extensionsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property ID As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property Version As String
            Get
                Return Me.versionField
            End Get
            Set(ByVal value As String)
                Me.versionField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property IssueInstant As String
            Get
                Return Me.issueInstantField
            End Get
            Set(ByVal value As String)
                Me.issueInstantField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Destination As String
            Get
                Return Me.destinationField
            End Get
            Set(ByVal value As String)
                Me.destinationField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Consent As String
            Get
                Return Me.consentField
            End Get
            Set(ByVal value As String)
                Me.consentField = value
            End Set
        End Property
    End Class

    <System.Xml.Serialization.XmlIncludeAttribute(GetType(AuthzDecisionQueryType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(AttributeQueryType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(AuthnQueryType))>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("SubjectQuery", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public MustInherit Class SubjectQueryAbstractType
        Inherits RequestAbstractType

        Private subjectField As SubjectType

        <System.Xml.Serialization.XmlElementAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Subject As SubjectType
            Get
                Return Me.subjectField
            End Get
            Set(ByVal value As SubjectType)
                Me.subjectField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("AuthnQuery", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class AuthnQueryType
        Inherits SubjectQueryAbstractType

        Private requestedAuthnContextField As RequestedAuthnContextType
        Private sessionIndexField As String

        Public Property RequestedAuthnContext As RequestedAuthnContextType
            Get
                Return Me.requestedAuthnContextField
            End Get
            Set(ByVal value As RequestedAuthnContextType)
                Me.requestedAuthnContextField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property SessionIndex As String
            Get
                Return Me.sessionIndexField
            End Get
            Set(ByVal value As String)
                Me.sessionIndexField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("RequestedAuthnContext", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class RequestedAuthnContextType
        Private itemsField As String()
        Private itemsElementNameField As ItemsChoiceType7()
        Private comparisonField As AuthnContextComparisonType
        Private comparisonFieldSpecified As Boolean

        <System.Xml.Serialization.XmlElementAttribute("AuthnContextClassRef", GetType(String), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", DataType:="anyURI")>
        <System.Xml.Serialization.XmlElementAttribute("AuthnContextDeclRef", GetType(String), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion", DataType:="anyURI")>
        <System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")>
        Public Property Items As String()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As String())
                Me.itemsField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("ItemsElementName")>
        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property ItemsElementName As ItemsChoiceType7()
            Get
                Return Me.itemsElementNameField
            End Get
            Set(ByVal value As ItemsChoiceType7())
                Me.itemsElementNameField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property Comparison As AuthnContextComparisonType
            Get
                Return Me.comparisonField
            End Get
            Set(ByVal value As AuthnContextComparisonType)
                Me.comparisonField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property ComparisonSpecified As Boolean
            Get
                Return Me.comparisonFieldSpecified
            End Get
            Set(ByVal value As Boolean)
                Me.comparisonFieldSpecified = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IncludeInSchema:=False)>
    Public Enum ItemsChoiceType7
        <System.Xml.Serialization.XmlEnumAttribute("urn:oasis:names:tc:SAML:2.0:assertion:AuthnContextClassRef")>
        AuthnContextClassRef
        <System.Xml.Serialization.XmlEnumAttribute("urn:oasis:names:tc:SAML:2.0:assertion:AuthnContextDeclRef")>
        AuthnContextDeclRef
    End Enum

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    Public Enum AuthnContextComparisonType
        exact
        minimum
        maximum
        better
    End Enum

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("AttributeQuery", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class AttributeQueryType
        Inherits SubjectQueryAbstractType

        Private attributeField As AttributeType()

        <System.Xml.Serialization.XmlElementAttribute("Attribute", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Attribute As AttributeType()
            Get
                Return Me.attributeField
            End Get
            Set(ByVal value As AttributeType())
                Me.attributeField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("AuthzDecisionQuery", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class AuthzDecisionQueryType
        Inherits SubjectQueryAbstractType

        Private actionField As ActionType()
        Private evidenceField As EvidenceType
        Private resourceField As String

        <System.Xml.Serialization.XmlElementAttribute("Action", [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Action As ActionType()
            Get
                Return Me.actionField
            End Get
            Set(ByVal value As ActionType())
                Me.actionField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Evidence As EvidenceType
            Get
                Return Me.evidenceField
            End Get
            Set(ByVal value As EvidenceType)
                Me.evidenceField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Resource As String
            Get
                Return Me.resourceField
            End Get
            Set(ByVal value As String)
                Me.resourceField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("AuthnRequest", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class AuthnRequestType
        Inherits RequestAbstractType

        Private subjectField As SubjectType
        Private nameIDPolicyField As NameIDPolicyType
        Private conditionsField As ConditionsType
        Private requestedAuthnContextField As RequestedAuthnContextType
        Private scopingField As ScopingType
        Private forceAuthnField As Boolean
        Private forceAuthnFieldSpecified As Boolean
        Private isPassiveField As Boolean
        Private isPassiveFieldSpecified As Boolean
        Private protocolBindingField As String
        Private assertionConsumerServiceIndexField As UShort
        Private assertionConsumerServiceIndexFieldSpecified As Boolean
        Private assertionConsumerServiceURLField As String
        Private attributeConsumingServiceIndexField As UShort
        Private attributeConsumingServiceIndexFieldSpecified As Boolean
        Private providerNameField As String

        <System.Xml.Serialization.XmlElementAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Subject As SubjectType
            Get
                Return Me.subjectField
            End Get
            Set(ByVal value As SubjectType)
                Me.subjectField = value
            End Set
        End Property

        Public Property NameIDPolicy As NameIDPolicyType
            Get
                Return Me.nameIDPolicyField
            End Get
            Set(ByVal value As NameIDPolicyType)
                Me.nameIDPolicyField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Conditions As ConditionsType
            Get
                Return Me.conditionsField
            End Get
            Set(ByVal value As ConditionsType)
                Me.conditionsField = value
            End Set
        End Property

        Public Property RequestedAuthnContext As RequestedAuthnContextType
            Get
                Return Me.requestedAuthnContextField
            End Get
            Set(ByVal value As RequestedAuthnContextType)
                Me.requestedAuthnContextField = value
            End Set
        End Property

        Public Property Scoping As ScopingType
            Get
                Return Me.scopingField
            End Get
            Set(ByVal value As ScopingType)
                Me.scopingField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property ForceAuthn As Boolean
            Get
                Return Me.forceAuthnField
            End Get
            Set(ByVal value As Boolean)
                Me.forceAuthnField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property ForceAuthnSpecified As Boolean
            Get
                Return Me.forceAuthnFieldSpecified
            End Get
            Set(ByVal value As Boolean)
                Me.forceAuthnFieldSpecified = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property IsPassive As Boolean
            Get
                Return Me.isPassiveField
            End Get
            Set(ByVal value As Boolean)
                Me.isPassiveField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property IsPassiveSpecified As Boolean
            Get
                Return Me.isPassiveFieldSpecified
            End Get
            Set(ByVal value As Boolean)
                Me.isPassiveFieldSpecified = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property ProtocolBinding As String
            Get
                Return Me.protocolBindingField
            End Get
            Set(ByVal value As String)
                Me.protocolBindingField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property AssertionConsumerServiceIndex As UShort
            Get
                Return Me.assertionConsumerServiceIndexField
            End Get
            Set(ByVal value As UShort)
                Me.assertionConsumerServiceIndexField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property AssertionConsumerServiceIndexSpecified As Boolean
            Get
                Return Me.assertionConsumerServiceIndexFieldSpecified
            End Get
            Set(ByVal value As Boolean)
                Me.assertionConsumerServiceIndexFieldSpecified = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property AssertionConsumerServiceURL As String
            Get
                Return Me.assertionConsumerServiceURLField
            End Get
            Set(ByVal value As String)
                Me.assertionConsumerServiceURLField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property AttributeConsumingServiceIndex As UShort
            Get
                Return Me.attributeConsumingServiceIndexField
            End Get
            Set(ByVal value As UShort)
                Me.attributeConsumingServiceIndexField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property AttributeConsumingServiceIndexSpecified As Boolean
            Get
                Return Me.attributeConsumingServiceIndexFieldSpecified
            End Get
            Set(ByVal value As Boolean)
                Me.attributeConsumingServiceIndexFieldSpecified = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property ProviderName As String
            Get
                Return Me.providerNameField
            End Get
            Set(ByVal value As String)
                Me.providerNameField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("NameIDPolicy", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class NameIDPolicyType
        Private formatField As String
        Private sPNameQualifierField As String
        Private allowCreateField As Boolean
        Private allowCreateFieldSpecified As Boolean

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Format As String
            Get
                Return Me.formatField
            End Get
            Set(ByVal value As String)
                Me.formatField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property SPNameQualifier As String
            Get
                Return Me.sPNameQualifierField
            End Get
            Set(ByVal value As String)
                Me.sPNameQualifierField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property AllowCreate As Boolean
            Get
                Return Me.allowCreateField
            End Get
            Set(ByVal value As Boolean)
                Me.allowCreateField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property AllowCreateSpecified As Boolean
            Get
                Return Me.allowCreateFieldSpecified
            End Get
            Set(ByVal value As Boolean)
                Me.allowCreateFieldSpecified = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("Scoping", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class ScopingType
        Private iDPListField As IDPListType
        Private requesterIDField As String()
        Private proxyCountField As String

        Public Property IDPList As IDPListType
            Get
                Return Me.iDPListField
            End Get
            Set(ByVal value As IDPListType)
                Me.iDPListField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("RequesterID", DataType:="anyURI")>
        Public Property RequesterID As String()
            Get
                Return Me.requesterIDField
            End Get
            Set(ByVal value As String())
                Me.requesterIDField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="nonNegativeInteger")>
        Public Property ProxyCount As String
            Get
                Return Me.proxyCountField
            End Get
            Set(ByVal value As String)
                Me.proxyCountField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("IDPList", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class IDPListType
        Private iDPEntryField As IDPEntryType()
        Private getCompleteField As String

        <System.Xml.Serialization.XmlElementAttribute("IDPEntry")>
        Public Property IDPEntry As IDPEntryType()
            Get
                Return Me.iDPEntryField
            End Get
            Set(ByVal value As IDPEntryType())
                Me.iDPEntryField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute(DataType:="anyURI")>
        Public Property GetComplete As String
            Get
                Return Me.getCompleteField
            End Get
            Set(ByVal value As String)
                Me.getCompleteField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("IDPEntry", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class IDPEntryType
        Private providerIDField As String
        Private nameField As String
        Private locField As String

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property ProviderID As String
            Get
                Return Me.providerIDField
            End Get
            Set(ByVal value As String)
                Me.providerIDField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property Name As String
            Get
                Return Me.nameField
            End Get
            Set(ByVal value As String)
                Me.nameField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Loc As String
            Get
                Return Me.locField
            End Get
            Set(ByVal value As String)
                Me.locField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("Response", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class ResponseType
        Inherits StatusResponseType

        Private itemsField As Object()

        <System.Xml.Serialization.XmlElementAttribute("Assertion", GetType(AssertionType), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        <System.Xml.Serialization.XmlElementAttribute("EncryptedAssertion", GetType(EncryptedElementType), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Items As Object()
            Get
                Return Me.itemsField
            End Get
            Set(ByVal value As Object())
                Me.itemsField = value
            End Set
        End Property
    End Class

    <System.Xml.Serialization.XmlIncludeAttribute(GetType(NameIDMappingResponseType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(ArtifactResponseType))>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(ResponseType))>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("ManageNameIDResponse", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class StatusResponseType
        Private issuerField As NameIDType
        Private signatureField As SignatureType
        Private extensionsField As ExtensionsType
        Private statusField As StatusType
        Private idField As String
        Private inResponseToField As String
        Private versionField As String
        Private issueInstantField As System.DateTime
        Private destinationField As String
        Private consentField As String

        <System.Xml.Serialization.XmlElementAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Issuer As NameIDType
            Get
                Return Me.issuerField
            End Get
            Set(ByVal value As NameIDType)
                Me.issuerField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.w3.org/2000/09/xmldsig#")>
        Public Property Signature As SignatureType
            Get
                Return Me.signatureField
            End Get
            Set(ByVal value As SignatureType)
                Me.signatureField = value
            End Set
        End Property

        Public Property Extensions As ExtensionsType
            Get
                Return Me.extensionsField
            End Get
            Set(ByVal value As ExtensionsType)
                Me.extensionsField = value
            End Set
        End Property

        Public Property Status As StatusType
            Get
                Return Me.statusField
            End Get
            Set(ByVal value As StatusType)
                Me.statusField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="ID")>
        Public Property ID As String
            Get
                Return Me.idField
            End Get
            Set(ByVal value As String)
                Me.idField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="NCName")>
        Public Property InResponseTo As String
            Get
                Return Me.inResponseToField
            End Get
            Set(ByVal value As String)
                Me.inResponseToField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property Version As String
            Get
                Return Me.versionField
            End Get
            Set(ByVal value As String)
                Me.versionField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property IssueInstant As System.DateTime
            Get
                Return Me.issueInstantField
            End Get
            Set(ByVal value As System.DateTime)
                Me.issueInstantField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Destination As String
            Get
                Return Me.destinationField
            End Get
            Set(ByVal value As String)
                Me.destinationField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="anyURI")>
        Public Property Consent As String
            Get
                Return Me.consentField
            End Get
            Set(ByVal value As String)
                Me.consentField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("ArtifactResolve", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class ArtifactResolveType
        Inherits RequestAbstractType

        Private artifactField As String

        Public Property Artifact As String
            Get
                Return Me.artifactField
            End Get
            Set(ByVal value As String)
                Me.artifactField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("ArtifactResponse", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class ArtifactResponseType
        Inherits StatusResponseType

        Private anyField As System.Xml.XmlElement

        <System.Xml.Serialization.XmlAnyElementAttribute()>
        Public Property Any As System.Xml.XmlElement
            Get
                Return Me.anyField
            End Get
            Set(ByVal value As System.Xml.XmlElement)
                Me.anyField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("ManageNameIDRequest", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class ManageNameIDRequestType
        Inherits RequestAbstractType

        Private itemField As Object
        Private item1Field As Object

        <System.Xml.Serialization.XmlElementAttribute("EncryptedID", GetType(EncryptedElementType), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        <System.Xml.Serialization.XmlElementAttribute("NameID", GetType(NameIDType), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Item As Object
            Get
                Return Me.itemField
            End Get
            Set(ByVal value As Object)
                Me.itemField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("NewEncryptedID", GetType(EncryptedElementType))>
        <System.Xml.Serialization.XmlElementAttribute("NewID", GetType(String))>
        <System.Xml.Serialization.XmlElementAttribute("Terminate", GetType(TerminateType))>
        Public Property Item1 As Object
            Get
                Return Me.item1Field
            End Get
            Set(ByVal value As Object)
                Me.item1Field = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("Terminate", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class TerminateType
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("LogoutRequest", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class LogoutRequestType
        Inherits RequestAbstractType

        Private itemField As Object
        Private sessionIndexField As String()
        Private reasonField As String
        Private notOnOrAfterField As System.DateTime
        Private notOnOrAfterFieldSpecified As Boolean

        <System.Xml.Serialization.XmlElementAttribute("BaseID", GetType(BaseIDAbstractType), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        <System.Xml.Serialization.XmlElementAttribute("EncryptedID", GetType(EncryptedElementType), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        <System.Xml.Serialization.XmlElementAttribute("NameID", GetType(NameIDType), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Item As Object
            Get
                Return Me.itemField
            End Get
            Set(ByVal value As Object)
                Me.itemField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlElementAttribute("SessionIndex")>
        Public Property SessionIndex As String()
            Get
                Return Me.sessionIndexField
            End Get
            Set(ByVal value As String())
                Me.sessionIndexField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property Reason As String
            Get
                Return Me.reasonField
            End Get
            Set(ByVal value As String)
                Me.reasonField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlAttributeAttribute()>
        Public Property NotOnOrAfter As System.DateTime
            Get
                Return Me.notOnOrAfterField
            End Get
            Set(ByVal value As System.DateTime)
                Me.notOnOrAfterField = value
            End Set
        End Property

        <System.Xml.Serialization.XmlIgnoreAttribute()>
        Public Property NotOnOrAfterSpecified As Boolean
            Get
                Return Me.notOnOrAfterFieldSpecified
            End Get
            Set(ByVal value As Boolean)
                Me.notOnOrAfterFieldSpecified = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("NameIDMappingRequest", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class NameIDMappingRequestType
        Inherits RequestAbstractType

        Private itemField As Object
        Private nameIDPolicyField As NameIDPolicyType

        <System.Xml.Serialization.XmlElementAttribute("BaseID", GetType(BaseIDAbstractType), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        <System.Xml.Serialization.XmlElementAttribute("EncryptedID", GetType(EncryptedElementType), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        <System.Xml.Serialization.XmlElementAttribute("NameID", GetType(NameIDType), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Item As Object
            Get
                Return Me.itemField
            End Get
            Set(ByVal value As Object)
                Me.itemField = value
            End Set
        End Property

        Public Property NameIDPolicy As NameIDPolicyType
            Get
                Return Me.nameIDPolicyField
            End Get
            Set(ByVal value As NameIDPolicyType)
                Me.nameIDPolicyField = value
            End Set
        End Property
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")>
    <System.SerializableAttribute()>
    <System.Diagnostics.DebuggerStepThroughAttribute()>
    <System.ComponentModel.DesignerCategoryAttribute("code")>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol")>
    <System.Xml.Serialization.XmlRootAttribute("NameIDMappingResponse", [Namespace]:="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable:=False)>
    Partial Public Class NameIDMappingResponseType
        Inherits StatusResponseType

        Private itemField As Object

        <System.Xml.Serialization.XmlElementAttribute("EncryptedID", GetType(EncryptedElementType), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        <System.Xml.Serialization.XmlElementAttribute("NameID", GetType(NameIDType), [Namespace]:="urn:oasis:names:tc:SAML:2.0:assertion")>
        Public Property Item As Object
            Get
                Return Me.itemField
            End Get
            Set(ByVal value As Object)
                Me.itemField = value
            End Set
        End Property
    End Class
End Namespace
