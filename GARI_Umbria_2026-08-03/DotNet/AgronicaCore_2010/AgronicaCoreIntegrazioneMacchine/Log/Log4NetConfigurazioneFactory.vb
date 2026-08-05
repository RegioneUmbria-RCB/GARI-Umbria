Public Class Log4NetConfigurazioneFactory

    Private Shared objSingleton As ConfiguratoreLog4Net
    Private Shared classLocker As New Object()
    Public Shared Function Instance() As ConfiguratoreLog4Net

        If (objSingleton Is Nothing) Then
            SyncLock (classLocker)
                If (objSingleton Is Nothing) Then

                    objSingleton = New ConfiguratoreLog4Net()

                End If
            End SyncLock
        End If
        Return objSingleton

    End Function
End Class


