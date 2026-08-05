Module RetryHelper

    Function RiprovaSuEccezione(Of T)(ByVal tentativi As Integer, ByVal delay As TimeSpan, ByVal azione As Func(Of T)) As T

        Dim attempts = 0
        Dim result As T = Nothing

        Do

            Try
                attempts += 1
                result = azione()
                Exit Do
            Catch ex As Exception
                If attempts = tentativi Then Throw New MaxRetryException()
                Threading.Thread.Sleep(delay)
            End Try

        Loop While True

        Return result

    End Function

End Module
