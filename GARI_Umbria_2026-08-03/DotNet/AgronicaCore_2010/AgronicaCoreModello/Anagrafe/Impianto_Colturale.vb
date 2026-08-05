Namespace Anagrafe
    Public Class Impianto_Colturale

        Private _Piva As String
        Private _Sa_Cod As Integer
        Private _Appezza As Integer
        Private _ID_Reg As Integer
        Private _Progetto_Cod As Integer
        Private _Validita_Inizio As Date
        Private _Validita_Fine As Date
        Private _Qta2 As Decimal

        Private _Appezzamento As Appezzamento

        Sub New(ByVal Piva_In As String, ByVal Sa_Cod_In As Integer, ByVal Appezza_In As Integer, ByVal ID_Reg_In As Integer, ByVal Progetto_Cod_In As Integer, ByVal Validita_Inizio_In As Date, ByVal Validita_fine_In As Date, Optional ByVal Qta2_In As Decimal = 0)
            Piva = Piva_In
            Sa_Cod = Sa_Cod_In
            Appezza = Appezza_In
            ID_Reg = ID_Reg_In
            Progetto_Cod = Progetto_Cod_In
            Validita_Inizio = Validita_Inizio_In
            Validita_Fine = Validita_fine_In
            Qta2 = Qta2_In

            Appezzamento = New Appezzamento(Piva_In, Sa_Cod_In, Appezza_In)
        End Sub

        ''' <summary>
        ''' Vanni, costruttore generico da non rimuovere!
        ''' </summary>
        Sub New()

        End Sub
        
        Public Property Appezzamento() As Appezzamento
            Get
                Return _Appezzamento
            End Get
            Set(value As Appezzamento)
                _Appezzamento = value
            End Set
        End Property

        Public Property ID_Reg As Integer
            Get
                Return _ID_Reg
            End Get
            Set(value As Integer)
                _ID_Reg = value
            End Set
        End Property

        Public Property Appezza As Integer
            Get
                Return _Appezza
            End Get
            Set(value As Integer)
                _Appezza = value
            End Set
        End Property

        Public Property Sa_Cod As Integer
            Get
                Return _Sa_Cod
            End Get
            Set(value As Integer)
                _Sa_Cod = value
            End Set
        End Property

        Public Property Piva As String
            Get
                Return _Piva
            End Get
            Set(value As String)
                _Piva = value
            End Set
        End Property

        Public Property Progetto_Cod As Integer
            Get
                Return _Progetto_Cod
            End Get
            Set(value As Integer)
                _Progetto_Cod = value
            End Set
        End Property

        Public Property Validita_Inizio As Date
            Get
                Return _Validita_Inizio
            End Get
            Set(value As Date)
                _Validita_Inizio = value
            End Set
        End Property

        Public Property Validita_Fine As Date
            Get
                Return _Validita_Fine
            End Get
            Set(value As Date)
                _Validita_Fine = value
            End Set
        End Property

        Public Property Qta2 As Decimal
            Get
                Return _Qta2
            End Get
            Set(value As Decimal)
                _Qta2 = value
            End Set
        End Property

        Public Function UgualeA(Impianto_Da_Confrontare As Impianto_Colturale) As Boolean
            If Impianto_Da_Confrontare._ID_Reg = ID_Reg AndAlso
               Impianto_Da_Confrontare._Piva = Piva AndAlso
               Impianto_Da_Confrontare._Sa_Cod = Sa_Cod AndAlso
               Impianto_Da_Confrontare._Appezza = Appezza Then
                Return True
            End If
            Return False
        End Function

    End Class

End Namespace

