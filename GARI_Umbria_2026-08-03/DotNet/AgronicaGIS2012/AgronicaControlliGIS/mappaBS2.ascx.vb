
Imports System.Web.UI
Imports System.ComponentModel
Imports System.Web

<ToolboxData("<{0}:mappaBS2 runat=server></{0}:mappaBS2>")> Public Class mappaBS2
    Inherits UserControl



    Shared Sub New()
        If Not IsDesignMode Then
            System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(New AgronicaControlliGIS.AssemblyResourceProvider(ResourcePrefix))
        End If
    End Sub

    

    Private Shared ReadOnly Property IsDesignMode As Boolean
        Get
            Return HttpContext.Current Is Nothing
        End Get
    End Property

    Shared mResourcePrefix As String = "mappaBS2"

    Public Shared Property ResourcePrefix As String
        Get
            Return mResourcePrefix
        End Get
        Set(ByVal value As String)
            mResourcePrefix = value
        End Set
    End Property

    Private mAssemblyName As String = ""

    <Bindable(True), Category("Behavior"), Localizable(True)> Public Property AssemblyName As String
        Get
            Return mAssemblyName
        End Get
        Set(ByVal value As String)
            mAssemblyName = value
        End Set
    End Property

    Private mControlNamespace As String = ""

    <Bindable(True), Category("Behavior"), Localizable(True)> Public Property ControlNamespace As String
        Get
            Return mControlNamespace
        End Get
        Set(ByVal value As String)
            mControlNamespace = value
        End Set
    End Property

    Private mControlClassName As String = ""

    <Bindable(True), Category("Behavior"), Localizable(True)> Public Property ControlClassName As String
        Get
            Return mControlClassName
        End Get
        Set(ByVal value As String)
            mControlClassName = value
        End Set
    End Property

    Private ReadOnly Property Path As String
        Get
            Return String.Format("~/{0}/{1}.dll/{2}.{3}.ascx", ResourcePrefix, AssemblyName, ControlNamespace, ControlClassName)
        End Get
    End Property

    Private c As Control

    Protected Overrides Sub OnInit(ByVal e As EventArgs)
        If mAssemblyName <> "" Then
            c = Page.LoadControl(Path)
            Controls.Add(c)
            MyBase.OnInit(e)
        End If

    End Sub


    Public Sub SetControlProperty(ByVal propName As String, ByVal value As Object)
        c.[GetType]().GetProperty(propName).SetValue(c, value, Nothing)
    End Sub

    Public Function GetControlProperty(ByVal propName As String) As Object
        Return c.[GetType]().GetProperty(propName).GetValue(c, Nothing)
    End Function
End Class
