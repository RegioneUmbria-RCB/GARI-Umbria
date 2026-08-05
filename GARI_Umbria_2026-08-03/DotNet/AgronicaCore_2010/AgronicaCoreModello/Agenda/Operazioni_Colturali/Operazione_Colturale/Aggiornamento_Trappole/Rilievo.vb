Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

Namespace Agenda.Operazioni_Colturali.Operazione_Colturale.Aggiornamento_Trappole

    Public Class Rilievo

        Private _Trappola As Installazione_Trappole.Con_Inneschi.Trappola
        Private _NumeroAvversita As Integer

        Public Sub New(ByRef Trappola_Interessata As Installazione_Trappole.Con_Inneschi.Trappola, ByVal Numero_Avversita As Integer)
            Trappola = Trappola_Interessata
            NumeroAvversita = Numero_Avversita
        End Sub

        Public Property Trappola As Installazione_Trappole.Con_Inneschi.Trappola
            Get
                Return _Trappola
            End Get
            Set(value As Installazione_Trappole.Con_Inneschi.Trappola)
                _Trappola = value
            End Set
        End Property

        Public Property NumeroAvversita As Integer
            Get
                Return _NumeroAvversita
            End Get
            Set(value As Integer)
                _NumeroAvversita = value
            End Set
        End Property

    End Class

End Namespace
