Imports System.IO
Imports System.Reflection
Imports Microsoft.Build.Framework
Imports Microsoft.Build.Utilities

Public Class CreateChangelog
    Inherits Task

    <Required>
    Public Property WebDir As ITaskItem
    Public Property BinDir As ITaskItem

    Public Overrides Function Execute() As Boolean

        'TODO: DEBUG: Decommentare questa riga per debuggare
        'System.Diagnostics.Debugger.Launch()

        Dim success As Boolean = True

        If String.IsNullOrEmpty(WebDir.ItemSpec) Then
            Log.LogError("[{0}] La Web Directory non è stata indicata",
                         Me.GetType().FullName)
            success = False
        Else If Not Directory.Exists(WebDir.ItemSpec) Then
            Log.LogError("[{0}] La Web Directory {1} non esiste",
                         Me.GetType().FullName, WebDir.ItemSpec)
            success = False
        Else

            'Me.GetType().Assembly.Location

            'Carico la dll del sito specifico
            Dim dll As Assembly = Assembly.LoadFile(Path.Combine(BinDir.ItemSpec, "AgroAgenda_2010.dll"))
            Dim tipo As Type = dll.GetType("AgroAgenda_2010.AgendaVersione.Versione")

            CreaChangelogGeneric(tipo, Enumerativi.PROJECT_AGENDA, WebDir.ItemSpec)

        End If

        Return success

    End Function

    Private Sub CreaChangelogGeneric(ByVal tipo As Type, ByVal projectName As String, ByVal dirOutput As String)

        'Dim dynamicType As Type = GetType(T)
        Dim dynamicType As Type = tipo

        ' Create an instance of a Type by calling Activator.CreateInstance
        Dim dynamicObject As Object = Activator.CreateInstance(dynamicType, New Object() {projectName})

        dynamicType.InvokeMember("Create_Changelog",
                                 BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.InvokeMethod,
                                 Type.DefaultBinder, dynamicObject, New Object() {dirOutput})

    End Sub

End Class
