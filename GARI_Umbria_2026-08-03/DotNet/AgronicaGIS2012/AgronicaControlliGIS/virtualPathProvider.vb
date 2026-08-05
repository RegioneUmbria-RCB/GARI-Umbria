Imports System.Web.Hosting
Imports System.Web.Caching
Imports System.Collections
Imports System
Imports System.IO
Imports System.Web
Imports System.Reflection

Public Class AssemblyResourceProvider
    Inherits VirtualPathProvider

    Private mResourcePrefix As String

    Public Sub New()
        Me.New("EmbeddedWebResource")
    End Sub

    Public Sub New(ByVal prefix As String)
        mResourcePrefix = prefix
    End Sub

    Private Function IsAppResourcePath(ByVal virtualPath As String) As Boolean
        Dim checkPath As String = VirtualPathUtility.ToAppRelative(virtualPath)
        Return checkPath.StartsWith("~/" & mResourcePrefix & "/", StringComparison.InvariantCultureIgnoreCase)
    End Function

    Public Overrides Function FileExists(ByVal virtualPath As String) As Boolean
        Return (IsAppResourcePath(virtualPath) OrElse MyBase.FileExists(virtualPath))
    End Function

    Public Overrides Function GetFile(ByVal virtualPath As String) As VirtualFile
        If IsAppResourcePath(virtualPath) Then
            Return New AssemblyResourceVirtualFile(virtualPath)
        Else
            Return MyBase.GetFile(virtualPath)
        End If
    End Function

    Public Overrides Function GetCacheDependency(ByVal virtualPath As String, ByVal virtualPathDependencies As IEnumerable, ByVal utcStart As DateTime) As CacheDependency
        If IsAppResourcePath(virtualPath) Then
            Return Nothing
        Else
            Return MyBase.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart)
        End If
    End Function
End Class

Class AssemblyResourceVirtualFile
    Inherits VirtualFile

    Private path1 As String

    Public Sub New(ByVal virtualPath As String)
        MyBase.New(virtualPath)
        path1 = VirtualPathUtility.ToAppRelative(virtualPath)
    End Sub

    Public Overrides Function Open() As Stream
        Dim parts As String() = path1.Split("/"c)
        Dim assemblyName As String = parts(2)
        Dim resourceName As String = parts(3)
        assemblyName = Path.Combine(HttpRuntime.BinDirectory, assemblyName)
        Dim assembly As Assembly = Assembly.LoadFile(assemblyName)
        If assembly Is Nothing Then Throw New Exception("Failed to load " & assemblyName)
        Dim s As Stream = assembly.GetManifestResourceStream(resourceName)
        If s Is Nothing Then Throw New Exception("Failed to load " & resourceName)
        Return s
    End Function
End Class
