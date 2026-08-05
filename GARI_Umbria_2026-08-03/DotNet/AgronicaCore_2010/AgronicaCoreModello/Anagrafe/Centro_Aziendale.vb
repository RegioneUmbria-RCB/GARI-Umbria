
Namespace Anagrafe

    Public Class Centro_Aziendale

        Private _Azienda As Azienda
        Private _Sa_Cod As Integer
        Private _X As Integer = 0
        Private _Y As Integer = 0
        Private _Qadrante As Integer = 0
        Private _NomeStazioneMeteoDiRiferimento As String = ""

        Sub New(ByVal Piva_In As String, ByVal Sa_Cod_In As Integer)
            Azienda = New Azienda(Piva_In)
            Sa_Cod = Sa_Cod_In
        End Sub

        Sub New(ByVal Azienda_In As Azienda, ByVal Sa_Cod_In As Integer)
            Azienda = Azienda_In
            Sa_Cod = Sa_Cod_In
        End Sub

        Public Property Azienda() As Azienda
            Get
                Return _Azienda
            End Get
            Set(value As Azienda)
                _Azienda = value
            End Set
        End Property

        Public Property Sa_Cod() As Integer
            Get
                Return _Sa_Cod
            End Get
            Set(value As Integer)
                _Sa_Cod = value
            End Set
        End Property

        Public Function UgualeA(Centro_Aziendale_Da_Confrontare As Centro_Aziendale) As Boolean
            If Centro_Aziendale_Da_Confrontare.Sa_Cod = Sa_Cod AndAlso
                Centro_Aziendale_Da_Confrontare.Azienda.UgualeA(Azienda) Then
                Return True
            End If
            Return False
        End Function

        Public Property Qadrante() As Integer
            Get
                Return _Qadrante
            End Get
            Set(value As Integer)
                _Qadrante = Value
            End Set
        End Property

        Public Property X() As Integer
            Get
                Return _X
            End Get
            Set(value As Integer)
                _X = Value
            End Set
        End Property

        Public Property Y() As Integer
            Get
                Return _Y
            End Get
            Set(value As Integer)
                _Y = Value
            End Set
        End Property

        Public Property NomeStazioneMeteoDiRiferimento() As String
            Get
                Return _NomeStazioneMeteoDiRiferimento
            End Get
            Set(value As String)
                _NomeStazioneMeteoDiRiferimento = value
            End Set
        End Property

    End Class


End Namespace





