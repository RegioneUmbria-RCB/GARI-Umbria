Public Class Allegato

    Public Property FullPath As String
    Public Property ContentType As String
    Public Property FileName As String
    Public Property Dimensione As Long

    Public ReadOnly Property Descrizione() As String
        Get

            Return ContentType & " (" & Math.Round(Dimensione / 1024) & " KB)"

        End Get
    End Property

    Public ReadOnly Property ImagePath() As String
        Get

            If ContentType.Contains("image/") Then
                Return FullPath
            ElseIf ContentType.Contains("video/") Then
                Return "movie.png"
            ElseIf ContentType.Contains("audio/") Then
                Return "audio.png"
            Else
                Return "file.png"
            End If

        End Get
    End Property

End Class
