
Imports System.ComponentModel
Imports System.Dynamic
Imports CrystalDecisions.CrystalReports.Engine

Friend Class ExpandoObjectPropertyDescriptor
    Inherits PropertyDescriptor

    Private ReadOnly m_Instance As IDictionary(Of String, Object)
    Private ReadOnly m_Name As String

    Public Sub New(ByVal instance As IDictionary(Of String, Object), ByVal name As String)
        MyBase.New(name, Nothing)
        m_Instance = instance
        m_Name = name
    End Sub

    Public Overrides ReadOnly Property ComponentType As Type
        Get
            Throw New NotImplementedException
        End Get
    End Property

    Public Overrides ReadOnly Property IsReadOnly As Boolean
        Get
            Throw New NotImplementedException
        End Get
    End Property

    Public Overrides ReadOnly Property PropertyType As Type
        Get
            Return GetExpandoObjectPropertyType()
        End Get
    End Property

    Private Function GetExpandoObjectPropertyType() As Type
        Return m_Instance(m_Name).[GetType]()
    End Function

    Public Overrides Function CanResetValue(ByVal component As Object) As Boolean
        Throw New NotImplementedException()
    End Function

    Public Overrides Function GetValue(ByVal component As Object) As Object
        Return m_Instance(m_Name)
    End Function

    Public Overrides Sub ResetValue(ByVal component As Object)
        Throw New NotImplementedException()
    End Sub

    Public Overrides Sub SetValue(ByVal component As Object, ByVal value As Object)
        m_Instance(m_Name) = value
    End Sub

    Public Overrides Function ShouldSerializeValue(ByVal component As Object) As Boolean
        Throw New NotImplementedException()
    End Function

    Protected Overrides Sub FillAttributes(attributeList As IList)
        Throw New NotImplementedException()
    End Sub

End Class

Public Class ExpandoObjectTypeDescriptor
    Implements ICustomTypeDescriptor

    Private ReadOnly m_Instance As IDictionary(Of String, Object)

    Public Sub New(ByVal instance As Object)
        m_Instance = TryCast(instance, IDictionary(Of String, Object))
    End Sub

    Public Function GetAttributes() As AttributeCollection Implements ICustomTypeDescriptor.GetAttributes
        Return TypeDescriptor.GetAttributes(m_Instance)
    End Function

    Public Function GetClassName() As String Implements ICustomTypeDescriptor.GetClassName
        Throw New NotImplementedException()
    End Function

    Public Function GetComponentName() As String Implements ICustomTypeDescriptor.GetComponentName
        Throw New NotImplementedException()
    End Function

    Public Function GetConverter() As TypeConverter Implements ICustomTypeDescriptor.GetConverter
        Throw New NotImplementedException()
    End Function

    Public Function GetDefaultEvent() As EventDescriptor Implements ICustomTypeDescriptor.GetDefaultEvent
        Throw New NotImplementedException()
    End Function

    Public Function GetDefaultProperty() As PropertyDescriptor Implements ICustomTypeDescriptor.GetDefaultProperty
        Throw New NotImplementedException()
    End Function

    Public Function GetEditor(editorBaseType As Type) As Object Implements ICustomTypeDescriptor.GetEditor
        Throw New NotImplementedException()
    End Function

    Public Function GetEvents() As EventDescriptorCollection Implements ICustomTypeDescriptor.GetEvents
        Throw New NotImplementedException()
    End Function

    Public Function GetEvents(attributes() As Attribute) As EventDescriptorCollection Implements ICustomTypeDescriptor.GetEvents
        Throw New NotImplementedException()
    End Function

    Public Function GetProperties() As PropertyDescriptorCollection Implements ICustomTypeDescriptor.GetProperties
        Return New PropertyDescriptorCollection(m_Instance.Keys.[Select](Function(x) New ExpandoObjectPropertyDescriptor(m_Instance, x)).ToArray())
    End Function

    Public Function GetProperties(attributes() As Attribute) As PropertyDescriptorCollection Implements ICustomTypeDescriptor.GetProperties
        Throw New NotImplementedException()
    End Function

    Public Function GetPropertyOwner(pd As PropertyDescriptor) As Object Implements ICustomTypeDescriptor.GetPropertyOwner
        Throw New NotImplementedException()
    End Function
End Class

Public Class ExpandoObjectTypeDescriptionProvider
    Inherits TypeDescriptionProvider

    Private Shared ReadOnly m_Default As TypeDescriptionProvider = TypeDescriptor.GetProvider(GetType(ExpandoObject))

    Public Sub New()
        MyBase.New(m_Default)
    End Sub

    Public Overrides Function GetTypeDescriptor(ByVal objectType As Type, ByVal instance As Object) As ICustomTypeDescriptor
        Dim defaultDescriptor = MyBase.GetTypeDescriptor(objectType, instance)
        Return If(instance Is Nothing, defaultDescriptor, New ExpandoObjectTypeDescriptor(instance))
    End Function
End Class


