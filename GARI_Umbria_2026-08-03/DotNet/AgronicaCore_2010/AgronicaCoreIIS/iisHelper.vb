Imports System.Management
Imports System.DirectoryServices
Imports System.Text
Imports System.Collections.ObjectModel

Public Class iisHelper

    ''' <summary>
    ''' Send a Command To App Pool
    ''' </summary>
    ''' <param name="ConnectionUser"></param>
    ''' <param name="ConnectionPassword"></param>
    ''' <param name="Machine"></param>
    ''' <param name="appPool"></param>
    ''' <param name="command">Start, Stop</param>
    ''' <remarks></remarks>
    Public Shared Sub SendCommandToAppPool(ByVal ConnectionUser As String, ByVal ConnectionPassword As String, ByVal Machine As String, ByVal appPool As String, ByVal command As String)

        Dim co As New ConnectionOptions
        'co.Username = ConnectionUser;
        'co.Password = ConnectionPassword;
        co.Impersonation = ImpersonationLevel.Impersonate
        co.Authentication = AuthenticationLevel.PacketPrivacy
        Dim objPath As String = "IISApplicationPool.Name='" + appPool + "'" 'watch the single quotes 

        Dim scope As New ManagementScope("\\" & Machine & "\root\MicrosoftIISV2", co)

        Dim mc As New ManagementObject(objPath)
        mc.Scope = scope
        mc.InvokeMethod(command, Nothing, Nothing)

    End Sub

    ' ''' <summary>
    ' ''' Open a application pool and return an IISAppPool instance
    ' ''' </summary>
    ' ''' <param name="name">application pool name</param>
    ' ''' <returns>IISAppPool object</returns>
    'Public Shared Function OpenAppPool(name As String) As DirectoryEntry
    '    Dim connectStr As String = "IIS://localhost/W3SVC/AppPools/"
    '    connectStr += name

    '    Dim entry As New DirectoryEntry(connectStr)
    '    Return New IISAppPool(entry)
    'End Function

    Public Shared Sub StartAppPool(ByVal name As String)
        Dim connectStr As String = "IIS://localhost/W3SVC/AppPools/"
        connectStr += name

        Dim entry As New DirectoryEntry(connectStr)
        If Not entry Is Nothing Then
            entry.Invoke("Start")
        End If

    End Sub

    Public Shared Sub StopAppPool(ByVal name As String)
        Dim connectStr As String = "IIS://localhost/W3SVC/AppPools/"
        connectStr += name

        Dim entry As New DirectoryEntry(connectStr)
        If Not entry Is Nothing Then
            entry.Invoke("Stop")
        End If

    End Sub

    Public Shared Sub getPropertiesFromVirtualDir(ByVal path As String)
        Dim connectStr As String = "IIS://localhost/W3SVC"
        connectStr += path

        Dim entry As New DirectoryEntry(connectStr)
        If Not entry Is Nothing Then
            ListProperty(entry)
        End If

    End Sub

    Public Shared Sub ListProperty(ByVal server As DirectoryEntry)



        For Each e As DirectoryEntry In server.Children
            ListProperty(e)
        Next

        Dim sb As New StringBuilder
        sb.AppendLine("Property for " + server.SchemaClassName)
        sb.AppendLine("Name = " + server.Name)
        sb.AppendLine("Path = " + server.Path)
        sb.AppendLine("UserName = " + server.Username)
        sb.AppendLine("====================================================================")

        Dim ie As IEnumerator = server.Properties.PropertyNames.GetEnumerator()


        While (ie.MoveNext())
            Try
                Dim name As String = CStr(ie.Current)
                Dim val As String = ""

                For Each obj In server.Properties(name)
                    val &= obj.ToString & ","
                Next

                sb.AppendLine(name + " = " + val.ToString())
            Catch ex As Exception

            End Try

            Dim sw As New System.IO.StreamWriter("PropertyList_" + server.SchemaClassName + "_" + server.Name + ".txt")

            sw.Write(sb.ToString())
            sw.Close()

        End While




    End Sub


    Private _appPoolFound As List(Of String)
    Public Property AppPoolFound() As List(Of String)
        Get
            Return _appPoolFound
        End Get
        Set(value As List(Of String))
            _appPoolFound = value
        End Set
    End Property

    Public Sub FindappPoolFromPhisicalDir(ByVal server As DirectoryEntry, ByVal pathTosearch As List(Of String))


        If server Is Nothing Then
            Dim connectStr As String = "IIS://localhost/W3SVC/1/ROOT"
            server = New DirectoryEntry(connectStr)
            _appPoolFound = New List(Of String)
        End If


        For Each e As DirectoryEntry In server.Children
            FindappPoolFromPhisicalDir(e, pathTosearch)
        Next

        Dim ie As IEnumerator = server.Properties.PropertyNames.GetEnumerator()


        While (ie.MoveNext())
            Try
                Dim path As String = ""
                Dim appPool As String = ""

                path = pathHelper.addslash(getProp(server.Properties("Path")))


                If pathTosearch.Contains(path, New caseInsensitiveComparer) Then

                    appPool = getProp(server.Properties("AppPoolId"))
                    If Not _appPoolFound.Contains(appPool) Then
                        _appPoolFound.Add(appPool)
                    End If

                End If

            Catch ex As Exception
            End Try


        End While


    End Sub

    Private Function getProp(ByVal p As PropertyValueCollection) As String
        Dim rval As String = ""
        For Each obj In p
            rval = obj.ToString
        Next
        Return rval
    End Function


End Class

