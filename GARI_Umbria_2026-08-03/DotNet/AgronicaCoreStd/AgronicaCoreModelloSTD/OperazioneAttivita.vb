Public Class OperazioneAttivita

    Public Property operazione As Operazione
    Public Property attivita As Attivita

    Public ReadOnly Property Codice As String
        Get
            If operazione Is Nothing Then
                Return String.Format("{0}", attivita.Cod)
            ElseIf attivita Is Nothing Then
                Return String.Format("{0}", operazione.lav_cod)
            End If
            Return String.Format("{0}|{1}", operazione.lav_cod, attivita.Cod)
        End Get
    End Property

    Public ReadOnly Property Descrizione As String
        Get
            If operazione Is Nothing Then
                Return String.Format("{0}", attivita.Descrizione)
            ElseIf attivita Is Nothing Then
                Return String.Format("{0}", operazione.lav_des)
            End If
            Return String.Format("{0} - {1}", operazione.lav_des, attivita.Descrizione)
        End Get
    End Property

End Class
