Imports System.Web.UI

Public Class Ricerca

    Public Shared Function FindControlIterative(ByVal root As Control, ByVal id As String) As Control

        Dim ctl As Control = root
        Dim ctls As LinkedList(Of Control) = New LinkedList(Of Control)

        Do While (ctl IsNot Nothing)
            If ctl.ID = id Then
                Return ctl
            End If
            For Each child As Control In ctl.Controls
                If child.ID = id Then
                    Return child
                End If
                If child.HasControls() Then
                    ctls.AddLast(child)
                End If
            Next
            ctl = ctls.First.Value
            ctls.Remove(ctl)
        Loop

        Return Nothing

    End Function

    Public Shared Function FindControlIterative2(ByVal root As Control, ByVal id As String) As Control

        Dim ctl As Control = root
        Dim ctls As LinkedList(Of Control) = New LinkedList(Of Control)

        Do While (ctl IsNot Nothing)
            If ctl.ID = id Then
                Return ctl
            End If
            For Each child As Control In ctl.Controls
                If child.ID = id Then
                    Return child
                End If
                If child.HasControls() Then
                    ctls.AddLast(child)
                End If
            Next
            If ctls.Count > 0 Then
                ctl = ctls.First.Value
                ctls.Remove(ctl)
            Else
                ctl = Nothing
            End If

        Loop

        Return Nothing

    End Function

End Class
