Namespace Anagrafe

    Public Class Magazzino

        Private _Piva As String
        Private _Sa_Cod As Integer
        Private _Fabbricato_Cod As Integer

        Sub New(ByVal Piva_In As String, ByVal Sa_Cod_In As Integer, ByVal Fabbricato_Cod_In As Integer)
            _Piva = Piva_In
            _Sa_Cod = Sa_Cod_In
            _Fabbricato_Cod = Fabbricato_Cod_In
        End Sub

        Public ReadOnly Property Fabbricato_Cod() As Integer
            Get
                Return _Fabbricato_Cod
            End Get
        End Property

        Public ReadOnly Property Sa_Cod() As Integer
            Get
                Return _Sa_Cod
            End Get
        End Property

        Public ReadOnly Property Piva() As String
            Get
                Return _Piva
            End Get
        End Property

    End Class

End Namespace

