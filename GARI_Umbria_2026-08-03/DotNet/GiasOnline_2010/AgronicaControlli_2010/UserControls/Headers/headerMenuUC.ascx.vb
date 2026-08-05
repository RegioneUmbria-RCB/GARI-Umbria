Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.Menu
Imports AgronicaCoreModelsSTD.profilazione

Imports System.Web.UI
Imports System.ComponentModel
Imports System.Web
Imports AgronicaCoreDataProvider

<ToolboxData("<{0}:headerMenuUC runat=server></{0}:headerMenuUC>")>
Public Class headerMenuUC
    Inherits UserControl

    Shared mResourcePrefix As String = "headerMenuUC"

    Public Shared Property ResourcePrefix As String
        Get
            Return mResourcePrefix
        End Get
        Set(ByVal value As String)
            mResourcePrefix = value
        End Set
    End Property

    Private mAssemblyName As String = ""
    Public Property AssemblyName As String
        Get
            Return mAssemblyName
        End Get
        Set(ByVal value As String)
            mAssemblyName = value
        End Set
    End Property

    Private mControlNamespace As String = ""
    Public Property ControlNamespace As String
        Get
            Return mControlNamespace
        End Get
        Set(ByVal value As String)
            mControlNamespace = value
        End Set
    End Property

    Private mControlClassName As String = ""
    Public Property ControlClassName As String
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


    Shared Sub New()
        'System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(New AgronicaControlliGIS.AssemblyResourceProvider(ResourcePrefix))

        System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(New AgronicaControlli_2010.AssemblyResourceProvider(ResourcePrefix))
    End Sub

    Private c As Control
    Protected Overrides Sub OnInit(ByVal e As EventArgs)
        Lingua.Gias_InizializzaCultura_DaSession()

        If mAssemblyName <> "" Then
            c = Page.LoadControl(Path)
            Controls.Add(c)
            MyBase.OnInit(e)
        End If

    End Sub






End Class