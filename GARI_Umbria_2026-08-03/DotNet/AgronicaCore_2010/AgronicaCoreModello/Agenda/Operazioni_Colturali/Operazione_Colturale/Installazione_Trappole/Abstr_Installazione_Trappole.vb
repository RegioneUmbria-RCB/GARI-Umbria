Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports System.Diagnostics.Contracts

Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Installazione_Trappole

    Public MustInherit Class Abstr_Installazione_Trappole
        Inherits Operazione_Colturale

        Private _Appezzamenti_Coinvolti As List(Of Anagrafe.Appezzamento)

        Protected _Numero_Trappole As Integer

        Private _Prodotto_ID As Integer = 0
        Private _Ditta_ID As Integer = 0
        Private _Avversita_ID As Integer = 0
        Private _Avversita_Sigla As String = ""
        Private _Specie As Integer = 0

        Public Sub New(Piva_Op As String, Data_Op As Date, Tipo_OperazioneDb As TipiEnumerativi.enum_TipoOperazioneDB, ByVal Id_Agenda_In As Integer, objParametri_Server As AgronicaCoreParametri)
            MyBase.new(Piva_Op, Data_Op, Tipo_OperazioneDb, Id_Agenda_In, objParametri_Server)
            ImpostaParametriOperazione(objParametri_Server)
        End Sub

        Public Sub New(ByRef OperazioneColturaleGenerica As I_Operazione_Colturale, ByRef objParametri_Server As AgronicaCoreParametri)
            MyBase.new(OperazioneColturaleGenerica, objParametri_Server)
            ImpostaParametriOperazione(objParametri_Server)
        End Sub

        Private Sub ImpostaParametriOperazione(objParametri_Server As AgronicaCoreParametri)
            _Appezzamenti_Coinvolti = New List(Of Anagrafe.Appezzamento)
        End Sub

        Public ReadOnly Property Appezzamenti_Coinvolti As List(Of Anagrafe.Appezzamento)
            Get
                Return _Appezzamenti_Coinvolti
            End Get
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

        Public Property Ditta_ID() As Integer
            Get
                Return _Ditta_ID
            End Get
            Set(value As Integer)
                _Ditta_ID = Value
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

        Public Property Specie() As Integer
            Get
                Return _Specie
            End Get
            Set(value As Integer)
                _Specie = Value
            End Set
        End Property

        Public Property Numero_Trappole() As Integer
            Get
                Return _Numero_Trappole
            End Get
            Set(value As Integer)
                _Numero_Trappole = Value
            End Set
        End Property

    End Class

End Namespace


