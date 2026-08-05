Public Class ServizioSchedulatoreFactory

    Private Shared objSingleton As ServizioSchedulatore
    Private Shared classLocker As New Object()
    Public Shared Function Instance() As ServizioSchedulatore

        If (objSingleton Is Nothing) Then
            ' Thread Safe
            SyncLock (classLocker)
                If (objSingleton Is Nothing) Then

                    objSingleton = New ServizioSchedulatore()

                End If
            End SyncLock
        End If
        Return objSingleton

    End Function

End Class
