Namespace Anagrafe

    Public Class Azienda

        Private _Piva As String

        Sub New(Piva_in As String)
            Piva = Piva_in
        End Sub

        Public Property Piva() As String
            Get
                Return _Piva
            End Get
            Set(value As String)
                _Piva = Value
            End Set
        End Property

        Public Function UgualeA(Azienda_Da_Confrontare As Azienda) As Boolean
            If Azienda_Da_Confrontare.Piva = Piva Then
                Return True
            End If
            Return False
        End Function

    End Class

End Namespace

