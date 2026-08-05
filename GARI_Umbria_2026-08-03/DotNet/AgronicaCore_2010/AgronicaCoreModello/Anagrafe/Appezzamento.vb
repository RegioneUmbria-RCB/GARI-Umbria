
Namespace Anagrafe

    Public Class Appezzamento

        Private _Appezza As Integer
        Private _Centro_Aziendale As Centro_Aziendale

        Sub New(ByVal Piva_In As String, ByVal Sa_Cod_In As Integer, ByVal Appezza_In As Integer)
            Centro_Aziendale = New Centro_Aziendale(Piva_In, Sa_Cod_In)
            Appezza = Appezza_In
        End Sub

        Sub New(ByVal Centro_Aziendale_In As Centro_Aziendale, ByVal Appezza_In As Integer)
            Centro_Aziendale = Centro_Aziendale_In
            Appezza = Appezza_In
        End Sub

        Public Property Appezza() As Integer
            Get
                Return _Appezza
            End Get
            Set(value As Integer)
                _Appezza = Value
            End Set
        End Property

        Public Property Centro_Aziendale() As Centro_Aziendale
            Get
                Return _Centro_Aziendale
            End Get
            Set(value As Centro_Aziendale)
                _Centro_Aziendale = Value
            End Set
        End Property


        Public Function UgualeA(Appezzamento_Da_Confrontare As Appezzamento) As Boolean
            If Appezzamento_Da_Confrontare.Appezza = Appezza AndAlso
               Appezzamento_Da_Confrontare.Centro_Aziendale.UgualeA(Centro_Aziendale) Then
                Return True
            End If
            Return False
        End Function

    End Class

End Namespace
