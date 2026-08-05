Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Installazione_Trappole.Con_Inneschi

    Public Class Trappola

        Private _ID As Integer = 0
        Private _ID_Personalizzato As Integer = 0
        Private _NumeroInneschi As Integer = 0
        Private _Impianto_Di_Installazione As Anagrafe.Impianto_Colturale

        Private _Prodotto_ID As Integer = 0
        Private _Ditta_ID As Integer = 0
        Private _Avversita_ID As Integer = 0
        Private _Avversita_Sigla As String = ""

        Sub New()
            _ID = 0
            _ID_Personalizzato = 0
            _NumeroInneschi = 0
        End Sub

        Sub New(ByVal ID_In As Integer, ByVal ID_Personalizzato_In As Integer, ByRef Impianto_In As Anagrafe.Impianto_Colturale)
            _ID = ID_In
            _ID_Personalizzato = ID_Personalizzato_In
            _NumeroInneschi = 0
            _Impianto_Di_Installazione = Impianto_In
        End Sub

        Sub New(ByVal ID_In As Integer, ByVal ID_Personalizzato_In As Integer)
            _ID = ID_In
            _ID_Personalizzato = ID_Personalizzato_In
            _NumeroInneschi = 0
        End Sub

        Sub New(ByVal _ID_In As Integer)
            _ID = _ID_In
            _ID_Personalizzato = 0
            _NumeroInneschi = 0
        End Sub


        Public Property ID As Integer
            Get
                Return _ID
            End Get
            Set(value As Integer)
                _ID = value
            End Set
        End Property

        Public Property Impianto_Di_Installazione As Anagrafe.Impianto_Colturale
            Get
                Return _Impianto_Di_Installazione
            End Get
            Set(value As Anagrafe.Impianto_Colturale)
                _Impianto_Di_Installazione = value
            End Set
        End Property

        Public Property NumeroInneschi As Integer
            Get
                Return _NumeroInneschi
            End Get
            Set(value As Integer)
                _NumeroInneschi = value
            End Set
        End Property

        Public Property ID_Personalizzato As Integer
            Get
                Return _ID_Personalizzato
            End Get
            Set(value As Integer)
                _ID_Personalizzato = value
            End Set
        End Property

        Public Property Prodotto_ID() As Integer
            Get
                Return _Prodotto_ID
            End Get
            Set(value As Integer)
                _Prodotto_ID = Value
            End Set
        End Property

        Public Property Ditta_ID() As Integer
            Get
                Return _Ditta_ID
            End Get
            Set(value As Integer)
                _Ditta_ID = Value
            End Set
        End Property

        Public Property Avversita_ID() As Integer
            Get
                Return _Avversita_ID
            End Get
            Set(value As Integer)
                _Avversita_ID = Value
            End Set
        End Property

        Public Property Avversita_Sigla() As String
            Get
                Return _Avversita_Sigla
            End Get
            Set(value As String)
                _Avversita_Sigla = Value
            End Set
        End Property

    End Class

End Namespace

